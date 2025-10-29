using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstrumentContainer : UIPanelBase
{

    [Header("乐器预制体")]
    public GameObject instrumentPrefab;

    [Header("布局组件")]
    public GridLayoutGroup instrumentGridLayout;

    private List<InstrumentItem> _instrumentItemList;

    protected override void OnShow()
    {
        _instrumentItemList = new List<InstrumentItem>();
    }
    public void SetInfo(List<InstrumentInfo> instrumentInfoList)
    {
        if (instrumentInfoList == null)
            return;
        int i = 0, count = 0;
        InstrumentItem item = null;
        for (i = 0; i < instrumentInfoList.Count; i++)
        {
            if (i < _instrumentItemList.Count)
            {
                item = _instrumentItemList[i];
            }
            else
            {
                GameObject itemGO = GameObject.Instantiate(instrumentPrefab);
                item = itemGO.GetComponent<InstrumentItem>();
                itemGO.transform.SetParent(instrumentGridLayout.transform);
                _instrumentItemList.Add(item);
            }
            if (item == null)
                continue;
            item.Show();
            item.SetInfo(instrumentInfoList[i]);
            count++;
        }

        for(int j =0; j<count;j++)
        {
            _instrumentItemList[j].SetSkillStar();
        }

        for (i = count; i < _instrumentItemList.Count; i++)
        {
            item = _instrumentItemList[i];
            if (item == null)
                continue;
            item.Hide();
        }
    }

    protected override void OnHide(Action onHideFinished)
    {
        foreach(var item in _instrumentItemList)
        {
            item.Hide();
        }
        onHideFinished?.Invoke();
    }

}
