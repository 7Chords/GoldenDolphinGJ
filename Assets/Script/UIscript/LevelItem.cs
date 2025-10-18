using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelItem : UIPanelBase,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image imgContent;
    public GameObject levelDescGO;
    public Text txtEnemyInfo;
    public Text txtRecommond;
    public Button btnSelect;
    public CanvasGroup canvasGroup;
    [Header("淡入时间")]
    public float fadeInDuration;
    [Header("淡出时间")]
    public float fadeOutDuration;

    [Header("选中放大scale")]
    public float selectBiggerScale;
    [Header("选中放大时间")]
    public float selectBiggerDuration;

    [Header("未选中缩小时间")]
    public float unselecSmallerDuration;

    [Header("详情面板淡入时间")]
    public float descFadeInDuration;
    [Header("详情面板淡出时间")]
    public float descFadeOutDuration;

    private BattleLevelRefObj _battleLevelRefObj;
    private TweenContainer _tweenContainer;


    private bool _isScaleChging;
    protected override void OnShow()
    {
        _tweenContainer = new TweenContainer();
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration));

        levelDescGO.SetActive(false);

        btnSelect.onClick.AddListener(() =>
        {
            OpenOrCloseDescGO();
        });
    }

    protected override void OnHide(Action onHideFinished)
    {
        btnSelect.onClick.RemoveAllListeners();

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

    public void SetInfo(BattleLevelRefObj levelRefObj)
    {
        _battleLevelRefObj = levelRefObj;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_battleLevelRefObj == null)
            return;
        List<InstrumentRefObj> instrumentRefList = new List<InstrumentRefObj>();

        foreach(var id in _battleLevelRefObj.recommendinstrumentsIdList)
        {
            InstrumentRefObj refObj = SCRefDataMgr.Instance.instrumentRefList.refDataList.Find(x => x.id == id);
            if (refObj == null)
                continue;
            instrumentRefList.Add(refObj);
        }
        string str = "";
        for(int i =0;i< instrumentRefList.Count;i++)
        {
            str += instrumentRefList[i].instrumentName;
            if (i < instrumentRefList.Count - 1)
                str += ",";
        }
        txtRecommond.text = str;
    }

    private void OpenOrCloseDescGO()
    {
        levelDescGO.SetActive(!levelDescGO.activeInHierarchy);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isScaleChging = true;
        _tweenContainer.RegDoTween(transform.DOScale(Vector3.one * selectBiggerScale, selectBiggerDuration).OnComplete
            (() =>
            {
                _isScaleChging = false;

            }));

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isScaleChging = true;
        _tweenContainer.RegDoTween(transform.DOScale(Vector3.one, unselecSmallerDuration).OnComplete
            (() =>
            {
                _isScaleChging = false;
            }));
    }
}
