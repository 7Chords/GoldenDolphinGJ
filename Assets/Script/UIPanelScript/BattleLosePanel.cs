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
    public Image imgBg;

    protected override void OnShow()
    {
        canvasGroup.alpha = 0;
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));
        BattleLevelRefObj levelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.level == GameMgr.Instance.curLevel);
        if(levelRefObj != null)
        {
            ResultResRefObj resultRefObj = SCRefDataMgr.Instance.resultResRefList.refDataList.Find(x => x.id == levelRefObj.resultSkinId);
            if (resultRefObj != null)
                imgBg.sprite = Resources.Load<Sprite>(resultRefObj.loseBgPath);
        }
    }
    protected override void OnHide(Action onHideFinished)
    {
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
        }));

    }
}
