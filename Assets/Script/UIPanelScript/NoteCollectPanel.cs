using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NoteCollectPanel : UIPanelBase
{
    [Header("开发者模式专用")]
    public Button SkipBtnForEditor;
    [SerializeField] private collectPageSkinSetter skinSetter;

    public Button btnOpenSideBar;
    protected override void OnShow()
    {
        SkipBtnForEditor.gameObject.SetActive(Application.isEditor);
#if UNITY_EDITOR
        // 跳过收集音符环节 直接前往商店
        SkipBtnForEditor.onClick.AddListener(()=>
        {
            PlayerMgr.Instance.SetAllNoteNum2Nine();
            MsgCenter.SendMsgAct(MsgConst.ON_STORE_OPEN);
            PanelUIMgr.Instance.OpenPanel(EPanelType.StorePanel);
            PanelUIMgr.Instance.ClosePanel(EPanelType.NoteCollectPanel);
            PanelUIMgr.Instance.ClosePanel(EPanelType.ColloctFinishPanel);
        });
#endif
        btnOpenSideBar.onClick.AddListener(() =>
        {

        });

        PlayerMgr.Instance.ClearInstrumentIdList();
        PlayerMgr.Instance.ResetNoteNum();
        AudioMgr.Instance.PlayBgm("土耳其");
        // 设置一下皮肤
        SetCurSkin();
    }

    private void SetCurSkin()
    {
        int level = GameMgr.Instance.curLevel;
        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
                .Find(x => x.level == level);

        CollectPageSkinRefObj collectPageSkinRefObj = SCRefDataMgr.Instance.collectPageSkinRefList.refDataList
            .Find(x => x.id == battleLevelRefObj.collectPageSkinId);

        skinSetter.SetCollectPageSkinInfo(collectPageSkinRefObj);

    }
    protected override void OnHide(Action onHideFinished)
    {
        btnOpenSideBar.onClick.RemoveAllListeners();
        onHideFinished?.Invoke();
    }
}
