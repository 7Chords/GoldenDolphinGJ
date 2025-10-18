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

    private int _instrumentActionCount;
    private int _instrumentDeadCount;

    //比较烂的写法 时间紧迫
    public List<InstrumentItem> instrumentItemList;
    public EnemyItem enemyItem;

    public bool gameStarted;
    private void Start()
    {
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER, OnInstrumentActionOver);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_ACTION_OVER, OnEnemyActionOver);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_DEAD, OnEnemyDead);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_DEAD, OnInstrumentDead);

        instrumentItemList = new List<InstrumentItem>();

        StartBattle();
    }

    private void OnDestroy()
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER, OnInstrumentActionOver);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_ENEMY_ACTION_OVER, OnEnemyActionOver);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_ENEMY_DEAD, OnEnemyDead);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_DEAD, OnInstrumentDead);
    }
    public void StartBattle()
    {
        gameStarted = true;

        PanelUIMgr.Instance.OpenPanel(EPanelType.BattlePanel);

        curTurn = ETurnType.Player;
        turnCount = 1;

        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.level == GameMgr.Instance.curLevel);
        if (battleLevelRefObj != null)
        {
            _enemyInfo = new EnemyInfo(battleLevelRefObj.enemyType,
                battleLevelRefObj.enemyName,
                battleLevelRefObj.enemyHealth,
                battleLevelRefObj.enemyAttack,
                battleLevelRefObj.enemyIconPath,
                battleLevelRefObj.enemyBgPath,
                battleLevelRefObj.enemyBodyPath);
        }
        _instrumentInfoList = new List<InstrumentInfo>();
        for(int i =0;i<PlayerMgr.Instance.instrumentIdList.Count;i++)
        {
            InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == PlayerMgr.Instance.instrumentIdList[i]);
            InstrumentInfo info = new InstrumentInfo(instrumentRefObj.instrumentType,
                instrumentRefObj.effectType,
                instrumentRefObj.instrumentName,
                instrumentRefObj.instrumentDesc,
                instrumentRefObj.health,
                instrumentRefObj.attack,
                instrumentRefObj.heal,
                instrumentRefObj.buff,
                instrumentRefObj.instrumentIconPath,
                instrumentRefObj.instrumentBgPath,
                instrumentRefObj.instrumentBodyPath,
                instrumentRefObj.instrumentBodyBgPath,
                instrumentRefObj.instrumentNamePath,
                instrumentRefObj.instrumentBodyBgWithChaPath);
            _instrumentInfoList.Add(info);
        }

        MsgCenter.SendMsg(MsgConst.ON_BATTLE_START, _enemyInfo, _instrumentInfoList);
    }

    private void FinishBattle(bool playerWin)
    {
        gameStarted = false;
        if (playerWin)
        {
            GameMgr.Instance.curLevel++;
            PanelUIMgr.Instance.OpenPanel(EPanelType.BattleWinPanel);
        }
        else
        {

            //todo:else
        }
    }
    public void RegInstrumentItem(InstrumentItem item)
    {
        instrumentItemList?.Add(item);
    }
    public void UnregInstrumentItem(InstrumentItem item)
    {
        if (instrumentItemList == null || instrumentItemList.Count == 0)
            return;
        if (instrumentItemList.Contains(item))
            instrumentItemList.Remove(item);
    }

    public void RegEnemyItem(EnemyItem item)
    {
        enemyItem = item;
    }

    public void UnregEnemyItem()
    {
        enemyItem = null;
    }

    private void OnInstrumentActionOver()
    {
        if (!gameStarted)
            return;

        _instrumentActionCount++;
        if(_instrumentActionCount == _instrumentInfoList.Count)
        {
            curTurn = ETurnType.Enemy;
            turnCount++;
            MsgCenter.SendMsgAct(MsgConst.ON_TURN_CHG);
        }
    }
    private void OnEnemyActionOver()
    {
       
        if (!gameStarted)
            return;
        Debug.Log("OnEnemyActionOver");
        _instrumentActionCount = 0;
        curTurn = ETurnType.Player;
        turnCount++;
        MsgCenter.SendMsgAct(MsgConst.ON_TURN_CHG);
    }

    private void OnEnemyDead()
    {
        FinishBattle(true);
    }

    private void OnInstrumentDead()
    {
        _instrumentDeadCount++;
        if(_instrumentDeadCount == _instrumentInfoList.Count)
        {
            FinishBattle(false);
        }
    }
}
