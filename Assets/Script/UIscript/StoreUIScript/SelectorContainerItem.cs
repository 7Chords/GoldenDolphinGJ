using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SelectorContainerItem : MonoBehaviour
{
    [Header("DOTween 弹出缩放设置")]
    [SerializeField] private float initialScaleFactor = 0.6f;   // 初始缩放（从多小开始）
    [SerializeField] private float overshootScaleFactor = 1.12f; // 放大到的比例（可略大于1用于过冲效果）
    [SerializeField] private float totalDuration = 0.28f;       // 总时长（秒）
    [SerializeField] private Ease ease = Ease.OutBack;          // 放大缓动（建议 OutBack 有弹性效果）
    public Image curSelectorImage;
    private Vector3 originalScale;
    private Sequence popSequence;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
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
        // 确保禁用时恢复原始缩放
        transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        if (popSequence != null && popSequence.IsActive())
        {
            popSequence.Kill();
            popSequence = null;
        }
    }

    public void SetItemInfo()
    {
        // 设置物品信息的逻辑
        //curSelectorImage
    }

}