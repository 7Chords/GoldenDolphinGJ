using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using GJFramework;

public class StoreContainerItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("DOTween 点击缩放设置")]
    [SerializeField] private float clickScale = 1.15f;    // 放大目标比例
    [SerializeField] private float totalDuration = 0.22f; // 放大+缩小总时长（秒）
    [SerializeField] private Ease ease = Ease.OutBack;    // 缓动类型
    [SerializeField] private Image StoreItemImg;
    [SerializeField] private List<Image> StoreItemImgs;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Text NoteNumText1;
    [SerializeField] private Text NoteNumText2;
    [SerializeField] private StoreItemPictureSelector storeItemPictureSelector;
    [Tooltip("缩放过渡时长（秒）")]
    [SerializeField] private float scaleDuration = 0.12f;
    [Tooltip("缩放缓动类型")]
    [SerializeField] private Ease scaleEase = Ease.OutQuad;
    [Tooltip("是否使用 UnscaledTime（忽略 Time.timeScale）")]
    [SerializeField] private bool useUnscaledTime = true;
    [Tooltip("悬停时目标缩放")]
    private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.0f);
    [Tooltip("按下时目标缩放（相对于 hoverScale 的比例）")]
    [SerializeField] private float pressedScaleMultiplier = 0.95f;
    private Tween scaleTween;
    private Vector3 originalScale;
    private Sequence clickSequence;
    public long storeItemId;// 商店商品Id
    TweenContainer tweenContainer = new TweenContainer();
    public string clipName;
    private bool isBought = false;// 商品是否被选中
    int highNoteCost;
    int middleCost;
    int lowCost;
    bool isLock = false;
    // 缓存原始颜色以便恢复
    private Color originalImageColor;
    private InstrumentStoreRefObj instrumentStoreRefObj = null;

    // 缓存每个 Image 的原始颜色
    private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>();
    // 缓存 NoteNumText 的原始颜色
    private Dictionary<Text, Color> originalTextColors = new Dictionary<Text, Color>();

    // 悬停显示配置
    [SerializeField] private float hoverDelay = 0.4f; // 悬停延迟
    [SerializeField] private float hoverFadeDuration = 0.2f; // 淡入时长
    private Tween hoverDelayTween;
    public InstrumentStoreRefObj InstrumentStoreRefObj => instrumentStoreRefObj;

    private void Awake()
    {
        originalScale = transform.localScale;

        // 初始化原始颜色缓存：优先使用 StoreItemImgs 列表
        originalImageColors.Clear();
        if (StoreItemImgs != null && StoreItemImgs.Count > 0)
        {
            foreach (var img in StoreItemImgs)
            {
                if (img != null && !originalImageColors.ContainsKey(img))
                    originalImageColors[img] = img.color;
            }
            // 同步 originalImageColor 为第一个可用图像的颜色，保持兼容旧逻辑
            foreach (var kv in originalImageColors)
            {
                originalImageColor = kv.Value;
                break;
            }
        }
        else
        {
            if (StoreItemImg != null)
            {
                originalImageColor = StoreItemImg.color;
                originalImageColors[StoreItemImg] = originalImageColor;
            }
            else
            {
                originalImageColor = Color.white;
            }
        }

        // 缓存文本原始颜色
        originalTextColors.Clear();
        if (NoteNumText1 != null && !originalTextColors.ContainsKey(NoteNumText1))
            originalTextColors[NoteNumText1] = NoteNumText1.color;
        if (NoteNumText2 != null && !originalTextColors.ContainsKey(NoteNumText2))
            originalTextColors[NoteNumText2] = NoteNumText2.color;
    }

    public void SetInfo(long unlockLvId)
    { 
        if (storeItemPictureSelector != null)
        {
            isLock = storeItemPictureSelector.SetInfo(unlockLvId);
        }
    }

    public void Init()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_WHILE_DOTWEEN_COMPLETE, ResumeColor);
        instrumentStoreRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
   .Find(x => x.id == storeItemId);
        if (instrumentStoreRefObj == null) Debug.Log(storeItemId);
        highNoteCost = instrumentStoreRefObj.hightNoteNum;
        middleCost = instrumentStoreRefObj.middleNoteNum;
        lowCost = instrumentStoreRefObj.lowNoteNum;
    }
    private void Start()
    {
    }

    private void ResumeColor(object[] objs)
    {
        long tempStoreItemId = (long)objs[0];
        if(tempStoreItemId == storeItemId)
        {
            // 恢复所有缓存的图像和文本颜色
            RestoreAllImagesColor();
        }
        SetGrayState(tempStoreItemId);
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

    public void SetGrayState(long curSelectStoreItem, bool isSelect = false)
    {
        // 如果没有任何 image，可以直接返回
        if ((StoreItemImgs == null || StoreItemImgs.Count == 0) && StoreItemImg == null) return;

        // 如果 originalImageColor 是默认的透明值，回退为当前图片颜色，避免把图片设为完全透明
        if (originalImageColor.a == 0f)
        {
            if (StoreItemImg != null) originalImageColor = StoreItemImg.color;
            else if (StoreItemImgs != null && StoreItemImgs.Count > 0)
            {
                foreach (var i in StoreItemImgs) { if (i != null) { originalImageColor = i.color; break; } }
            }
        }

        bool temp = (PlayerMgr.Instance.GetNoteNum(NoteType.HightNote) >= highNoteCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote) >= middleCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.LowNote) >= lowCost);

        // 如果资源不够 就设置灰色
        if (!temp)
        {
            SetAllImagesToGray();
        }
        else
        {
            // 如果这个商品是从商店->selector的 即使是够的也设置为灰色
            if (isSelect && curSelectStoreItem == storeItemId)
            {
                SetAllImagesToGray();
            }
            else
            {
                // 考虑是否还在列表里 不在列表里才恢复颜色 否则保持灰色
                if (curSelectStoreItem == -1 || !StoreItemContainer.instance.storeItemList[storeItemId])
                    RestoreAllImagesColor();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果是锁定状态 则不响应点击
        if (!isLock) return;

        if (!isCanBuy())
        {
            // 购买失败时震动并做短暂位置抖动作为视觉反馈

            // 一点位移抖动反馈
            // 如果这是UI（RectTransform），DOShakePosition 也可作用在 transform 上
            tweenContainer.RegDoTween(transform.DOShakePosition(0.12f, 20f, 10, 90f, false));
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
            // 如果资源更改了 就立即判断是否设置为灰色
            MsgCenter.SendMsg(MsgConst.ON_STORE_ITEM_SELECT, selectedSprite, storeItemId);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 如果是锁定状态 则不响应
        if (!isLock) return;
        // 取消已有延迟
        hoverDelayTween?.Kill();
        // 开始缩放 到 hover 大小
        StartScale(hoverScale);


        // 延迟显示
        hoverDelayTween = DOVirtual.DelayedCall(hoverDelay, () =>
        {
            PanelUIMgr.Instance.OpenPanel(EPanelType.StoreItemInfoPanel);
            var panel = PanelUIMgr.Instance.GetCachedPanel(EPanelType.StoreItemInfoPanel) as StoreItemInfoPanel;
            if (panel != null && instrumentStoreRefObj != null)
            {
                // 传入淡入时长，由 panel 负责淡入
                panel.ShowInfo(instrumentStoreRefObj.instrumentId, eventData.position, GetComponent<RectTransform>(), hoverFadeDuration);
            }
        }, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 如果是锁定状态 则不响应
        if (!isLock) return;
        // 取消未到时的延迟显示
        hoverDelayTween?.Kill();
        hoverDelayTween = null;
        StartScale(originalScale);
        // 隐藏面板 不淡出
        var panel = PanelUIMgr.Instance.GetCachedPanel(EPanelType.StoreItemInfoPanel) as StoreItemInfoPanel;
        if (panel != null)
        {
            panel.HideInfo();
        }
    }

    private void OnDisable()
    {
        hoverDelayTween?.Kill();
        hoverDelayTween = null;

        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
            clickSequence = null;
            transform.localScale = originalScale;
        }

        // 组件失效时确保 tween 被清理
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
            scaleTween = null;
        }

    }
    private bool isCanBuy()
    {
        InstrumentStoreRefObj instrumentStoreRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
.Find(x => x.id == storeItemId);


        bool temp = (PlayerMgr.Instance.GetNoteNum(NoteType.HightNote) >= highNoteCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote) >= middleCost &&
               PlayerMgr.Instance.GetNoteNum(NoteType.LowNote) >= lowCost);

        // 如果已经包含了该乐器 则不能购买
        temp &= !StoreItemContainer.instance.storeItemList[storeItemId];
        // 且现在乐器列表不能满
        temp &= PlayerMgr.Instance.instrumentIdList.Count < PlayerMgr.Instance.GetCurrentMaxInstrumentNum();
        // 还要判断当前乐器是否已满

        if (temp)
        {
            // 如果买得起直接加入玩家乐器列表 并且扣除对应音符数量
            PlayerMgr.Instance.instrumentIdList.Add(instrumentStoreRefObj.instrumentId);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.LowNote, lowCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.HightNote, highNoteCost);
            PlayerMgr.Instance.RemoveNoteNum(NoteType.MiddleNote, middleCost);
            MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);

            // 购买成功后把所有 image/text 设置为灰色
            SetAllImagesToGray();
        }
        return temp;
    }
    private void OnDestroy()
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_WHILE_DOTWEEN_COMPLETE, ResumeColor);
        tweenContainer.KillAllDoTween();
    }

    private void StartScale(Vector3 target)
    {
        // 取消现有 tween
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
            scaleTween = null;
        }

        // 直接设置
        if (scaleDuration <= 0f)
        {
            transform.localScale = target;
            return;
        }

        // 创建新的 DOTween 缩放并保存引用
        scaleTween = transform.DOScale(target, scaleDuration)
            .SetEase(scaleEase)
            .SetUpdate(useUnscaledTime) // 使用 unscaled 时间以匹配之前的实现
            .OnKill(() => scaleTween = null);
    }

    // 将所有目标 Image 和指定 Text 置灰
    private void SetAllImagesToGray()
    {
        if (StoreItemImgs != null && StoreItemImgs.Count > 0)
        {
            foreach (var img in StoreItemImgs)
            {
                if (img == null) continue;
                Color ori = originalImageColors.ContainsKey(img) ? originalImageColors[img] : img.color;
                img.color = new Color(0.5f, 0.5f, 0.5f, ori.a);
            }
        }
        else if (StoreItemImg != null)
        {
            var col = StoreItemImg.color;
            StoreItemImg.color = new Color(0.5f, 0.5f, 0.5f, col.a);
        }

        // 对文本也做同样处理（保留原始 alpha）
        if (NoteNumText1 != null)
        {
            Color ori = originalTextColors.ContainsKey(NoteNumText1) ? originalTextColors[NoteNumText1] : NoteNumText1.color;
            NoteNumText1.color = new Color(0.5f, 0.5f, 0.5f, ori.a);
        }
        if (NoteNumText2 != null)
        {
            Color ori = originalTextColors.ContainsKey(NoteNumText2) ? originalTextColors[NoteNumText2] : NoteNumText2.color;
            NoteNumText2.color = new Color(0.5f, 0.5f, 0.5f, ori.a);
        }
    }

    // 恢复所有目标 Image 和指定 Text 的原始颜色
    private void RestoreAllImagesColor()
    {
        if (StoreItemImgs != null && StoreItemImgs.Count > 0)
        {
            foreach (var img in StoreItemImgs)
            {
                if (img == null) continue;
                if (originalImageColors.ContainsKey(img))
                    img.color = originalImageColors[img];
            }
        }
        else if (StoreItemImg != null)
        {
            StoreItemImg.color = originalImageColor;
        }

        // 恢复文本颜色
        if (NoteNumText1 != null && originalTextColors.ContainsKey(NoteNumText1))
            NoteNumText1.color = originalTextColors[NoteNumText1];
        if (NoteNumText2 != null && originalTextColors.ContainsKey(NoteNumText2))
            NoteNumText2.color = originalTextColors[NoteNumText2];
    }
}