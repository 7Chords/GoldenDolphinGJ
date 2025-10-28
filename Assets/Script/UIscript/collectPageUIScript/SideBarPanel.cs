using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBarPanel : UIPanelBase
{
    public Button btnClose;
    public Transform itemParentTransform;
    protected override void OnShow()
    {
        btnClose.onClick.AddListener(() =>
        {

        });
    }

    protected override void OnHide(Action onHideFinished)
    {
        btnClose.onClick.RemoveAllListeners();
    }
}
