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

    [Header("选择时的缩放")]
    public float hasSelectScale;
    [Header("选择时的缩放时间")]
    public float hasSelectScaleDuration;
    [Header("选择黑色背景")]
    public Image imgBlackBg;
    [Header("选择黑色背景渐变时间")]
    public float selectBlackFadeDuration;
    [Header("渐变材质")]
    public Material fadeMaterial;
    [Header("材质渐变时间")]
    public float materialFadeDuration;
    [Header("材质渐变物体")]
    public GameObject fadeGO;

    [Header("描述渐变画布列表")]
    public List<CanvasGroup> fadeCanvasGroup;
    [Header("单个画布渐变时间")]
    public float singleCanvasGroupFadeDuration;
    [Header("画布间隔时间")]
    public float canvasFadeInterval;
    [Header("开始按钮")]
    public Button btnStart;
    [Header("返回按钮")]
    public Button btnReturn;
    [Header("按钮渐变时间")]
    public float btnFadeDuration;
    [Header("选中后的替换图片")]
    public Sprite selectSprite;
    [Header("未选中后的替换图片")]
    public Sprite unselectSprite;
    [Header("未解锁的图片")]
    public Sprite unlockSprite;
    [Header("专辑图片")]
    public Image imgContent;

    [Header("播放展开动画时专辑的位置")]
    public Transform transformPlayShow;
    [Header("专辑移动速度")]
    public float moveSpeed;

    [Header("专辑所属关卡")]
    public int level;


    private BattleLevelRefObj _battleLevelRefObj;
    private TweenContainer _tweenContainer;

    private bool _hasSelected;
    protected override void OnShow()
    {
        _tweenContainer = new TweenContainer();

        btnSelect.onClick.AddListener(OnSelectBtnClicked);
        btnReturn.onClick.AddListener(CancelSelect);

        if (level > GameMgr.Instance.PlayerMaxLevel)
            imgContent.sprite = unlockSprite;
        else
            imgContent.sprite = unselectSprite;
    }

    protected override void OnHide(Action onHideFinished)
    {
        btnSelect.onClick.RemoveAllListeners();
        btnReturn.onClick.RemoveAllListeners();
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }

    private void OnSelectBtnClicked()
    {
        if (level > GameMgr.Instance.PlayerMaxLevel)
            return;
        GameMgr.Instance.curLevel = level;
        if (_hasSelected)
            return;
        _hasSelected = true;
        gameObject.GetComponent<Canvas>().sortingOrder = 3;


        for (int i = 0; i < fadeCanvasGroup.Count; i++)
        {
            fadeCanvasGroup[i].alpha = 0;
        }
        btnStart.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        btnReturn.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        fadeMaterial.SetFloat("_RevealAmount", 0f);
        fadeGO.SetActive(true);

        BattleLevelRefObj levelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList.Find(x => x.level == level);
        fadeGO.GetComponent<LevelDescItem>().SetInfo(levelRefObj);
        btnReturn.enabled = false;
        btnStart.enabled = false;
        btnStart.GetComponent<GoToCollectPage>().enabled = false;
        transform.GetComponent<Image>().raycastTarget = false;


        float dist = transformPlayShow.position.x - transform.position.x;
        float moveDuration = dist / moveSpeed;
        float targetPosX = transform.parent.position.x + dist;

        Sequence seq = DOTween.Sequence();
        if (Mathf.Abs(moveDuration) > 0.01f)
        {
            seq.Append(transform.parent.DOMoveX(targetPosX, Mathf.Abs(moveDuration)).OnComplete(() =>
            {
                AudioMgr.Instance.PlaySfx("专辑");
                imgContent.sprite = selectSprite;
            }));
        }
        else
        {
            AudioMgr.Instance.PlaySfx("专辑");
            imgContent.sprite = selectSprite;
        }

        
        _tweenContainer.RegDoTween(transform.DOScale(Vector3.one * hasSelectScale, hasSelectScaleDuration));
        _tweenContainer.RegDoTween(imgBlackBg.DOFade(1, selectBlackFadeDuration));

        seq.Append(fadeMaterial.DOFloat(1f, "_RevealAmount", materialFadeDuration));

        for(int i =0;i< fadeCanvasGroup.Count;i++)
        {
            seq.Append(fadeCanvasGroup[i].DOFade(1f, singleCanvasGroupFadeDuration));
        }
        seq.Append(btnStart.GetComponent<Image>().DOFade(1, btnFadeDuration)).OnComplete(() =>
        {
            btnReturn.enabled = true;
            btnStart.enabled = true;
            btnStart.GetComponent<GoToCollectPage>().enabled = true;
        });
        seq.Join(btnReturn.GetComponent<Image>().DOFade(1, btnFadeDuration));

        _tweenContainer.RegDoTween(seq);
    }

    public void CancelSelect()
    {
        if (!_hasSelected)
            return;
        btnReturn.enabled = false;
        btnStart.enabled = false;
        btnStart.GetComponent<GoToCollectPage>().enabled = false;

        AudioMgr.Instance.PlaySfx("木头按钮");

        _tweenContainer.RegDoTween(imgBlackBg.DOFade(0, selectBlackFadeDuration));

        Sequence seq = DOTween.Sequence();
        seq.Append(btnStart.GetComponent<Image>().DOFade(0, btnFadeDuration));
        seq.Join(btnReturn.GetComponent<Image>().DOFade(0, btnFadeDuration));
        for(int i =0;i< fadeCanvasGroup.Count;i++)
        {
            if(i == 0)
                seq.Append(fadeCanvasGroup[i].DOFade(0, singleCanvasGroupFadeDuration));
            else
                seq.Join(fadeCanvasGroup[i].DOFade(0, singleCanvasGroupFadeDuration));
        }
        seq.Append(fadeMaterial.DOFloat(0f, "_RevealAmount", materialFadeDuration));

        seq.Append(transform.DOScale(Vector3.one, hasSelectScaleDuration).OnComplete(() =>
        {
            _hasSelected = false;
            imgContent.sprite = unselectSprite;
            fadeGO.SetActive(false);
            gameObject.GetComponent<Canvas>().sortingOrder = 2;
            transform.GetComponent<Image>().raycastTarget = true;
        }));

        _tweenContainer.RegDoTween(seq);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hasSelected)
            return;
        _tweenContainer.RegDoTween(transform.DOScale(Vector3.one * selectBiggerScale, selectBiggerDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hasSelected)
            return;
        _tweenContainer.RegDoTween(transform.DOScale(Vector3.one, unselecSmallerDuration));
    }
}
