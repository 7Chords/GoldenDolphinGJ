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


    public Button btnReturn;

    private TweenContainer _tweenContainer;

    public List<LevelItem> levelItemList;

    public LevelDescItem descItem;
    protected override void OnShow()
    {
        //_tweenContainer = new TweenContainer();
        //_tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
        foreach(var item in levelItemList)
        {
            item?.Show();
        }
        btnReturn.onClick.AddListener(() =>
        {
            SceneLoader.Instance.AddNextScenePanel(EPanelType.StartPanel);
            TransitionMgr.Instance.StarTransition("StartScene", "FadeInAndOutTransition");
        });
    }


    protected override void OnHide(Action onHideFinished)
    {
        btnReturn.onClick.RemoveAllListeners();
        foreach (var item in levelItemList)
        {
            item?.Hide();
        }
        descItem?.Hide();
        onHideFinished?.Invoke();
    }

    private void OnDestroy()
    {
    }
}
