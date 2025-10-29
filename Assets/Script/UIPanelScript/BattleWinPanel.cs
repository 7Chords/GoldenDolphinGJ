using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWinPanel : UIPanelBase
{

    public CanvasGroup canvasGroup;

    private TweenContainer _tweenContainer;

    public float fadeInDuration;
    public float fadeOutDuration;
    public Button btnConfirm;
    public Image imgBg;
    public GameObject goNoLastTile;
    public GameObject goLastTile;

    public List<GameObject> goLevelOneUnlockList;
    public List<GameObject> goLevelTwoUnlockList;

    protected override void OnShow()
    {
        canvasGroup.alpha = 0;
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));

        /*        btnConfirm.onClick.AddListener(() =>
                {
                    AudioMgr.Instance.PlayBgm("背景音乐");
                    SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
                    TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
                });*/
        BattleLevelRefObj levelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.level == GameMgr.Instance.curLevel);
        if (levelRefObj != null)
        {
            ResultResRefObj resultRefObj = SCRefDataMgr.Instance.resultResRefList.refDataList.Find(x => x.id == levelRefObj.resultSkinId);
            if (resultRefObj != null)
                imgBg.sprite = Resources.Load<Sprite>(resultRefObj.winBgPath);
        }


        //以下是临时的写法
        if (GameMgr.Instance.curLevel == 3)
        {
            goLastTile.SetActive(true);
            goNoLastTile.SetActive(false);
        }
        else
        {
            goLastTile.SetActive(false);
            goNoLastTile.SetActive(true);
        }

        //之前打过的关卡
        if(GameMgr.Instance.curLevel < GameMgr.Instance.PlayerMaxLevel)
        {
            goLastTile.SetActive(true);
            goNoLastTile.SetActive(false);

            foreach (var go in goLevelTwoUnlockList)
                go.SetActive(false);
            foreach (var go in goLevelOneUnlockList)
                go.SetActive(false);
        }
        else
        {
            if (GameMgr.Instance.curLevel == 2)
            {
                foreach (var go in goLevelTwoUnlockList)
                    go.SetActive(true);
                foreach (var go in goLevelOneUnlockList)
                    go.SetActive(false);
            }
            else if (GameMgr.Instance.curLevel == 1)
            {
                foreach (var go in goLevelTwoUnlockList)
                    go.SetActive(false);
                foreach (var go in goLevelOneUnlockList)
                    go.SetActive(true);
            }
            else
            {
                foreach (var go in goLevelTwoUnlockList)
                    go.SetActive(false);
                foreach (var go in goLevelOneUnlockList)
                    go.SetActive(false);
            }
        }

        GameMgr.Instance.PlayerMaxLevel = Mathf.Clamp(GameMgr.Instance.PlayerMaxLevel + 1, 1, 3);
    }


    protected override void OnHide(Action onHideFinished)
    {
        //btnConfirm.onClick.RemoveAllListeners();

        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
        }));

    }
}
