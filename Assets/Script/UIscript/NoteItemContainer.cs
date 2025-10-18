using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteItemContainer : MonoBehaviour
{
    // Start is called before the first frame update
    // 手动把NoteItem拖进去
    [Header("音符UI（按顺序填充：0=高音，1=中音，2=低音）")]
    [Tooltip("请按顺序拖入 High, Middle, Low 的 NoteItem 引用，索引0=高音,1=中音,2=低音")]
    public List<NoteItem> noteItemList = new List<NoteItem>();
    
    void Start()
    {
        // 音符数量变化就刷新UI显示
        MsgCenter.RegisterMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE, SetNoteItemInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 设置音符UI显示信息
    private void SetNoteItemInfo()
    {
        noteItemList[0].SetInfo(PlayerMgr.Instance.GetNoteNum(NoteType.HightNote));
        noteItemList[1].SetInfo(PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote));
        noteItemList[2].SetInfo(PlayerMgr.Instance.GetNoteNum(NoteType.LowNote));
    }

    private void OnDestroy()
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE, SetNoteItemInfo);
    }
}
