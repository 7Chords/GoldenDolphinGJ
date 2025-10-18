using DG.Tweening;
using GJFramework;
using System;
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

    [Header("画布组件")]
    public CanvasGroup canvasGroup;
    //[Header("选中时的淡出时间")]
    //public float selectFadeOutDuration;
    //[Header("取消选中时的淡入时间")]
    //public float unSelectFadeInDuration;

    [Header("攻击指示线预制体")]
    public GameObject attackLinePrefab;

    #endregion

    private InstrumentInfo _instrumentInfo;
    public InstrumentInfo instrumentInfo => _instrumentInfo;

    private TweenContainer _tweenContainer;

    private bool _hasInited;

    private GameObject _attackLineGO;
    private LineRenderer _lineRenderer;

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
        instrumentIcon.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentIconPath);
        instrumentBack.sprite = Resources.Load<Sprite>(_instrumentInfo.instrumentBgPath);
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
        enterSeq.Join(transform.DOShakePosition(enterShakeDuration, enterShakeStrength, fadeOut: true));
        _tweenContainer.RegDoTween(enterSeq);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_hasInited)
            return;
        Sequence exitSeq = DOTween.Sequence();

        exitSeq.Append(transform.DOScale(exitSmallerScale, exitSmallerDuration));
        exitSeq.Join(transform.DOShakePosition(exitShakeDuration, exitShakeStrength, fadeOut: true));
        _tweenContainer.RegDoTween(exitSeq);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 创建攻击指示线
        _attackLineGO = Instantiate(attackLinePrefab);
        _lineRenderer = _attackLineGO.GetComponent<LineRenderer>();

        // 设置初始位置
        UpdateLinePositions(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_lineRenderer != null)
        {
            UpdateLinePositions(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 这里可以添加释放后的逻辑，比如检测释放目标等

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



        // 获取鼠标位置对应的世界位置（终点）
        Vector3 endWorldPos = GetMouseWorldPosition(eventData);

        // 设置LineRenderer的位置
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startWorldPos);
        _lineRenderer.SetPosition(1, endWorldPos);
    }

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
    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {

        Vector3 mouseScreenPos = eventData.position;

        return Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));
    }

    public void Attack()
    {
    }

    public void TakeDamage(int damage)
    {

    }
}
