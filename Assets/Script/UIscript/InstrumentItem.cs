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
    IDamagable
{

    #region Mono

    public Image instrumentIcon;
    public Image instrumentCharacter;
    public Image instrumentBack;
    public Image imgHealthBar;
    public Text txtAttack;
    public Text txtHealth;
    public Image imgName;
    public Image imgAttack;
    public Image imgDeadMask;

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

    [Header("点击按钮")]
    public Button btnClick;
    [Header("角色画布（突出用）")]
    public Canvas iconCanvas;
    [Header("点击角色放大缩放")]
    public float clickBiggerScale;
    [Header("点击角色放大持续时间")]
    public float clickBiggerDuration;
    [Header("点击角色放大后不变时间")]
    public float clickKeepDuration;
    [Header("点击角色缩小持续时间")]
    public float clickSmallerDuration;

    [Header("画布组件")]
    public CanvasGroup canvasGroup;

    [Header("攻击指示线预制体")]
    public GameObject attackLinePrefab;

    #endregion

    private InstrumentInfo _instrumentInfo;
    public InstrumentInfo instrumentInfo => _instrumentInfo;

    private TweenContainer _tweenContainer;
    private int _maxHealth;

    private int _extraAttack;

    private bool _hasInited;
    private bool _hasActioned;
    private bool _hasDead;
    private bool _isPlaying;
    private bool _isScaling;
    protected override void OnShow()
    {

        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);


        _hasInited = true;
        _tweenContainer = new TweenContainer();
        BattleMgr.instance.RegInstrumentItem(this);

        btnClick.onClick.AddListener(OnBtnClicked);
    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);

        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
        BattleMgr.instance.UnregInstrumentItem(this);

        onHideFinished?.Invoke();
        btnClick.onClick.RemoveAllListeners();
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
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBodyBgWithChaPath);
        instrumentBack.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBgPath);
        imgName.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentNamePath);
        imgName.SetNativeSize();
        imgAttack.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentAttackIconPath);
        txtHealth.text = _instrumentInfo.health.ToString() + "/" + _maxHealth.ToString();
        Tween tween =  imgHealthBar.DOFillAmount((float)_instrumentInfo.health / _maxHealth,0.5f);
        _tweenContainer.RegDoTween(tween);
        instrumentCharacter.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBodyPath);
        switch (_instrumentInfo.effectType)
        {
            case EInstrumentEffectType.Attack:
                {
                    if (_extraAttack > 0)
                        txtAttack.text = (_instrumentInfo.attack - _extraAttack).ToString() + "<color=#00FF00>+" + _extraAttack.ToString() + "</color>";
                    else
                        txtAttack.text = _instrumentInfo.attack.ToString();
                }
                break;
            case EInstrumentEffectType.Heal:
                txtAttack.text = _instrumentInfo.heal.ToString();
                break;
            case EInstrumentEffectType.Buff:
                txtAttack.text = _instrumentInfo.buff.ToString();
                break;
            default:
                break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_hasInited || _hasDead || BattleMgr.instance.isPlaying)
            return;
        _isScaling = true;
        Sequence enterSeq = DOTween.Sequence();
        enterSeq.Append(transform.DOScale(enterBiggerScale, enterBiggerDuration));
        enterSeq.Join(transform.DOShakePosition(enterShakeDuration, enterShakeStrength, fadeOut: true))
            .OnComplete(() =>
            {
                _isScaling = false;
                // 强制刷新布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            });
        _tweenContainer.RegDoTween(enterSeq);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_hasInited || _hasDead || BattleMgr.instance.isPlaying)
            return;
        _isScaling = true;

        Sequence exitSeq = DOTween.Sequence();
        exitSeq.Append(transform.DOScale(exitSmallerScale, exitSmallerDuration));
        exitSeq.Join(transform.DOShakePosition(exitShakeDuration, exitShakeStrength, fadeOut: true))
                                    .OnComplete(() =>
                                    {
                                        _isScaling = false;
                                        // 强制刷新布局
                                        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                                    });
        _tweenContainer.RegDoTween(exitSeq);

    }

    public void Attack()
    {
        AudioMgr.Instance.PlaySfx(_instrumentInfo.instrumentAttackSoundPath);
        _hasActioned = true;
        imgDeadMask.gameObject.SetActive(true);
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK);
    }
    public void TakeDamage(int damage)
    {
        if (_hasDead)
            return;
        AudioMgr.Instance.PlaySfx(_instrumentInfo.instrumentHurtSoundPath);
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
    public void TakeHeal(int healAmount)
    {
        instrumentInfo.health = Mathf.Clamp(instrumentInfo.health + healAmount, 0, _maxHealth);
        RefreshShow();
    }

    public int GetHealAmount()
    {
        return instrumentInfo.heal;
    }
    public void TakeBuff(int buffAmount)
    {
        instrumentInfo.attack += buffAmount;
        _extraAttack += buffAmount;
        RefreshShow();

    }
    public int GetBuffAmount()
    {
        return instrumentInfo.buff;
    }
    public void Dead()
    {
        _hasDead = true;
        imgDeadMask.gameObject.SetActive(true);
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_DEAD);
    }
    private void OnBtnClicked()
    {
        if (BattleMgr.instance.isPlaying || _hasActioned || _hasDead || _isScaling)
            return;
        AudioMgr.Instance.PlaySfx(_instrumentInfo.instrumentName);
        BattleMgr.instance.isPlaying = true;
        btnClick.enabled = false;
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK);
        instrumentCharacter.gameObject.SetActive(true);
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBodyBgPath);
        Sequence seq = DOTween.Sequence();
        seq.Append(instrumentCharacter.transform.DOScale(clickBiggerScale, clickBiggerDuration));



        seq.Append(DOVirtual.DelayedCall(clickKeepDuration, () =>
         {
             List<IDamagable> damagableList = new List<IDamagable>();

             switch (instrumentInfo.effectType)
             {
                 case EInstrumentEffectType.Attack:
                     damagableList.Add(BattleMgr.instance.enemyItem);
                     break;
                 case EInstrumentEffectType.Heal:
                     foreach (var item in BattleMgr.instance.instrumentItemList)
                     {
                         damagableList.Add(item);
                     }
                     break;
                 case EInstrumentEffectType.Buff:
                     foreach (var item in BattleMgr.instance.instrumentItemList)
                     {
                         damagableList.Add(item);
                     }
                     break;
                 default:
                     break;
             }
             if (damagableList != null)
                 AttackHandler.DealAttack(instrumentInfo.effectType, this, damagableList);
             MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER);
         }));


        seq.Append(instrumentCharacter.transform.DOScale(Vector3.one, clickSmallerDuration)).OnComplete(() =>
        {
            BattleMgr.instance.isPlaying = false;
            btnClick.enabled = true;
            instrumentCharacter.gameObject.SetActive(false);
            instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBodyBgWithChaPath);
            OnPointerExit(null);
        });

        _tweenContainer.RegDoTween(seq);
    }
    private void OnTurnChg()
    {

        if (_hasDead)
            return;
        if(BattleMgr.instance.curTurn == ETurnType.Player)
        {
            _hasActioned = false;
            imgDeadMask.gameObject.SetActive(false);
        }
    }
}
