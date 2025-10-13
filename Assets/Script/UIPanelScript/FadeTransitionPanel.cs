using DG.Tweening;
using GJFramework;
using UnityEngine;
using UnityEngine.UI;

// 具体过渡效果：淡入淡出（继承中间基类UITransitionBase）
public class FadeTransition : TransitionPanelBase
{
    // 淡入淡出专属配置（只此过渡需要，不影响其他过渡）
    [Header("淡入淡出专属配置")]
    [SerializeField] private CanvasGroup fadeMask; // 淡入淡出需要的遮罩（子类特有）
    [SerializeField] private float fadeInRatio = 0.5f; // 淡入占总时长的比例（如0.5=总时长的一半用于淡入）

    // 实现基类的“过渡开始”方法  写具体淡入逻辑
    protected override void OnTransitionStart()
    {
        // 容错：遮罩未赋值时报错
        if (fadeMask == null)
        {
            Debug.LogError($"{gameObject.name}淡入淡出面板未赋值 CanvasGroup 遮罩！");
            return;
        }

        // 显示面板
        Show();
        // 遮罩初始状态：透明、阻挡点击
        fadeMask.alpha = 0;
        fadeMask.blocksRaycasts = true;

        // 具体淡入动画（子类特有逻辑）
        float fadeInTime = transitionDuration * fadeInRatio; // 计算淡入实际时长
        currentTween = fadeMask.DOFade(1, fadeInTime)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 淡入完成  调用基类的“过渡完成”通用方法，触发消息发送
                TriggerTransitionComplete();
            });
    }

    // 实现基类的“过渡清理”方法 → 写具体淡出逻辑
    protected override void OnTransitionCleanup()
    {
        // 具体淡出动画（子类特有逻辑）
        float fadeOutTime = transitionDuration * (1 - fadeInRatio); // 计算淡出实际时长
        currentTween = fadeMask.DOFade(0, fadeOutTime)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 淡出完成  重置状态、隐藏面板
                fadeMask.blocksRaycasts = false;
                Hide(); // 调用最底层UIPanelBase的Hide方法
                targetSceneName = null;
            });
    }

    // 子类特有 显示时重置遮罩
    protected override void OnShow()
    {
        base.OnShow();
        if (fadeMask != null)
        {
            fadeMask.alpha = 0;
        }
    }
}