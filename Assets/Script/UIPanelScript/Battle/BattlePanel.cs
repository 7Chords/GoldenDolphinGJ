using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : UIPanelBase
{
    [Header("敌人预制体")]
    public GameObject enemyPrefab;
    [Header("敌人生成位置")]
    public RectTransform enemySpawnTransfrom;
    [Header("乐器Conatiner")]
    public InstrumentContainer instrumentContainer;
    [Header("回合数文本")]
    public Text txtTurnCount;
    [Header("回合持有者文本")]
    public Text txtTurnOwner;
    protected override void OnShow()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);


    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);

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
        GameObject enemyItem = GameObject.Instantiate(enemyPrefab);
        enemyItem.transform.SetParent(transform);
        enemyItem.transform.localPosition = enemySpawnTransfrom.localPosition;
        EnemyItem item = enemyItem.GetComponent<EnemyItem>();
        item.SetInfo(enemyInfo);
    }
}
