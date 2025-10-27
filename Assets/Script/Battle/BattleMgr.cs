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

    private int _instrumentAliveCount;

    public bool isPlaying;
    private void Start()
    {
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_ACTION_OVER, OnInstrumentActionOver);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_ACTION_OVER, OnEnemyActionOver);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_DEAD, OnEnemyDead);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_DEAD, OnInstrumentDead);

        instrumentItemList = new List<InstrumentItem>();
        isPlaying = false;

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
                battleLevelRefObj.enemyResId);
        }
        _instrumentInfoList = new List<InstrumentInfo>();
        for(int i =0;i<PlayerMgr.Instance.instrumentIdList.Count;i++)
        {
            InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == PlayerMgr.Instance.instrumentIdList[i]);
            InstrumentInfo info = new InstrumentInfo(instrumentRefObj);
            _instrumentInfoList.Add(info);
        }
        _instrumentAliveCount = _instrumentInfoList.Count;
        MsgCenter.SendMsg(MsgConst.ON_BATTLE_START, _enemyInfo, _instrumentInfoList);

        AudioMgr.Instance.PlayBgm(battleLevelRefObj.bgmName);
    }

    private void FinishBattle(bool playerWin)
    {
        gameStarted = false;
        if (playerWin)
        {
            AudioMgr.Instance.PlaySfx("游戏胜利cut");
            PanelUIMgr.Instance.OpenPanel(EPanelType.BattleWinPanel);
        }
        else
        {
            AudioMgr.Instance.PlaySfx("游戏失败cut");
            PanelUIMgr.Instance.OpenPanel(EPanelType.BattleLosePanel);
        }
        SkillHandler.bounceRate = 0.2f;
        EnemyContinueEffectHandler.UnregContinueEffect();
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
        if(_instrumentActionCount == _instrumentAliveCount)
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
        _instrumentActionCount = 0;
        curTurn = ETurnType.Player;
        turnCount++;
        MsgCenter.SendMsgAct(MsgConst.ON_TURN_CHG);
    }

    private void OnEnemyDead()
    {
        if (!gameStarted)
            return;
        FinishBattle(true);
    }

    private void OnInstrumentDead()
    {
        if (!gameStarted)
            return;
        _instrumentDeadCount++;
        _instrumentAliveCount--;
        if (_instrumentDeadCount == _instrumentInfoList.Count)
        {
            FinishBattle(false);
        }
    }
}
