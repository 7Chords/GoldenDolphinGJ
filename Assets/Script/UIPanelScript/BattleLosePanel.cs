using DG.Tweening;
using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleLosePanel : UIPanelBase
{

    public CanvasGroup canvasGroup;

    private TweenContainer _tweenContainer;

    public float fadeInDuration;
    public float fadeOutDuration;
    public Button btnConfirm;

    protected override void OnShow()
    {
        canvasGroup.alpha = 0;
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
        btnConfirm.onClick.AddListener(() =>
        {
            SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
            SceneLoader.Instance.LoadScene("LevelSelectScene");
        });
    }
    protected override void OnHide(Action onHideFinished)
    {
        btnConfirm.onClick.RemoveAllListeners();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
        }));

    }
}
