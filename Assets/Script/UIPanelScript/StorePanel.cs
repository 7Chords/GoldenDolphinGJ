using System.Collections.Generic;
using GJFramework;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : UIPanelBase
{
    // 可在 Inspector 绑定容器（Panel 下的子物体），若未绑定会在 OnShow 时自动查找
    [SerializeField] private Container container;

    // 可在 Inspector 绑定 ScrollRect（推荐），若未绑定会自动查找
    [SerializeField] private ScrollRect scrollRect;

    protected override void OnShow()
    {
        Debug.Log($"{this.name} is Show!");

        // 兜底查找 Container（如果在预制体层级中把 Container 放在 Panel 下）
        if (container == null)
            container = GetComponentInChildren<Container>();

        if (container == null)
        {
            Debug.LogWarning("StorePanel: 未找到 Container，无法显示商店物品。");
            return;
        }

        // 尝试查找并自动配置 ScrollRect 的 content（仅在需要时）
        EnsureScrollSetup();

        // 示例默认行为：根据 Container Inspector 中的 Prefab 列表实例化所有预制体
        // 若你需要按数据动态填充，请使用 PopulateFromPrefabs 或 AddItem 方法
        container.InstantiateAllPrefabs();

        // 实例化后强制刷新布局（确保 ScrollRect / ContentSizeFitter 正确生效）
        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }
    }

    protected override void OnHide()
    {
        Debug.Log($"{this.name} is Hide!");

        // 默认隐藏时清空容器（按需可移除以保留缓存）
        if (container != null)
            container.ClearItems();
    }

    // Ensure ScrollRect.content 指向 Container，并保证 Content 有 ContentSizeFitter（PreferredSize）
    private void EnsureScrollSetup()
    {
        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>();

        if (scrollRect == null)
            return;

        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt == null)
            return;

        // 警告：如果你把 Container 设为 ScrollRect 的 Viewport，这是不对的
        if (scrollRect.viewport == contentRt)
        {
            Debug.LogWarning("ScrollRect.viewport 与 Content 相同。请在层级中创建一个 Viewport（带 RectMask2D 或 Mask）并把 ScrollRect.Viewport 指向它，Content 指向 Container。");
        }

        // 自动绑定 content（如果未绑定）
        if (scrollRect.content == null)
        {
            scrollRect.content = contentRt;
        }

        // 确保 content 上有 ContentSizeFitter（便于 Grid 自动扩展内容大小以支持滚动）
        var csf = contentRt.GetComponent<ContentSizeFitter>();
        if (csf == null)
        {
            csf = contentRt.gameObject.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    // 运行时：使用一组预制体填充商店（会先 Clear 再添加）
    public void PopulateFromPrefabs(List<GameObject> prefabs)
    {
        if (container == null) container = GetComponentInChildren<Container>();
        if (container == null) return;

        container.ClearItems();
        foreach (var p in prefabs)
        {
            if (p != null)
                container.AddItemFromPrefab(p);
        }

        // 刷新布局
        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }
    }

    // 运行时：添加单个商品项
    public GameObject AddItem(GameObject prefab)
    {
        if (container == null) container = GetComponentInChildren<Container>();
        if (container == null) return null;

        var go = container.AddItemFromPrefab(prefab);

        // 刷新布局
        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }

        return go;
    }

    // 运行时：移除单个实例
    public bool RemoveItem(GameObject instance)
    {
        if (container == null) container = GetComponentInChildren<Container>();
        if (container == null) return false;

        var result = container.RemoveItem(instance);

        // 刷新布局
        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }

        return result;
    }

    // 运行时：清空商店
    public void ClearStore()
    {
        if (container == null) container = GetComponentInChildren<Container>();
        if (container == null) return;

        container.ClearItems();

        var contentRt = container.GetComponent<RectTransform>();
        if (contentRt != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }
    }
}