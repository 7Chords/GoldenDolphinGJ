using GJFramework;
using System;
using UnityEngine;

public class StorePanel : UIPanelBase
{
    [SerializeField] private storeSkinSetter skinSetter;
    protected override void OnShow()
    {
        AudioMgr.Instance.PlayBgm("背景音乐");
        // 进入就清空已收集的乐器ID列表
        PlayerMgr.Instance.ClearInstrumentIdList();
    }

    protected override void OnHide(Action onHideFinished)
    {
        onHideFinished?.Invoke();
    }
}