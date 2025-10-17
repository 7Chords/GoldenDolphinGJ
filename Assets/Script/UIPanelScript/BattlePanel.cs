using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : UIPanelBase
{
    public GridLayout instrumentGridLayout;

    [Header("乐器预制体")]
    public GameObject instrumentPrefab;
    [Header("敌人预制体")]
    public GameObject enemyPrefab;
    [Header("敌人生成位置")]
    public RectTransform enemySpawnTransfrom;

    public Text txtTurnCount;
    public Text txtTurnOwner;

    private int _turnCount;
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
        _turnCount = 1;
        txtTurnCount.text = _turnCount.ToString();

    }

    public void SpawnInstruments()
    {
        for (int i = 0; i < PlayerMgr.Instance.instrumentInfoList.Count; i++)
        {
            GameObject instrumentGO = GameObject.Instantiate(instrumentPrefab);
            if (instrumentGO == null)
                continue;
            Instrument instrument = instrumentGO.GetComponent<Instrument>();
            if (instrument == null)
                continue;
            instrument.SetInfo(PlayerMgr.Instance.instrumentInfoList[i]);
            instrument.Init();
        }

    }
}
