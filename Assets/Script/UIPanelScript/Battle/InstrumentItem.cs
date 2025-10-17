using DG.Tweening;
using GJFramework;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 乐器UIItem
/// </summary>
public class InstrumentItem : UIPanelBase, IPointerEnterHandler, IPointerExitHandler
{
    
    public Image instrumentIcon;
    public Image instrumentBack;
    public Text txtHealth;
    public Text txtAttack;
    public Text txtName;


    [Space(10)]

    [Header("移入放大持续时间")]
    public float enterBiggerDuration;
    [Header("移入放大缩放")]
    public float enterBiggerScale;
    [Header("移入震动强度")]
    public float enterShakeStrength;
    [Header("移入震动持续时间")]
    public float enterShakeDuration;
    [Header("移出缩小持续时间")]
    public float exitSmallerDuration;
    [Header("移出缩小缩放")]
    public float exitSmallerScale;
    [Header("移出震动强度")]
    public float exitShakeStrength;
    [Header("移出震动持续时间")]
    public float exitShakeDuration;

    private InstrumentInfo _instrumentInfo;
    private TweenContainer _tweenContainer;

    private bool _hasInited;



    protected override void OnShow()
    {
        Init();
    }

    protected override void OnHide(Action onHideFinished)
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }
    public void Init()
    {
        _hasInited = true;
        _tweenContainer = new TweenContainer();
    }
    public void SetInfo(InstrumentInfo instrumentInfo)
    {
        _instrumentInfo = instrumentInfo;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_instrumentInfo == null)
            return;
        txtHealth.text = _instrumentInfo.health.ToString();
        txtAttack.text = _instrumentInfo.attack.ToString();
        txtName.text = _instrumentInfo.instrumentName;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_hasInited)
            return;
        Sequence enterSeq = DOTween.Sequence();
        enterSeq.Append(transform.DOScale(enterBiggerScale, enterBiggerDuration));
        enterSeq.Join(transform.DOShakePosition(enterShakeDuration, enterShakeStrength));
        _tweenContainer.RegDoTween(enterSeq);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_hasInited)
            return;
        Sequence exitSeq = DOTween.Sequence();

        exitSeq.Append(transform.DOScale(exitSmallerScale, exitSmallerDuration));
        exitSeq.Join(transform.DOShakePosition(exitShakeDuration, exitShakeStrength));
        _tweenContainer.RegDoTween(exitSeq);

    }
}
