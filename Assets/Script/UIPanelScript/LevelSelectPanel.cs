using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : UIPanelBase
{
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject levelPrefab;
    
    protected override void OnShow()
    {
        List<BattleLevelRefObj> battleLevelRefList = SCRefDataMgr.Instance.battleLevelRefList.refDataList;

        if(battleLevelRefList != null && battleLevelRefList.Count > 0)
        {
            for(int i =0;i<battleLevelRefList.Count;i++)
            {
                GameObject levelGO = GameObject.Instantiate(levelPrefab);


            }
        }
    }


    protected override void OnHide(Action onHideFinished)
    {
        
    }
}
