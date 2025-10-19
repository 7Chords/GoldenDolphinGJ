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

    [Header("DOTween 消失动画设置")]
    [SerializeField] private float disappearDuration = 0.18f;    // 点击时消失动画时长
    [SerializeField] private Ease disappearEase = Ease.InQuad;   // 消失缓动
    [SerializeField] private float disappearScaleFactor = 0.6f;  // 缩小到的比例
    [SerializeField] private bool deactivateAfterDisappear = true;// 动画结束后是否禁用整个物体
    public Sprite DefaultSprite;
    private bool isSelected = false;
    public Image preSelectorImage;
    private Vector3 originalScale;
    private Sequence popSequence;
    private Sequence disappearSequence;
    private bool isAnimating = false;

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
        isAnimating = false;
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

    public void SetItemInfo(Sprite _sprite, int index)
    {
        preSelectorImage.sprite = _sprite;
        if (preSelectorImage != null)
        {
            var c = preSelectorImage.color;
            preSelectorImage.color = new Color(c.r, c.g, c.b, 1f);
            preSelectorImage.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 还没有图片就不响应点击
        if (preSelectorImage == null || preSelectorImage.sprite == null) return;

        // 如果正在执行动画，忽略重复点击
        if (isAnimating) return;

        PlayDisappear();
    }

    /// <summary>
    /// 播放消失动画：同时淡出图片和缩小物体，结束后根据设置禁用物体或隐藏图片
    /// </summary>
    public void PlayDisappear()
    {
        isAnimating = true;

        // 终止可能正在播放的弹出动画
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

        disappearSequence = DOTween.Sequence();

        // 缩放（到指定比例）
        disappearSequence.Join(transform.DOScale(originalScale * disappearScaleFactor, disappearDuration).SetEase(disappearEase));
        // 图片淡出（如果存在）
        if (preSelectorImage != null)
        {
            disappearSequence.Join(preSelectorImage.DOFade(0f, disappearDuration).SetEase(disappearEase));
        }

        disappearSequence.OnComplete(() =>
        {
            // 动画结束：重置状态或禁用
            isSelected = false;
            isAnimating = false;

            if (preSelectorImage != null)
            {
                // 隐藏图片资源，避免下一次显示时仍为透明
                preSelectorImage.enabled = false;
            }

            if (deactivateAfterDisappear)
            {
                gameObject.SetActive(false);
            }
            else
            {
                // 若不禁用 GameObject，则恢复缩放到原始（但保持图片隐藏）
                transform.localScale = originalScale;
            }

            disappearSequence = null;
            // todo: ryanReturnMaterial
            ReturnMaterial();
        });
        
    }
    public void ReturnMaterial()
    {

    }
}