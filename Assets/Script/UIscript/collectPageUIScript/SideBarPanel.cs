using DG.Tweening;
using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBarPanel : UIPanelBase
{
    public Button btnClose;
    public Button btnOpen;

    public Transform itemParentTransform;

    public GameObject sideBarItemPrefab;

    public float fadeInDuration;
    public float fadeOutDuration;

    private TweenContainer _tweenContainer;

    public CanvasGroup canvasGroup;

    private List<GameObject> itemList;

    private bool hasShow;
    protected override void OnShow()
    {
        btnClose.onClick.AddListener(() =>
        {
            Hide();
        });
        btnOpen.gameObject.SetActive(false);
        if(itemList == null)
            itemList = new List<GameObject>();
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));


        SetInfo();
        hasShow = true;
    }

    public void SetInfo()
    {
        foreach(var go in itemList)
        {
            Destroy(go);
        }
        itemList.Clear();
        List<InstrumentRefObj> instrumentRefList = SCRefDataMgr.Instance.instrumentRefList.refDataList;
        for(int i =0;i< instrumentRefList.Count;i++)
        {
            if(instrumentRefList[i].unlockLevelId <= GameMgr.Instance.curLevel)
            {
                GameObject go = GameObject.Instantiate(sideBarItemPrefab);
                go.transform.SetParent(itemParentTransform);
                go.GetComponent<SideBarItem>().SetInfo(instrumentRefList[i]);
                itemList.Add(go);
            }
        }
    }

    protected override void OnHide(Action onHideFinished)
    {
        if (!hasShow)
            return;
        hasShow = false;
        btnClose.onClick.RemoveAllListeners();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            onHideFinished?.Invoke();
            btnOpen.gameObject.SetActive(true);

        }));

    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }
}
