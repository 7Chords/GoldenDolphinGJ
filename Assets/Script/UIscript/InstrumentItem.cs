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
    IDamagable,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
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


    #endregion

    private InstrumentInfo _instrumentInfo;
    public InstrumentInfo instrumentInfo => _instrumentInfo;

    private TweenContainer _tweenContainer;

    private int _maxHealth;
    private int _extraAttack;
    private int _maxSkillPoint;


    private bool _hasInited;
    private bool _hasActioned;
    private bool _hasDead;
    private bool _isScaling;


    // 拖拽相关变量
    private GameObject _dragClone;
    private CanvasGroup _originalCanvasGroup;
    private bool _isDragging = false;
    private Canvas _parentCanvas;
    private Vector2 _dragOffset;
    protected override void OnShow()
    {

        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);


        _hasInited = true;
        _tweenContainer = new TweenContainer();
        BattleMgr.instance.RegInstrumentItem(this);

        btnClick.onClick.AddListener(OnBtnClicked);

        // 获取父级Canvas
        _parentCanvas = GetComponentInParent<Canvas>();
        _originalCanvasGroup = canvasGroup;
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
        _maxSkillPoint = _instrumentInfo.refObj.canUseSkillPoint;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_instrumentInfo == null)
            return;
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentBodyBgWithChaPath);
        instrumentBack.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentBgPath);
        imgName.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentNamePath);
        imgName.SetNativeSize();
        imgAttack.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentAttackIconPath);
        txtHealth.text = _instrumentInfo.health.ToString() + "/" + _maxHealth.ToString();
        Tween tween =  imgHealthBar.DOFillAmount((float)_instrumentInfo.health / _maxHealth,0.5f);
        _tweenContainer.RegDoTween(tween);
        instrumentCharacter.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentBodyPath);
        switch (_instrumentInfo.refObj.effectType)
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


        _instrumentInfo.skillPoint = Mathf.Min(_instrumentInfo.skillPoint + 1, _maxSkillPoint);
        AudioMgr.Instance.PlaySfx(_instrumentInfo.refObj.instrumentAttackSoundPath);
        _hasActioned = true;
        imgDeadMask.gameObject.SetActive(true);
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK);
    }
    public void TakeDamage(int damage)
    {
        if (_hasDead)
            return;
        AudioMgr.Instance.PlaySfx(_instrumentInfo.refObj.instrumentHurtSoundPath);
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
        AudioMgr.Instance.PlaySfx(_instrumentInfo.refObj.instrumentName);
        BattleMgr.instance.isPlaying = true;
        btnClick.enabled = false;
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK);
        instrumentCharacter.gameObject.SetActive(true);
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentBodyBgPath);
        Sequence seq = DOTween.Sequence();
        seq.Append(instrumentCharacter.transform.DOScale(clickBiggerScale, clickBiggerDuration));



        seq.Append(DOVirtual.DelayedCall(clickKeepDuration, () =>
         {
             List<IDamagable> damagableList = new List<IDamagable>();

             switch (instrumentInfo.refObj.effectType)
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
                 AttackHandler.DealAttack(instrumentInfo.refObj.effectType, this, damagableList);
             MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER);
         }));


        seq.Append(instrumentCharacter.transform.DOScale(Vector3.one, clickSmallerDuration)).OnComplete(() =>
        {
            BattleMgr.instance.isPlaying = false;
            btnClick.enabled = true;
            instrumentCharacter.gameObject.SetActive(false);
            instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.refObj.instrumentBodyBgWithChaPath);
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_hasDead || _hasActioned || BattleMgr.instance.isPlaying)
            return;
        Debug.Log("BeginDrag");
        _isDragging = true;

        // 创建拖拽克隆体
        CreateDragClone();

        // 隐藏原物体的交互（使其"看不见"但保持位置）
        if (_originalCanvasGroup != null)
        {
            _originalCanvasGroup.alpha = 0f;
            _originalCanvasGroup.blocksRaycasts = false;
        }
        //关闭射线交互
        instrumentBack.raycastTarget = false;


        // 保存点击位置与物体中心的偏移量
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            eventData.position,
            _parentCanvas.worldCamera,
            out _dragOffset);

        // 更新克隆体位置
        UpdateDragClonePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _dragClone == null) return;

        // 更新克隆体位置
        UpdateDragClonePosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        _isDragging = false;

        // 检查是否放置在有效区域
        InstrumentItem item = CheckIsValidInstrument(eventData);

        if (item != null)
            OnSuccessfulUseTogetherSkill(item);
        else
            OnInvalidDrop();

        // 强制刷新布局（确保原物体位置正确）
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 创建拖拽克隆体
    /// </summary>
    private void CreateDragClone()
    {
        if (_dragClone != null) return;

        // 创建克隆体
        _dragClone = Instantiate(gameObject, _parentCanvas.transform);
        _dragClone.GetComponent<RectTransform>().sizeDelta = new Vector2
            (gameObject.GetComponent<RectTransform>().rect.width,
            gameObject.GetComponent<RectTransform>().rect.height);


        // 移除克隆体上不需要的组件
        var cloneInstrumentItem = _dragClone.GetComponent<InstrumentItem>();
        if (cloneInstrumentItem != null)
            Destroy(cloneInstrumentItem);

        var cloneBtn = _dragClone.GetComponent<Button>();
        if (cloneBtn != null)
            Destroy(cloneBtn);

        var cloneDragHandlers = _dragClone.GetComponents<IBeginDragHandler>();
        foreach (var handler in cloneDragHandlers)
        {
            if (handler is MonoBehaviour behaviour)
                Destroy(behaviour);
        }

        // 设置克隆体的属性
        var cloneCanvasGroup = _dragClone.GetComponent<CanvasGroup>();
        if (cloneCanvasGroup == null)
            cloneCanvasGroup = _dragClone.AddComponent<CanvasGroup>();

        cloneCanvasGroup.alpha = 0.8f;
        cloneCanvasGroup.blocksRaycasts = false;

        // 设置克隆体的层级，确保显示在最前面
        _dragClone.transform.SetAsLastSibling();

        // 应用当前缩放（包括悬停效果产生的缩放）
        _dragClone.transform.localScale = transform.localScale;
    }

    /// <summary>
    /// 更新拖拽克隆体位置
    /// </summary>
    private void UpdateDragClonePosition(PointerEventData eventData)
    {
        if (_dragClone == null) return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            eventData.position,
            _parentCanvas.worldCamera,
            out localPointerPosition))
        {
            _dragClone.transform.localPosition = localPointerPosition;
        }
    }

    /// <summary>
    /// 清理拖拽克隆体
    /// </summary>
    private void CleanupDragClone()
    {
        if (_dragClone != null)
        {
            Destroy(_dragClone);
            _dragClone = null;
        }
    }

    /// <summary>
    /// 检查是否放置在有效区域
    /// </summary>
    private InstrumentItem CheckIsValidInstrument(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameObject go = results.Find(x => x.gameObject.GetComponent<InstrumentItem>() != null).gameObject;
        if (go == null)
            return null;
        InstrumentItem item = go.GetComponent<InstrumentItem>();
        if (item != null)
        {
            if (canUseTogetherSkill(item.instrumentInfo.refObj.id))
                return item;
            return null;
        }
        return null;
    }

    /// <summary>
    /// 成功放置时的处理
    /// </summary>
    private void OnSuccessfulUseTogetherSkill(InstrumentItem item)
    {

        EnterTogetherSkill();
        item.EnterTogetherSkill();


        Sequence seq = DOTween.Sequence();
        seq.Append(_dragClone.GetComponent<CanvasGroup>().DOFade(0, 0.25f).OnComplete(()=> 
        {
            CleanupDragClone();
        }));
        seq.Join(_originalCanvasGroup.DOFade(1, 0.25f));

        seq.Append(transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));

        seq.Join(item.transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));

        seq.AppendInterval(2f).OnComplete(() =>
        {
            ExitTogetherSkill();
            item.ExitTogetherSkill();
        });

        

        Debug.Log("合击成功！");
    }

    /// <summary>
    /// 无效放置时的处理
    /// </summary>
    private void OnInvalidDrop()
    {
        CleanupDragClone();
        // 恢复原物体的显示和交互
        if (_originalCanvasGroup != null)
        {
            _originalCanvasGroup.alpha = 1f;
            _originalCanvasGroup.blocksRaycasts = true;
        }
    }

    public bool canUseTogetherSkill(long anotherId)
    {
        bool flag = false;
        for(int i =0;i<_instrumentInfo.skillRefList.Count;i++)
        {
            if (_instrumentInfo.skillRefList[i].skillUserList.Contains(anotherId))
            {
                flag = true;
                break;
            }
        }
        return _instrumentInfo.refObj.hasTogetherSkill
            && _instrumentInfo.skillPoint == _maxSkillPoint
            && flag;
    }


    public void EnterTogetherSkill()
    {
        _hasActioned = true;
        instrumentBack.raycastTarget = false;
        _originalCanvasGroup.blocksRaycasts = false;
    }

    public void ExitTogetherSkill()
    {
        _instrumentInfo.skillPoint = 0;
        MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER);
        instrumentBack.raycastTarget = true;
        _originalCanvasGroup.blocksRaycasts = true;
        imgDeadMask.gameObject.SetActive(true);
    }
}
