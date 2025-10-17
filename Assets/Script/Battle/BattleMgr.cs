using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleMgr:MonoBehaviour
{
    //public const string INSTRUMENT_BANNER_PATH = "UI/Battle/panel_instrument_banner";
    //public const string INSTRUMENT_CARD_PATH = "UI/Battle/prefab_instrument";
    //public const string BATTLE_UI_PATH = "UI/Battle/panel_battle";

    public Canvas gameCanvas;

    private ETurnType _curTurn;
    public int _turnCount;
    public void StartBattle()
    {
        PanelUIMgr.Instance.OpenPanel(EPanelType.BattlePanel); 

        _curTurn = ETurnType.Player;
        _turnCount = 0;

        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.level == GameMgr.Instance.curLevel);
        if (battleLevelRefObj != null)
            MsgCenter.SendMsg(MsgConst.ON_BATTLE_START, battleLevelRefObj);
    }


}
