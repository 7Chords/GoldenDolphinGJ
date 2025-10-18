using GJFramework;
using System;
using UnityEngine;

public class StorePanel : UIPanelBase
{
    protected override void OnShow()
    {
    }

    protected override void OnHide(Action onHideFinished)
    {
        onHideFinished?.Invoke();
    }
}