using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NoteCollectPanel : UIPanelBase
{


    protected override void OnShow()
    {
        PlayerMgr.Instance.ClearInstrumentIdList();
        PlayerMgr.Instance.ResetNoteNum();
    }

    protected override void OnHide(Action onHideFinished)
    {
        onHideFinished?.Invoke();
    }
}
