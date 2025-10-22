using DG.Tweening;
using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIPanelBase
{
    public Slider sldBgm;
    public Slider sldSfx;
    public Button btnClose;

    public CanvasGroup canvasGroup;
    public float fadeInDuration;
    public float fadeOutDuration;

    private TweenContainer _tweenContainer;

    protected override void OnShow()
    {
        _tweenContainer = new TweenContainer();
        canvasGroup.alpha = 0;
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
    }

    protected override void OnHide(Action onHideFinished)
    {
        canvasGroup.alpha = 1;
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration)
            .OnComplete(() =>
            {
                base.OnHide(onHideFinished);
            }));


    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }



}
