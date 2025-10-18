using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : UIPanelBase
{
    [Header("敌人item")]
    public EnemyItem enemyItem;
    [Header("乐器Conatiner")]
    public InstrumentContainer instrumentContainer;
    [Header("回合数文本")]
    public Text txtTurnCount;
    [Header("回合持有者文本")]
    public Text txtTurnOwner;
    [Header("攻击时的遮罩")]
    public GameObject maskAttack;
    protected override void OnShow()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);
        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK, OnInstrumentStartAttack);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK, OnInstrumentEndAttack);

    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK, OnInstrumentStartAttack);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK, OnInstrumentEndAttack);
        onHideFinished?.Invoke();
    }

    private void OnBattleStartStart(object[] _objs)
    {
        if (_objs == null || _objs.Length == 0)
            return;

        EnemyInfo enemyInfo = _objs[0] as EnemyInfo;
        List<InstrumentInfo> instrumentInfoList = _objs[1] as List<InstrumentInfo>;

        txtTurnCount.text = BattleMgr.instance.turnCount.ToString();
        txtTurnOwner.text = BattleMgr.instance.curTurn.ToString();

        instrumentContainer.Show();
        instrumentContainer.SetInfo(instrumentInfoList);

        enemyItem.Show();
        enemyItem.SetInfo(enemyInfo);
    }


    private void OnTurnChg()
    {
        txtTurnCount.text = BattleMgr.instance.turnCount.ToString();
        txtTurnOwner.text = BattleMgr.instance.curTurn.ToString();
    }

    private void OnInstrumentStartAttack()
    {
        maskAttack.SetActive(true);
    }

    private void OnInstrumentEndAttack()
    {
        maskAttack.SetActive(false);
    }
}
