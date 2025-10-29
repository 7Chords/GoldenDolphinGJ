using GJFramework;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ColloctFinishPanel : UIPanelBase
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;
    [SerializeField] private Image collectFinishImage;
    [SerializeField] private Sprite[] finishSprites;
    private CanvasGroup canvasGroup;
    private Tween currentTween;

    private void Awake()
    {
        // 确保有 CanvasGroup，用于控制整个面板的透明度与交互
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    protected override void OnShow()
    {
        SetFinishImage();
        // 终止任何残留 Tween，开始淡入
        currentTween?.Kill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        currentTween = canvasGroup.DOFade(1f, fadeDuration)
            .SetEase(fadeEase)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                currentTween = null;
            });
        AudioMgr.Instance.PlaySfx("鼓掌 胜利音cut");
        AudioMgr.Instance.ResumeBgm();

    }

    protected override void OnHide(Action onHideFinished)
    {
        // 终止任何残留 Tween，开始淡出，完成后回调
        currentTween?.Kill();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        currentTween = canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(fadeEase)
            .OnComplete(() =>
            {
                currentTween = null;
                onHideFinished?.Invoke();
            });
    }
    public void SetFinishImage()
    {
        collectFinishImage.sprite = finishSprites[GameMgr.Instance.curLevel - 1];
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}