using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattelGuidePanel : UIPanelBase
{
    protected override void OnShow()
    {

    }

    protected override void OnHide(Action onHideFinished)
    {
        onHideFinished?.Invoke();
    }
}