using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 乐器UIItem
/// </summary>
public class InstrumentItem : UIPanelBase, 
    IPointerEnterHandler, 
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDamagable
{

    #region Mono

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

    [Header("已经行动过时的画布Alpha")]
    public float alreadyActionAlpha;
    [Header("受伤震动强度")]
    public float hurtShakeStrength;
    [Header("受伤震动时间")]
    public float hurtShakeDuration;
    [Header("受伤颜色")]
    public Color hurtColor;
    [Header("受伤颜色变化时间")]
    public float hurtColorFadeDuration;

    [Header("画布组件")]
    public CanvasGroup canvasGroup;

    [Header("攻击指示线预制体")]
    public GameObject attackLinePrefab;

    #endregion

    private InstrumentInfo _instrumentInfo;
    public InstrumentInfo instrumentInfo => _instrumentInfo;

    private TweenContainer _tweenContainer;

    private GameObject _attackLineGO;
    private LineRenderer _lineRenderer;
    private int _maxHealth;


    private bool _hasInited;
    private bool _hasActioned;
    private bool _hasDead;
    protected override void OnShow()
    {

        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);


        _hasInited = true;
        _tweenContainer = new TweenContainer();
        BattleMgr.instance.RegInstrumentItem(this);
    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);

        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
        BattleMgr.instance.UnregInstrumentItem(this);

        onHideFinished?.Invoke();
    }
    public void SetInfo(InstrumentInfo instrumentInfo)
    {
        _instrumentInfo = instrumentInfo;
        _maxHealth = _instrumentInfo.health;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_instrumentInfo == null)
            return;
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentIconPath);
        instrumentBack.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBgPath);
        txtHealth.text = _instrumentInfo.health.ToString();
        txtAttack.text = _instrumentInfo.attack.ToString();
        txtName.text = _instrumentInfo.instrumentName;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_hasInited && _hasDead)
            return;

        Sequence enterSeq = DOTween.Sequence();
        enterSeq.Append(transform.DOScale(enterBiggerScale, enterBiggerDuration));
        enterSeq.Join(transform.DOShakePosition(enterShakeDuration, enterShakeStrength, fadeOut: true))
            .OnComplete(() =>
            {
                // 强制刷新布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            });
        _tweenContainer.RegDoTween(enterSeq);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_hasInited && _hasDead)
            return;

        Sequence exitSeq = DOTween.Sequence();
        exitSeq.Append(transform.DOScale(exitSmallerScale, exitSmallerDuration));
        exitSeq.Join(transform.DOShakePosition(exitShakeDuration, exitShakeStrength, fadeOut: true))
                                    .OnComplete(() =>
                                    {
                                        // 强制刷新布局
                                        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                                    });
        _tweenContainer.RegDoTween(exitSeq);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_hasInited && _hasDead)
            return;
        if (_hasActioned)
            return;
        // 创建攻击指示线
        _attackLineGO = Instantiate(attackLinePrefab);
        _lineRenderer = _attackLineGO.GetComponent<LineRenderer>();

        // 设置初始位置
        UpdateLinePositions(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_hasInited && _hasDead)
            return;
        if (_hasActioned)
            return;
        if (_lineRenderer != null)
        {
            UpdateLinePositions(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_hasInited && _hasDead)
            return;
        if (_hasActioned)
            return;
        // 这里可以添加释放后的逻辑，比如检测释放目标等

        GameObject enemyIconGO = eventData.hovered?.Find(x => x.tag == "EnemyIcon");
        if(enemyIconGO != null)
        {
            IDamagable targetDamagable = enemyIconGO.transform.parent.GetComponent<IDamagable>();
            if (targetDamagable != null)
                AttackHandler.DealAttack(this, new List<IDamagable>() { targetDamagable });
        }
        // 销毁攻击指示线
        if (_attackLineGO != null)
        {
            Destroy(_attackLineGO);
            _attackLineGO = null;
            _lineRenderer = null;
        }
    }

    /// <summary>
    /// 更新攻击指示线的起点和终点位置
    /// </summary>
    private void UpdateLinePositions(PointerEventData eventData)
    {
        if (_lineRenderer == null) return;

        // 获取UI元素的世界位置（起点）
        Vector3 startWorldPos = GetUIPositionWorldPos(transform.position);
        Vector3 endWorldPos;

        endWorldPos = GetMouseWorldPosition(eventData.position);

        // 设置LineRenderer的位置
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startWorldPos);
        _lineRenderer.SetPosition(1, endWorldPos);
    }

    #region Util
    /// <summary>
    /// 获取UI位置对应的世界坐标
    /// </summary>
    private Vector3 GetUIPositionWorldPos(Vector3 uiPosition)
    {
        Vector3 worldPos = Vector3.zero;
        worldPos = Camera.main.ScreenToWorldPoint(uiPosition);
        return worldPos;
    }

    /// <summary>
    /// 获取鼠标位置对应的世界坐标
    /// </summary>
    private Vector3 GetMouseWorldPosition(Vector3 mousePos)
    {

        Vector3 mouseScreenPos = mousePos;

        return Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));
    }

    #endregion
    public void Attack()
    {
        _hasActioned = true;
        canvasGroup.alpha = alreadyActionAlpha;
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER);
    }

    public void TakeDamage(int damage)
    {
        instrumentInfo.health = Mathf.Clamp(instrumentInfo.health - damage, 0, _maxHealth);
        RefreshShow();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(hurtShakeDuration, hurtShakeStrength, fadeOut: true));
        seq.Join(instrumentIcon.DOColor(hurtColor, hurtColorFadeDuration / 2));
        seq.Append(instrumentIcon.DOColor(Color.white, hurtColorFadeDuration / 2));
        _tweenContainer.RegDoTween(seq);

        if (instrumentInfo.health == 0)
            Dead();
    }

    public int GetAttackAmount()
    {
        return instrumentInfo.attack;
    }
    public void Dead()
    {
        _hasDead = true;
    }







    private void OnTurnChg()
    {
        if(BattleMgr.instance.curTurn == ETurnType.Player)
        {
            _hasActioned = false;
            canvasGroup.alpha = 1;
        }
    }

}
