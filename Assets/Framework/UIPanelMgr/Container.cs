using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GJFramework
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class Container : MonoBehaviour
    {
        [Header("可拖拽的子元素预制体列表（用于编辑器和运行时实例化）")]
        [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

        // 运行时/编辑器实例化的子对象（管理用）
        [SerializeField, HideInInspector] private List<GameObject> _instantiatedItems = new List<GameObject>();

        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        // 只读访问已实例化的子项
        public IReadOnlyList<GameObject> InstantiatedItems => _instantiatedItems;

        private void OnValidate()
        {
            // 确保引用
            _rectTransform = GetComponent<RectTransform>();
        }

        // 在 Scene 视图绘制容器边框，便于可视化大小
        private void OnDrawGizmos()
        {
            if (rectTransform == null) return;

            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Gizmos.color = new Color(0f, 0.6f, 1f, 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }

        #region 编辑器/运行时的实例化与管理 API

        // 用于 Inspector 显示与拖拽的列表访问
        public List<GameObject> PrefabList => prefabList;

        // 运行时或编辑器中从 prefab 添加一个实例（返回实例对象）
        public GameObject AddItemFromPrefab(GameObject prefab)
        {
            if (prefab == null) return null;

            GameObject go;
#if UNITY_EDITOR
            // 在编辑器中也允许实例化（方便预览）
            if (!Application.isPlaying)
                go = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, rectTransform) as GameObject;
            else
#endif
                go = GameObject.Instantiate(prefab);

            if (go == null) return null;

            // 把实例 parent 到容器下，不保持世界位置（false 保留本地 transform）
            go.transform.SetParent(rectTransform, false);

            // 标准化 localScale（保持 prefab 的局部缩放）
            go.transform.localScale = Vector3.one;

            // 如果子物体有 RectTransform，默认不强制修改 anchoredPosition，
            // 让 LayoutGroup 管理位置。如果你需要自定义位置，可在外部设置。
            var rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                // 可选：如果 prefab 的锚点/大小不合适，这里可以设置默认值：
                // rt.anchorMin = new Vector2(0.5f, 0.5f);
                // rt.anchorMax = new Vector2(0.5f, 0.5f);
                // rt.pivot = new Vector2(0.5f, 0.5f);
                rt.localScale = Vector3.one;
            }

            _instantiatedItems.Add(go);

            // 如果容器上有 LayoutGroup，强制刷新布局
            var layoutGroup = rectTransform.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            return go;
        }

        // 根据已存在的实例移除（销毁）
        public bool RemoveItem(GameObject instance)
        {
            if (instance == null) return false;
            if (!_instantiatedItems.Remove(instance)) return false;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(instance);
            else
#endif
                Destroy(instance);

            var layoutGroup = rectTransform.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            return true;
        }

        // 清空容器（销毁所有已实例化项）
        public void ClearItems()
        {
            for (int i = _instantiatedItems.Count - 1; i >= 0; i--)
            {
                var go = _instantiatedItems[i];
                if (go == null) continue;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(go);
                else
#endif
                    Destroy(go);
            }
            _instantiatedItems.Clear();

            var layoutGroup = rectTransform.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        // 一次性实例化 PrefabList 中所有预制体（编辑器/运行时皆可）
        public void InstantiateAllPrefabs()
        {
            foreach (var p in prefabList)
            {
                if (p != null)
                    AddItemFromPrefab(p);
            }
        }

        #endregion
    }
}