using DG.Tweening;
using GJFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorContainerItem : MonoBehaviour, IPointerClickHandler
{
    [Header("DOTween 弹出缩放设置")]
    [SerializeField] private float initialScaleFactor = 0.6f;   // 初始缩放（从多小开始）
    [SerializeField] private float overshootScaleFactor = 1.12f; // 放大到的比例（可略大于1用于过冲效果）
    [SerializeField] private float totalDuration = 0.28f;       // 总时长（秒）
    [SerializeField] private Ease ease = Ease.OutBack;          // 放大缓动（建议 OutBack 有弹性效果）
    [SerializeField] private GameObject parentGameObject;
    public Sprite DefaultSprite;
    private bool isSelected = false;
    public Image preSelectorImage;
    private Vector3 originalScale;
    private Sequence popSequence;
    private Sequence disappearSequence;
    private long storeItemId;// 商店商品Id
    public GameObject ParentGameObject
    {
        get => parentGameObject;
    }
    public long StoreItemId
    {
        get => storeItemId;
        set => storeItemId = value;
    }
    public void SetDefault()
    {
        preSelectorImage.sprite = DefaultSprite;
    }
    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }


    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        isSelected = true;
        // 恢复图片透明度与缩放，避免上次消失遗留状态
        if (preSelectorImage != null)
        {
            var c = preSelectorImage.color;
            preSelectorImage.color = new Color(c.r, c.g, c.b, 1f);
            preSelectorImage.enabled = true;
        }
        transform.localScale = originalScale;
        // 启用时自动播放弹出效果（若不需要自动播放，可移除此行）
        PlayPop();
    }
    /// <summary>
    /// 播放从小变大再回到正常的效果
    /// </summary>
    public void PlayPop()
    {
        // 终止已有动画并重置为原始缩放的初始状态
        if (popSequence != null && popSequence.IsActive())
        {
            popSequence.Kill();
            popSequence = null;
        }

        // 设置起始为较小的比例，再播放动画
        transform.localScale = originalScale * initialScaleFactor;

        float half = Mathf.Max(0.001f, totalDuration * 0.5f);
        popSequence = DOTween.Sequence();
        popSequence.Append(transform.DOScale(originalScale * overshootScaleFactor, half).SetEase(ease));
        popSequence.Append(transform.DOScale(originalScale, half).SetEase(Ease.OutQuad));
        popSequence.OnComplete(() =>
        {
            transform.localScale = originalScale;
            popSequence = null;
        });
    }

    private void OnDisable()
    {
        if (popSequence != null && popSequence.IsActive())
        {
            popSequence.Kill();
            popSequence = null;
        }
        if (disappearSequence != null && disappearSequence.IsActive())
        {
            disappearSequence.Kill();
            disappearSequence = null;
        }
        // 确保禁用时恢复原始缩放与状态
        transform.localScale = originalScale;
        isSelected = false;
    }

    private void OnDestroy()
    {
        if (popSequence != null && popSequence.IsActive())
        {
            popSequence.Kill();
            popSequence = null;
        }
        if (disappearSequence != null && disappearSequence.IsActive())
        {
            disappearSequence.Kill();
            disappearSequence = null;
        }
    }

    public void SetItemInfo(Sprite _sprite, long _storeItemId)
    {
        preSelectorImage.sprite = _sprite;
        if (preSelectorImage != null)
        {
            var c = preSelectorImage.color;
            preSelectorImage.enabled = true;
        }
        // 用于回调告诉回传了什么商品Id
        storeItemId = _storeItemId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (preSelectorImage.raycastTarget == false) return;

        preSelectorImage.raycastTarget = false;

        // 停掉已有动画
        if (popSequence != null && popSequence.IsActive())
        {
            popSequence.Kill();
            popSequence = null;
        }
        if (disappearSequence != null && disappearSequence.IsActive())
        {
            disappearSequence.Kill();
            disappearSequence = null;
        }

        // 确保图片可见并重置 alpha
        if (preSelectorImage != null)
        {   
            preSelectorImage.DOKill();
            preSelectorImage.enabled = true;
            var col = preSelectorImage.color;
            preSelectorImage.color = new Color(col.r, col.g, col.b, 1f);
        }
        float growDuration = Mathf.Max(0.05f, totalDuration * 0.6f);
        float shrinkDuration = Mathf.Max(0.05f, totalDuration * 0.6f);

        popSequence = DOTween.Sequence();
        // 缓慢变大（平滑出力）
        popSequence.Append(transform.DOScale(originalScale * overshootScaleFactor, growDuration).SetEase(Ease.OutCubic));
        // 缓慢变小（平滑收力）
        popSequence.Append(transform.DOScale(originalScale * initialScaleFactor, shrinkDuration).SetEase(Ease.InCubic));

        // 图片在整个过程中平滑淡出（根据需要可改为只在缩小时淡出）
        if (preSelectorImage != null)
        {
            preSelectorImage.DOKill();
            preSelectorImage.enabled = true;
            var col = preSelectorImage.color;
            preSelectorImage.color = new Color(col.r, col.g, col.b, 1f);

            popSequence.Join(preSelectorImage.DOFade(0f, growDuration + shrinkDuration).SetEase(Ease.InOutQuad));
        }

        popSequence.OnComplete(() =>
        {

            MsgCenter.SendMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_BEFORE_COMPLETE, storeItemId);
            MsgCenter.SendMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_WHILE_DOTWEEN_COMPLETE, storeItemId);
            // 动画结束后禁用图片并恢复 alpha，避免下次复用时出现问题
            if (preSelectorImage != null)
            {
                var c = preSelectorImage.color;
                preSelectorImage.color = new Color(c.r, c.g, c.b, 1f);
                gameObject.SetActive(false);
            }
            // 恢复缩放以防下次显示继承缩小状态
            transform.localScale = originalScale;
            popSequence = null;
            preSelectorImage.raycastTarget = true;
        });
    }
}