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
    [SerializeField] private long stroeItemId;// 商店商品Id
    TweenContainer tweenContainer = new TweenContainer();
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!isCanBuy())
        {
            // 购买失败时震动（移动设备）并做短暂位置抖动作为视觉反馈（编辑器也能看到）
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
            // 一点位移抖动反馈
            // 如果这是UI（RectTransform），DOShakePosition 也可作用在 transform 上
            tweenContainer.RegDoTween(transform.DOShakePosition(0.12f, 20f, 10, 90f, false));
        }
        else
        {
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
            MsgCenter.SendMsg(MsgConst.ON_STORE_ITEM_SELECT, selectedSprite);
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
.Find(x => x.id == stroeItemId);

        int highNoteCost = instrumentStoreRefObj.hightNoteNum;
        int middleCost = instrumentStoreRefObj.middleNoteNum;
        int lowCost = instrumentStoreRefObj.lowNoteNum;

        bool temp = (PlayerMgr.Instance.GetNoteNum(NoteType.HightNote) >= highNoteCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.LowNote) >= middleCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote) >= lowCost);

        if (temp)
        {
            // 如果买得起直接加入玩家乐器列表 并且扣除对应音符数量
            PlayerMgr.Instance.instrumentIdList.Add(instrumentStoreRefObj.instrumentId);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.LowNote, lowCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.HightNote, highNoteCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.MiddleNote, middleCost);
        }

        return temp;
    }
    private void OnDestroy()
    {
        tweenContainer.KillAllDoTween();
    }
}