using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NoteCollectPanel : UIPanelBase
{
    [SerializeField] private GameObject btnGo;
    protected override void OnShow()
    {
        
    }

    protected override void OnHide(Action onHideFinished)
    {
        Debug.Log($"{this.name} is Hide!");
        onHideFinished?.Invoke();
    }
}
