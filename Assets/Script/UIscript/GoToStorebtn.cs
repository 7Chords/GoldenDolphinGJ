using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToStorebtn : MonoBehaviour, IPointerClickHandler
{
    // 去商店按钮点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioMgr.Instance.PlaySfx("木头按钮");
        MsgCenter.SendMsgAct(MsgConst.ON_STORE_OPEN);
        PanelUIMgr.Instance.OpenPanel(EPanelType.StorePanel);
        PanelUIMgr.Instance.ClosePanel(EPanelType.NoteCollectPanel);
        PanelUIMgr.Instance.ClosePanel(EPanelType.ColloctFinishPanel);
    }
}
