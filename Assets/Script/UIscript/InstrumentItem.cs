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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_hasDead || _hasActioned || BattleMgr.instance.isPlaying)
            return;
        Debug.Log("BeginDrag");
        _isDragging = true;
        _isDragging = true;

        // 创建拖拽克隆体
        CreateDragClone();

        // 隐藏原物体的交互（使其"看不见"但保持位置）
        if (_originalCanvasGroup != null)
        {
            _originalCanvasGroup.alpha = 0.3f; // 半透明显示原物体
            _originalCanvasGroup.blocksRaycasts = false;
        }

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

        // 恢复原物体的显示和交互
        if (_originalCanvasGroup != null)
        {
            _originalCanvasGroup.alpha = 1f;
            _originalCanvasGroup.blocksRaycasts = true;
        }

        // 检查是否放置在有效区域
        bool isValidDrop = CheckValidDrop(eventData);

        if (isValidDrop)
        {
            // 处理成功的放置
            OnSuccessfulDrop(eventData);
        }
        else
        {
            // 放置无效，可以播放回退动画等
            OnInvalidDrop(eventData);
        }

        // 清理拖拽克隆体
        CleanupDragClone();

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
            _dragClone.transform.localPosition = localPointerPosition - _dragOffset;
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
    private bool CheckValidDrop(PointerEventData eventData)
    {
        // 这里可以添加你的放置验证逻辑
        // 例如检查是否放置在某个特定区域上

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // 检查是否放置在有效的目标上
            // 例如：if (result.gameObject.GetComponent<DropArea>() != null) return true;
        }

        // 暂时返回true，表示任何位置都可以放置
        // 你可以根据实际需求修改这个逻辑
        return true;
    }

    /// <summary>
    /// 成功放置时的处理
    /// </summary>
    private void OnSuccessfulDrop(PointerEventData eventData)
    {
        // 这里可以处理成功放置后的逻辑
        // 例如：更新数据、播放音效、移动原物体到新位置等

        // 发送消息通知拖拽结束和放置成功
        //MsgCenter.SendMsgAct(MsgConst.ON_INSTRUMENT_DRAG_END, this);

        // 可以在这里添加放置成功的视觉效果
        Debug.Log("放置成功！");
    }

    /// <summary>
    /// 无效放置时的处理
    /// </summary>
    private void OnInvalidDrop(PointerEventData eventData)
    {
        // 这里可以处理无效放置的逻辑
        // 例如：播放回退动画、显示提示等

        Debug.Log("放置无效，回到原位置");
    }
}
