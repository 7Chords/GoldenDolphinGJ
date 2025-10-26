using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家管理器 管理玩家信息
/// </summary>
public class PlayerMgr : SingletonPersistent<PlayerMgr>
{
    //玩家当前持有的乐器信息
    public List<long> instrumentIdList;
    public Dictionary<NoteType, int> noteDic;// 玩家现在有的音符资源
    protected override void Awake()
    {
        base.Awake();
        // 初始化
        Init();
    }

    public int GetCurrentMaxInstrumentNum()
    {
        // 引导关卡 最大只能选择两个乐器
        if (GameMgr.Instance.curLevel == 1)
            return 2;
        else
            return 3;
    }
    public void SetAllNoteNum2Nine()
    {
        noteDic[NoteType.LowNote] = 9;
        noteDic[NoteType.MiddleNote] = 9;
        noteDic[NoteType.HightNote] = 9;
        MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);
        NoteMgr.instance.isEnd = true;
    }

    // 添加对应种类音符数量的方法
    public void AddHMLNoteNum(int numH, int numM, int numL)
    {
        noteDic[NoteType.LowNote] += numL;
        noteDic[NoteType.MiddleNote] += numM;
        noteDic[NoteType.HightNote] += numH;
        MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);
    }

    // 对外公开资源增减的方法
    public void AddNoteNum(NoteType noteType)
    {
        noteDic[noteType] ++;

        // 数量变更 发送消息
        MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);
    }

    public void ClearInstrumentIdList()
    {
        instrumentIdList.Clear();
    }

    public void ResetNoteNum()
    {
        noteDic[NoteType.LowNote] = 0;
        noteDic[NoteType.MiddleNote] = 0;
        noteDic[NoteType.HightNote] = 0;
        MsgCenter.SendMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE);
    }
    public void RemoveNoteNum(NoteType noteType, int num) 
    {
        if (noteDic[noteType] >= num) noteDic[noteType] -= num;
        else Debug.Log("current noteType not enough");
    }
    // 获得对应种类的音符数量
    public int GetNoteNum(NoteType noteType)
    {
        return noteDic[noteType];
    }
    private void Init()
    {
        instrumentIdList = new List<long>();
        // 初始化玩家目前有的音符种类数量
        noteDic = new Dictionary<NoteType, int>
        {
            { NoteType.LowNote, 0 },
            { NoteType.MiddleNote, 0 },
            { NoteType.HightNote, 0 }
        };
    }
}
