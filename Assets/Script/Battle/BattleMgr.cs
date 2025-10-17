using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理器
/// </summary>
public class BattleMgr : SingletonMono<BattleMgr>
{
    public ETurnType curTurn;
    public int turnCount;

    private EnemyInfo _enemyInfo;//敌人信息
    private List<InstrumentInfo> _instrumentInfoList;//音乐列表信息


    private void Start()
    {
        //todo:test
        StartBattle();
    }
    public void StartBattle()
    {
        PanelUIMgr.Instance.OpenPanel(EPanelType.BattlePanel);

        curTurn = ETurnType.Player;
        turnCount = 0;

        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.level == GameMgr.Instance.curLevel);
        if (battleLevelRefObj != null)
        {
            _enemyInfo = new EnemyInfo(battleLevelRefObj.enemyType,
                battleLevelRefObj.enemyName,
                battleLevelRefObj.enemyHealth,
                battleLevelRefObj.enemyAttack,
                battleLevelRefObj.enemyIconPath,
                battleLevelRefObj.enemyBgPath);
        }
        _instrumentInfoList = new List<InstrumentInfo>();
        for(int i =0;i<PlayerMgr.Instance.instrumentIdList.Count;i++)
        {
            InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == PlayerMgr.Instance.instrumentIdList[i]);
            InstrumentInfo info = new InstrumentInfo(instrumentRefObj.instrumentType,
                instrumentRefObj.instrumentName,
                instrumentRefObj.instrumentDesc,
                instrumentRefObj.health,
                instrumentRefObj.attack,
                instrumentRefObj.instrumentIconPath,
                instrumentRefObj.instrumentBgPath);
            _instrumentInfoList.Add(info);
        }

        MsgCenter.SendMsg(MsgConst.ON_BATTLE_START, _enemyInfo, _instrumentInfoList);
    }


}
