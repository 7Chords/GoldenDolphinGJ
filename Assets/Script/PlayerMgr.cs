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
    public List<InstrumentInfo> instrumentInfoList;
    public Dictionary<NoteType, int> noteDic;// 玩家现在有的音符资源
    protected override void Awake()
    {
        // 初始化
        Init();
    }
    // 对外公开资源增减的方法
    public void AddNoteNum(NoteType noteType)
    {
        noteDic[noteType] ++;
    }

    public void RemoveNoteNum(NoteType noteType, int num) 
    {
        if (noteDic[noteType] >= num) noteDic[noteType] -= num;
        else Debug.Log("current noteType not enough");
    }
    private void Init()
    {
        // 初始化玩家目前有的音符种类数量
        noteDic = new Dictionary<NoteType, int>
        {
            { NoteType.LowNote, 0 },
            { NoteType.MiddleNote, 0 },
            { NoteType.HightNote, 0 }
        };
    }
}
