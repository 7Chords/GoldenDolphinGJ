using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GoToBattleBtn : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("悬停时目标缩放")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    [Tooltip("按下时目标缩放（相对于 hoverScale 的比例）")]
    [SerializeField] private float pressedScaleMultiplier = 0.95f;
    [Tooltip("缩放过渡时长（秒）")]
    [SerializeField] private float scaleDuration = 0.12f;
    [Tooltip("缩放缓动类型")]
    [SerializeField] private Ease scaleEase = Ease.OutQuad;
    [Tooltip("是否使用 UnscaledTime（忽略 Time.timeScale）")]
    [SerializeField] private bool useUnscaledTime = true;

    private Vector3 originalScale;
    private Vector3 pressedScale;
    private Tween scaleTween;

    private void Awake()
    {
        originalScale = transform.localScale;
        pressedScale = new Vector3(hoverScale.x * pressedScaleMultiplier, hoverScale.y * pressedScaleMultiplier, hoverScale.z * pressedScaleMultiplier);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PlayerMgr.Instance.instrumentIdList.Count == 0)
        {
            Debug.Log("Error当前无角色 ,直接返回");
            return;
        }
        AudioMgr.Instance.PlaySfx("木头按钮");
        SceneLoader.Instance.AddNextScenePanel(EPanelType.BattlePanel);
        TransitionMgr.Instance.StarTransition("BattleScene", "FadeInAndOutTransition");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScale(pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool isPointerOver = (eventData.pointerCurrentRaycast.gameObject == gameObject) || (eventData.pointerEnter == gameObject);
        StartScale(isPointerOver ? hoverScale : originalScale);
    }

    private void StartScale(Vector3 target)
    {
        // 取消现有 tween
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
            scaleTween = null;
        }

        // 直接设置（防止非常短时长导致视觉跳变）
        if (scaleDuration <= 0f)
        {
            transform.localScale = target;
            return;
        }

        // 创建新的 DOTween 缩放并保存引用
        scaleTween = transform.DOScale(target, scaleDuration)
            .SetEase(scaleEase)
            .SetUpdate(useUnscaledTime) // 使用 unscaled 时间以匹配之前的实现
            .OnKill(() => scaleTween = null);
    }

    private void OnDisable()
    {
        // 组件失效时确保 tween 被清理
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill();
            scaleTween = null;
        }
    }
}