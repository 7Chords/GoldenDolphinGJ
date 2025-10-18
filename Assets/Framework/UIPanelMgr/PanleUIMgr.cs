using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



namespace GJFramework
{
    // 面板类型枚举
    public enum EPanelType
    {
        SubPanel,//面板中的子面板统一设置
        StorePanel,
        BagPanel,
        BattlePanel,
        NoteCollectPanel,
        BattleWinPanel,
        LevelSelectPanel,
        BossInfoPanel,
        ColloctFinishPanel,
        BattleLosePanel,
        StartPanel,
        // 其他面板类型
    }

    // 全屏面板UI管理器
    public class PanelUIMgr : Singleton<PanelUIMgr>
    {
        // 面板根节点（所有面板都作为其子对象）
        public Transform panelRoot;

        // 面板栈 - 存储所有打开的面板
        private Stack<UIPanelBase> _panelStack = new Stack<UIPanelBase>();

        // 缓存所有已加载的面板（包括隐藏的），实现复用
        private Dictionary<EPanelType, UIPanelBase> _cachedPanels = new Dictionary<EPanelType, UIPanelBase>();

        // 面板预制体路径（Resources文件夹下的路径）
        private const string PANEL_RESOURCE_PATH = "UI/PanelsPrefabs/";

        public PanelUIMgr()
        {
            EnsurePanelRootExists();
        }

        /// <summary>
        /// 确保面板根节点和 EventSystem 存在
        /// </summary>
        private void EnsurePanelRootExists()
        {
            GameObject rootObj = GameObject.Find("UIRoot");
            if (rootObj == null)
            {
                rootObj = new GameObject("UIRoot");
            }

            // 确保必须组件存在
            var canvas = rootObj.GetComponent<Canvas>();
            if (canvas == null) canvas = rootObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (rootObj.GetComponent<CanvasScaler>() == null) rootObj.AddComponent<CanvasScaler>();
            if (rootObj.GetComponent<GraphicRaycaster>() == null) rootObj.AddComponent<GraphicRaycaster>();

            panelRoot = rootObj.transform;

            // 确保全局 EventSystem 存在（切换场景后仍可交互）
            if (EventSystem.current == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.transform.SetParent(panelRoot, false);
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// 打开面板（优先从缓存中获取，不存在则加载）
        /// </summary>
        public bool OpenPanel(EPanelType panelType)
        {
            // 1. 先检查缓存中是否已有该面板
            if (_cachedPanels.TryGetValue(panelType, out UIPanelBase panel))
            {
                // 缓存中存在，直接复用
                // 如果已经在栈中，先移除（避免重复入栈）
                if (_panelStack.Contains(panel))
                {
                    RemoveFromStack(panel);
                }
            }
            else
            {
                // 2. 缓存中没有，从资源加载并创建
                panel = LoadPanelFromResources(panelType);
                if (panel == null)
                {
                    Debug.LogError($"无法加载面板 {panelType}");
                    return false;
                }
                // 加入缓存
                _cachedPanels.Add(panelType, panel);
            }

            // 将新面板入栈并显示
            _panelStack.Push(panel);
            panel.Show();

            // 确保新面板在最上层显示
            panel.transform.SetAsLastSibling();

            return true;
        }

        /// <summary>
        /// 关闭指定面板
        /// </summary>
        public bool ClosePanel(EPanelType panelType)
        {
            if (!_cachedPanels.TryGetValue(panelType, out UIPanelBase panel))
            {
                Debug.LogWarning($"面板 {panelType} 未加载");
                return false;
            }

            if (!_panelStack.Contains(panel))
            {
                Debug.LogWarning($"面板 {panelType} 不在当前栈中");
                return false;
            }

            // 从栈中移除
            RemoveFromStack(panel);
            // 隐藏面板（不销毁，保留在缓存中）
            panel.Hide();

            // 显示新栈顶
            if (_panelStack.Count > 0)
            {
                UIPanelBase newTop = _panelStack.Peek();
                newTop.Show();
                newTop.transform.SetAsLastSibling(); // 确保显示在最上层
            }

            return true;
        }

        /// <summary>
        /// 返回上一个面板（关闭当前栈顶）
        /// </summary>
        public bool GoBack()
        {
            if (_panelStack.Count <= 1)
            {
                Debug.LogWarning("已经是最底层面板，无法返回");
                return false;
            }

            var currentTop = _panelStack.Peek();
            return ClosePanel(currentTop.panelType);
        }

        /// <summary>
        /// 彻底销毁面板（从缓存中移除）
        /// </summary>
        public bool DestroyPanel(EPanelType panelType)
        {
            if (_cachedPanels.TryGetValue(panelType, out UIPanelBase panel))
            {
                // 从栈中移除
                if (_panelStack.Contains(panel))
                {
                    RemoveFromStack(panel);

                }

                // 销毁实例
                panel.Destroy();
                // 从缓存中移除
                _cachedPanels.Remove(panelType);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从栈中移除指定面板
        /// </summary>
        private void RemoveFromStack(UIPanelBase panel)
        {
            List<UIPanelBase> tempList = new List<UIPanelBase>();

            // 弹出所有元素直到找到目标面板
            while (_panelStack.Count > 0)
            {
                var current = _panelStack.Pop();
                if (current == panel)
                {
                    break;
                }
                tempList.Add(current);
            }

            // 将临时存储的元素重新压回栈中
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                _panelStack.Push(tempList[i]);
            }
        }

        /// <summary>
        /// 从Resources加载面板
        /// </summary>
        private UIPanelBase LoadPanelFromResources(EPanelType panelType)
        {
            string resourcePath = PANEL_RESOURCE_PATH + panelType.ToString();
            GameObject panelPrefab = Resources.Load<GameObject>(resourcePath);
            if (panelPrefab == null)
            {
                Debug.LogError($"找不到面板预制体: {resourcePath}");
                return null;
            }

            // 实例化面板并设置父节点
            GameObject panelObj = GameObject.Instantiate(panelPrefab, panelRoot);
            panelObj.name = panelType.ToString();

            // 获取面板组件
            UIPanelBase panel = panelObj.GetComponent<UIPanelBase>();
            if (panel == null)
            {
                Debug.LogError($"面板 {panelType} 缺少 UIPanelBase 组件");
                panel.Destroy();
                return null;
            }

            // 确保面板类型正确
            panel.panelType = panelType;

            // 初始隐藏新面板
            panelObj.SetActive(false);

            return panel;
        }

        //关闭所有面板
        public void HideAllPanel()
        {
            foreach (var panel in _panelStack)
            {
                Debug.Log($"name is {panel.ToString()}");
                panel.Hide();
            }
        }

        public void OpenPanelFromList(List<EPanelType> EPanelTypeList, Action isAllLoaded = null)
        {
            // 先关闭所有面板
            HideAllPanel();

            // 依次打开列表中的面板
            foreach (var panelType in EPanelTypeList)
            {
                OpenPanel(panelType);
            }

            isAllLoaded?.Invoke();
        }

        /// <summary>
        /// 获取缓存的面板
        /// </summary>
        public UIPanelBase GetCachedPanel(EPanelType panelType)
        {
            if (_cachedPanels.TryGetValue(panelType, out var panel))
            {
                return panel;
            }
            return null;
        }
    }
}
