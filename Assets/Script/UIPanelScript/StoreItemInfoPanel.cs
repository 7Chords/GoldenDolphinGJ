using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StoreItemInfoPanel : UIPanelBase
{
    [SerializeField] private RectTransform panelRect; // 面板根 RectTransform
    [SerializeField] private StoreItemUIScript contentScript;
    [SerializeField] private Vector2 offset = new Vector2(8f, -50f); // 鼠标偏移
    // CanvasGroup 用于控制 alpha 淡入 避免直接修改 Graphic 的颜色
    private CanvasGroup canvasGroup;
    private Tween fadeTween;

    private void Awake()
    {
        if (panelRect == null) panelRect = GetComponent<RectTransform>();
        if (contentScript == null) contentScript = GetComponentInChildren<StoreItemUIScript>(true);

        // 确保 CanvasGroup 存在
        canvasGroup = panelRect != null ? panelRect.GetComponent<CanvasGroup>() : null;
        if (canvasGroup == null && panelRect != null)
            canvasGroup = panelRect.gameObject.AddComponent<CanvasGroup>();

        // 初始隐藏并确保 alpha 为 0
        if (panelRect != null)
        {
            canvasGroup.alpha = 0f;
            panelRect.gameObject.SetActive(false);
            // 不拦截射线
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    // 设置信息并淡入显示
    public void ShowInfo(long storeItemId, Vector2 screenPos, RectTransform source = null, float fadeDuration = 0.2f)
    {
        if (contentScript != null)
            contentScript.SetInfo(storeItemId);

        if (panelRect == null) return;

        // 激活并置顶
        panelRect.gameObject.SetActive(true);
        panelRect.SetAsLastSibling();

        // 强制刷新布局，确保 ContentSizeFitter / LayoutGroup 完成计算
        Canvas.ForceUpdateCanvases();
        var parentRect = panelRect.parent as RectTransform;
        if (parentRect != null)
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        // 直接按传入的 screenPos 定位
        PositionAt(screenPos, null);

        // 停掉已有淡入
        fadeTween?.Kill();

        if (canvasGroup == null)
        {
            // 如果没有 CanvasGroup，就直接启用
            return;
        }

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = 1f;
            return;
        }

        // 从 0 到 1 淡入
        canvasGroup.alpha = 0f;
        fadeTween = canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
    }

    // 仅隐藏 不做淡出
    public void HideInfo()
    {
        // 取消淡入 Tween
        fadeTween?.Kill();
        fadeTween = null;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panelRect != null)
            panelRect.gameObject.SetActive(false);
    }

    private void PositionAt(Vector2 screenPos, RectTransform source)
    {
        if (panelRect == null) return;

        // 获取 Canvas 与应当传给 RectTransformUtility 的摄像机（Overlay -> null）
        Canvas rootCanvas = panelRect.GetComponentInParent<Canvas>();
        Camera useCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            useCam = rootCanvas.worldCamera != null ? rootCanvas.worldCamera : null;

        // 我们这里固定按鼠标 screenPos 定位，忽略 source（保证始终在鼠标下方的偏移位置）
        // 将屏幕点转换到 panelRect.parent 的本地坐标系（若 parent 为 null 则退回到 canvas）
        RectTransform parentRect = panelRect.parent as RectTransform;
        if (parentRect == null && rootCanvas != null)
            parentRect = rootCanvas.transform as RectTransform;
        if (parentRect == null)
            parentRect = panelRect;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, useCam, out Vector2 localPoint))
            localPoint = Vector2.zero;

        // 确保 panel 尺寸稳定
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        // 直接应用偏移
        Vector2 anchoredPos = localPoint + offset;
        panelRect.anchoredPosition = anchoredPos;

        // clamp 到 parentRect 内
        Rect parentBounds = parentRect.rect;
        Vector2 panelSize = panelRect.rect.size;
        float minX = parentBounds.xMin + panelRect.pivot.x * panelSize.x;
        float maxX = parentBounds.xMax - (1 - panelRect.pivot.x) * panelSize.x;
        float minY = parentBounds.yMin + panelRect.pivot.y * panelSize.y;
        float maxY = parentBounds.yMax - (1 - panelRect.pivot.y) * panelSize.y;

        Vector2 finalPos = panelRect.anchoredPosition;
        finalPos.x = Mathf.Clamp(finalPos.x, minX, maxX);
        finalPos.y = Mathf.Clamp(finalPos.y, minY, maxY);
        panelRect.anchoredPosition = finalPos;
    }

    protected override void OnShow()
    {
        if (panelRect != null)
            panelRect.gameObject.SetActive(true);
    }

    protected override void OnHide(Action onHideFinished)
    {
        if (panelRect != null)
            panelRect.gameObject.SetActive(false);
        onHideFinished?.Invoke();
    }
}