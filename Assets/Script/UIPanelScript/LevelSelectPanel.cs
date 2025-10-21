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

    public List<LevelItem> levelItemList;
    protected override void OnShow()
    {
        //_tweenContainer = new TweenContainer();
        //_tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
        foreach(var item in levelItemList)
        {
            item?.Show();
        }
    }


    protected override void OnHide(Action onHideFinished)
    {
        //_tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        //{
        //    onHideFinished?.Invoke();
        //    _tweenContainer?.KillAllDoTween();
        //    _tweenContainer = null;
        //}));
        foreach (var item in levelItemList)
        {
            item?.Hide();
        }
        onHideFinished?.Invoke();
    }

    private void OnDestroy()
    {
    }
}
