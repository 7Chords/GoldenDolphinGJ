using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelectPanel : UIPanelBase
{ 

    public CanvasGroup canvasGroup;

    [Header("淡入时间")]
    public float fadeInDuration;
    [Header("淡出时间")]
    public float fadeOutDuration;

    private TweenContainer _tweenContainer;

    public LevelItem levelItem;
    protected override void OnShow()
    {
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
        levelItem?.Show();
    }


    protected override void OnHide(Action onHideFinished)
    {
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
        }));
    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }
}
