using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [Header("长持续时间装饰物位置列表")]
    public List<Transform> longDurationDecPosList;
    [Header("中持续时间装饰物位置列表")]
    public List<Transform> middleDurationDecPosList;
    [Header("短持续时间装饰物位置列表")]
    public List<Transform> smallDurationDecPosList;
    [Header("长持续时间装饰物生成间隔")]
    public float longDurationDecSpawnInterval;
    [Header("中持续时间装饰物生成间隔")]
    public float middleDurationDecSpawnInterval;
    [Header("短持续时间装饰物生成间隔")]
    public float smallDurationDecSpawnInterval;
    [Header("长持续时间装饰物每次生成数量")]
    public float longDurationDecSpawnPerAmount;
    [Header("中持续时间装饰物每次生成数量")]
    public float middleDurationDecSpawnPerAmount;
    [Header("短持续时间装饰物每次生成数量")]
    public float smallDurationDecSpawnPerAmount;
    [Header("长持续时间装饰物列表")]
    public List<GameObject> longDurationDecList;
    [Header("中持续时间装饰物列表")]
    public List<GameObject> middleDurationDecList;
    [Header("短持续时间装饰物列表")]
    public List<GameObject> smallDurationDecList;
    protected override void OnShow()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);
        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK, OnInstrumentStartAttack);
        MsgCenter.RegisterMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK, OnInstrumentEndAttack);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_START_ATTACK, OnEnemyStartAttack);
        MsgCenter.RegisterMsgAct(MsgConst.ON_ENEMY_END_ATTACK, OnEnemyEndAttack);
    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_BATTLE_START, OnBattleStartStart);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_START_ATTACK, OnEnemyStartAttack);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_INSTRUMENT_END_ATTACK, OnEnemyEndAttack);
        instrumentContainer.Hide();
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
        int randomIdx = 0;
        int randomPosIdx = 0;
        List<int> hasSpawnPosIdxList = new List<int>();
        for(int i =0;i< longDurationDecSpawnPerAmount;i++)
        {
            randomIdx = Random.Range(0, longDurationDecList.Count);
            randomPosIdx = Random.Range(0, longDurationDecPosList.Count);

            while(hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, longDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(longDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);

            decGO.GetComponent<RectTransform>().localPosition = longDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }

        hasSpawnPosIdxList.Clear();
        for (int i = 0; i < middleDurationDecSpawnPerAmount; i++)
        {
            randomIdx = Random.Range(0, middleDurationDecList.Count);
            randomPosIdx = Random.Range(0, middleDurationDecPosList.Count);

            while (hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, middleDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(middleDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);
            decGO.GetComponent<RectTransform>().localPosition = middleDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }

        hasSpawnPosIdxList.Clear();
        for (int i = 0; i < smallDurationDecSpawnPerAmount; i++)
        {
            randomIdx = Random.Range(0, smallDurationDecList.Count);
            randomPosIdx = Random.Range(0, smallDurationDecPosList.Count);

            while (hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, smallDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(smallDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);

            decGO.GetComponent<RectTransform>().localPosition = smallDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }

    }

    private void OnInstrumentEndAttack()
    {
        maskAttack.SetActive(false);
    }

    private void OnEnemyStartAttack()
    {
        maskAttack.SetActive(true);
        int randomIdx = 0;
        int randomPosIdx = 0;
        List<int> hasSpawnPosIdxList = new List<int>();
        for (int i = 0; i < longDurationDecSpawnPerAmount; i++)
        {
            randomIdx = Random.Range(0, longDurationDecList.Count);
            randomPosIdx = Random.Range(0, longDurationDecPosList.Count);

            while (hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, longDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(longDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);

            decGO.GetComponent<RectTransform>().localPosition = longDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }

        hasSpawnPosIdxList.Clear();
        for (int i = 0; i < middleDurationDecSpawnPerAmount; i++)
        {
            randomIdx = Random.Range(0, middleDurationDecList.Count);
            randomPosIdx = Random.Range(0, middleDurationDecPosList.Count);

            while (hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, middleDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(middleDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);
            decGO.GetComponent<RectTransform>().localPosition = middleDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }

        hasSpawnPosIdxList.Clear();
        for (int i = 0; i < smallDurationDecSpawnPerAmount; i++)
        {
            randomIdx = Random.Range(0, smallDurationDecList.Count);
            randomPosIdx = Random.Range(0, smallDurationDecPosList.Count);

            while (hasSpawnPosIdxList.Contains(randomPosIdx))
            {
                randomPosIdx = Random.Range(0, smallDurationDecPosList.Count);
            }
            hasSpawnPosIdxList.Add(randomPosIdx);
            GameObject decGO = GameObject.Instantiate(smallDurationDecList[randomIdx]);
            decGO.transform.SetParent(transform);

            decGO.GetComponent<RectTransform>().localPosition = smallDurationDecPosList[randomPosIdx].localPosition;
            decGO.GetComponent<BattleDecoration>().Init();
        }
    }

    private void OnEnemyEndAttack()
    {
        maskAttack.SetActive(false);
    }
}
