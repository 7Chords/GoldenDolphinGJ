using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GJFramework
{
    public interface IListItem<TData>
    {
        void SetData(TData data);
        RectTransform GetRectTransform();
    }
    public class LoopScrollList<TData, TItem> : MonoBehaviour
        where TItem : MonoBehaviour, IListItem<TData>
    {
        [Header("滚动组件")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private TItem itemPrefab;

        [Header("布局设置")]
        [SerializeField] private int itemsPerRow = 1;
        [SerializeField] private Vector2 itemSize = new Vector2(100, 100);
        [SerializeField] private Vector2 spacing = new Vector2(10, 10);
        [SerializeField] private Vector2 padding = new Vector2(10, 10);

        private UIGameObjPool<TItem> itemPool;
        private List<TData> dataList = new List<TData>();
        private Dictionary<int, TItem> activeItems = new Dictionary<int, TItem>();

        private float viewportHeight;
        private float itemTotalHeight; // 包含间距的项高度
        private float itemTotalWidth;  // 包含间距的项宽度

        private void Awake()
        {
            // 初始化布局参数
            itemTotalWidth = itemSize.x + spacing.x;
            itemTotalHeight = itemSize.y + spacing.y;
            viewportHeight = viewport.rect.height;

            // 初始化对象池
            itemPool = new UIGameObjPool<TItem>(itemPrefab, content);

            // 监听滚动事件
            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        /// <summary>
        /// 设置列表数据
        /// </summary>
        public void SetData(List<TData> data)
        {
            dataList.Clear();
            dataList.AddRange(data);

            // 回收所有活跃项
            foreach (var item in activeItems.Values)
            {
                itemPool.Release(item);
            }
            activeItems.Clear();

            // 更新内容区域大小
            UpdateContentSize();

            // 初始化显示
            UpdateVisibleItems();
        }

        /// <summary>
        /// 更新内容区域大小
        /// </summary>
        private void UpdateContentSize()
        {
            int totalRows = Mathf.CeilToInt((float)dataList.Count / itemsPerRow);
            float contentHeight = totalRows * itemTotalHeight - spacing.y + padding.y * 2;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
        }

        /// <summary>
        /// 滚动事件处理
        /// </summary>
        private void OnScroll(Vector2 scrollPos)
        {
            UpdateVisibleItems();
        }

        /// <summary>
        /// 更新可视区域的项
        /// </summary>
        private void UpdateVisibleItems()
        {
            if (dataList.Count == 0) return;

            // 计算当前滚动偏移
            float scrollOffset = content.anchoredPosition.y;

            // 计算可视区域的索引范围
            int firstVisibleRow = Mathf.Max(0, Mathf.FloorToInt(scrollOffset / itemTotalHeight));
            int lastVisibleRow = Mathf.Min(
                Mathf.CeilToInt((scrollOffset + viewportHeight) / itemTotalHeight),
                Mathf.CeilToInt((float)dataList.Count / itemsPerRow)
            );

            int firstIndex = firstVisibleRow * itemsPerRow;
            int lastIndex = Mathf.Min((lastVisibleRow + 1) * itemsPerRow - 1, dataList.Count - 1);

            // 回收不在可视范围内的项
            List<int> toRemove = new List<int>();
            foreach (var kvp in activeItems)
            {
                if (kvp.Key < firstIndex || kvp.Key > lastIndex)
                {
                    itemPool.Release(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (int index in toRemove)
            {
                activeItems.Remove(index);
            }

            // 激活可视范围内的项
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (activeItems.ContainsKey(i)) continue;

                TItem item = itemPool.Get();
                activeItems[i] = item;

                // 设置位置
                SetItemPosition(item, i);

                // 设置数据
                item.SetData(dataList[i]);
            }
        }

        /// <summary>
        /// 设置列表项位置
        /// </summary>
        private void SetItemPosition(TItem item, int index)
        {
            int row = index / itemsPerRow;
            int col = index % itemsPerRow;

            float x = padding.x + col * itemTotalWidth + itemSize.x / 2;
            float y = -(padding.y + row * itemTotalHeight + itemSize.y / 2);

            RectTransform rect = item.GetRectTransform();
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = itemSize;
        }

        private void OnDestroy()
        {
            itemPool.Clear();
        }
    }
}
