using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SelectorContainer : MonoBehaviour
{
    [SerializeField] private List<SelectorContainerItem> selectItemList;
    // Start is called before the first frame update
    void Start()
    {
        // 当选中的时候 触发回调
        MsgCenter.RegisterMsg(MsgConst.ON_STORE_ITEM_SELECT, OnSelectedCharacter);
        MsgCenter.RegisterMsgAct(MsgConst.ON_STORE_OPEN, RefreshInfo);
    }

    private void OnSelectedCharacter(object[] _objs)
    {
        if (_objs.Length <= 0 || _objs == null)
        {
            Debug.Log("error");
            return;
        }
        Sprite sprite = (Sprite)_objs[0];

        if (!selectItemList[0].IsSelected)
        {
            
            selectItemList[0].SetItemInfo(sprite, 0);
            selectItemList[0].gameObject.SetActive(true);
        }
        else if(!selectItemList[1].IsSelected)
        {
            selectItemList[1].SetItemInfo(sprite,1);
            selectItemList[1].gameObject.SetActive(true);
        }
        else if(!selectItemList[2].IsSelected)
        {
            selectItemList[2].SetItemInfo(sprite,2);
            selectItemList[2].gameObject.SetActive(true);
        }
    }

    // refresh info
    public void RefreshInfo()
    {
        selectItemList[0].gameObject.SetActive(false);
        selectItemList[0].IsSelected = false;
        selectItemList[2].gameObject.SetActive(false);
        selectItemList[2].IsSelected = false;
        selectItemList[1].gameObject.SetActive(false);
        selectItemList[1].IsSelected = false;
    }
    private void OnDestroy()
    {
        // 反注册
        MsgCenter.UnregisterMsg(MsgConst.ON_STORE_ITEM_SELECT, OnSelectedCharacter);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_STORE_OPEN, RefreshInfo);
    }
}
