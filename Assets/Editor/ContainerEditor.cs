using UnityEditor;
using UnityEngine;
using GJFramework;

[CustomEditor(typeof(Container))]
public class ContainerEditor : Editor
{
    private Container container;
    private SerializedProperty prefabListProp;

    private void OnEnable()
    {
        container = (Container)target;
        prefabListProp = serializedObject.FindProperty("prefabList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Container（可视化容器）", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 显示并允许拖拽 Prefab 列表
        EditorGUILayout.PropertyField(prefabListProp, new GUIContent("Prefab 列表"), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("运行 / 编辑 操作", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Instantiate All"))
        {
            Undo.RecordObject(container, "Instantiate All Prefabs");
            container.InstantiateAllPrefabs();
            EditorUtility.SetDirty(container);
        }
        if (GUILayout.Button("Clear Items"))
        {
            Undo.RecordObject(container, "Clear Items");
            container.ClearItems();
            EditorUtility.SetDirty(container);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 运行时 API 提示
        EditorGUILayout.HelpBox("运行时通过脚本调用 AddItemFromPrefab/RemoveItem/ClearItems 来动态管理子元素。", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}