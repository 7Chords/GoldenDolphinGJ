using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelectPanel : UIPanelBase
{
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject levelPrefab;
    public CanvasGroup canvasGroup;

    [Header("淡入时间")]
    public float fadeInDuration;
    [Header("淡出时间")]
    public float fadeOutDuration;

    private TweenContainer _tweenContainer;
    protected override void OnShow()
    {
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));


        List<BattleLevelRefObj> battleLevelRefList = SCRefDataMgr.Instance.battleLevelRefList.refDataList;

        if(battleLevelRefList != null && battleLevelRefList.Count > 0)
        {
            for(int i =0;i<battleLevelRefList.Count;i++)
            {
                GameObject levelGO = GameObject.Instantiate(levelPrefab);
                levelGO.transform.SetParent(horizontalLayoutGroup.transform);
                LevelItem item = levelGO.GetComponent<LevelItem>();
                item.Show();
                item.SetInfo(battleLevelRefList[i]);
            }
        }
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
