using GJFramework;
using UnityEngine;

public class StorePanel : UIPanelBase
{
    protected override void OnShow()
    {
        Debug.Log($"{this.name} is Show!");
    }

    protected override void OnHide()
    {
        Debug.Log($"{this.name} is Hide!");
    }
}