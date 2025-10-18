using DG.Tweening;
using GJFramework;
using System;
using UnityEngine;

public class BattleWinPanel : UIPanelBase
{

    public CanvasGroup canvasGroup;

    private TweenContainer _tweenContainer;

    public float fadeInDuration;
    public float fadeOutDuration;

    protected override void OnShow()
    {
        canvasGroup.alpha = 0;
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));

    }
    protected override void OnHide(Action onHideFinished)
    {
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
        }));

    }
}
