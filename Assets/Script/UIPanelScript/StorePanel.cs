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
        SetCurSkin();
    }

    protected override void OnHide(Action onHideFinished)
    {
        onHideFinished?.Invoke();
    }

    private void SetCurSkin()
    {
        int level = GameMgr.Instance.curLevel;
        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
                .Find(x => x.level == level);

        StorePageSkinRefObj storePageSkinRefObj = SCRefDataMgr.Instance.storePageSkinRefList.refDataList
            .Find(x => x.id == battleLevelRefObj.StorePageSkinId);

        skinSetter.SetStorePageSkinInfo(storePageSkinRefObj);

    }
}