using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using GJFramework;

public class StoreContainerItem : MonoBehaviour, IPointerClickHandler
{
    [Header("DOTween 点击缩放设置")]
    [SerializeField] private float clickScale = 1.15f;    // 放大目标比例
    [SerializeField] private float totalDuration = 0.22f; // 放大+缩小总时长（秒）
    [SerializeField] private Ease ease = Ease.OutBack;    // 缓动类型
    [SerializeField] private Image StoreItemImg;
    [SerializeField] private Sprite selectedSprite;
    private Vector3 originalScale;
    private Sequence clickSequence;
    [SerializeField] private long storeItemId;// 商店商品Id
    TweenContainer tweenContainer = new TweenContainer();
    public string clipName;
    private bool isBought = false;// 商品是否被选中

    // 缓存原始颜色以便恢复
    private Color originalImageColor;

    private void Awake()
    {
        originalScale = transform.localScale;
        if (StoreItemImg != null)
            originalImageColor = StoreItemImg.color;
        else
            originalImageColor = Color.white;
    }

    private void Start()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_IMMEDIATE, ResumeColor);
    }

    private void ResumeColor(object[] objs)
    {
        long tempStoreItemId = (long)objs[0];
        if(tempStoreItemId == storeItemId)
        {
            if (StoreItemImg != null)
                StoreItemImg.color = originalImageColor;
        }
    }
    public long StoreItemId
    {
        get => storeItemId;
        set => storeItemId = value;
    }
    public bool IsSelected
    {
        get => isBought;
        set => isBought = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!isCanBuy())
        {
            // 购买失败时震动并做短暂位置抖动作为视觉反馈

            // 一点位移抖动反馈
            // 如果这是UI（RectTransform），DOShakePosition 也可作用在 transform 上
            tweenContainer.RegDoTween(transform.DOShakePosition(0.12f, 20f, 10, 90f, false));
            // 立即将商品图片置为灰色
        }
        else
        {
            AudioMgr.Instance.PlaySfx(clipName);
            // 终止已有动画
            if (clickSequence != null && clickSequence.IsActive())
            {
                clickSequence.Kill();
                clickSequence = null;
                transform.localScale = originalScale;
            }

            // 构建放大 -> 缩小 的 Sequence
            float half = Mathf.Max(0.001f, totalDuration * 0.5f);
            clickSequence = DOTween.Sequence();
            clickSequence.Append(transform.DOScale(originalScale * clickScale, half).SetEase(ease));
            clickSequence.Append(transform.DOScale(originalScale, half).SetEase(ease));
            clickSequence.OnComplete(() =>
            {
                transform.localScale = originalScale;
                clickSequence = null;
            });

            MsgCenter.SendMsg(MsgConst.ON_STORE_ITEM_SELECT, selectedSprite, storeItemId);
        }

    }

    private void OnDisable()
    {
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
            clickSequence = null;
            transform.localScale = originalScale;
        }
    }
    private bool isCanBuy()
    {
        InstrumentStoreRefObj instrumentStoreRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
.Find(x => x.id == storeItemId);

        int highNoteCost = instrumentStoreRefObj.hightNoteNum;
        int middleCost = instrumentStoreRefObj.middleNoteNum;
        int lowCost = instrumentStoreRefObj.lowNoteNum;

        bool temp = (PlayerMgr.Instance.GetNoteNum(NoteType.HightNote) >= highNoteCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote) >= middleCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.LowNote) >= lowCost);

        // 如果已经包含了该乐器 则不能购买
        temp &= !StoreItemContainer.instance.storeItemList[storeItemId];
        // 且现在乐器列表不能满
        temp &= PlayerMgr.Instance.instrumentIdList.Count < 3;

        if (temp)
        {
            // 如果买得起直接加入玩家乐器列表 并且扣除对应音符数量
            PlayerMgr.Instance.instrumentIdList.Add(instrumentStoreRefObj.instrumentId);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.LowNote, lowCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.HightNote, highNoteCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.MiddleNote, middleCost);
            MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);
            if (StoreItemImg != null)
            {
                var col = StoreItemImg.color;
                StoreItemImg.color = new Color(0.5f, 0.5f, 0.5f, col.a);
            }
        }
        return temp;
    }
    private void OnDestroy()
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_IMMEDIATE, ResumeColor);
        tweenContainer.KillAllDoTween();
    }
}