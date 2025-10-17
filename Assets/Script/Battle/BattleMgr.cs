using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleMgr : MonoBehaviour
{
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
