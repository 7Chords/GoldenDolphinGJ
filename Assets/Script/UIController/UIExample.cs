using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GJFramework;

public class UIExample : MonoBehaviour
{
    public Container container;         // 在 Inspector 赋值
    public GameObject itemPrefab;       // 运行时动态添加的 prefab

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) container.AddItemFromPrefab(itemPrefab);
    }

    void RemoveFirst()
    {
        if (container.InstantiatedItems.Count > 0)
        {
            var first = container.InstantiatedItems[0];
            container.RemoveItem(first);
        }
    }

    void ClearAll()
    {
        container.ClearItems();
    }
}
