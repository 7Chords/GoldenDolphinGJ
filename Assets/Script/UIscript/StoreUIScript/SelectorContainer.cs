using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SelectorContainer : MonoBehaviour
{
    [SerializeField] private List<SelectorContainerItem> selectItemList;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image[] images;
    // Start is called before the first frame update
    void Start()
    {
        // 当选中的时候 触发回调
        MsgCenter.RegisterMsg(MsgConst.ON_STORE_ITEM_SELECT, OnSelectedCharacter);
        MsgCenter.RegisterMsgAct(MsgConst.ON_STORE_OPEN, RefreshInfo);
        MsgCenter.RegisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_BEFORE_COMPLETE, ReturnSelectorItem2Store);
        Init();
    }

    /// <summary>
    /// 修改Secletor对应位置信息
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_sprite"></param>

    public void Init()
    {
        // 引导关卡 做一下特判
        SetSelectBackGroundSkin();
        bool isFirstLevel = GameMgr.Instance.curLevel == 1;
        selectItemList[2].ParentGameObject.SetActive(!isFirstLevel);
        selectItemList[2].IsSelected = isFirstLevel;

    }

    private void OnSelectedCharacter(object[] _objs)
    {
        if (_objs.Length <= 0 || _objs == null)
        {
            Debug.Log("error");
            return;
        }

        Sprite sprite = (Sprite)_objs[0];
        long storeItemId = (long)_objs[1];
        // 逻辑是 如果第一个位置没有被选中 就设置第一个位置
        // 同时设置 图片和商品Id 
        // 新增 如果是第一关 则最大只能选择两个乐器
        if (!selectItemList[0].IsSelected)
        {
            selectItemList[0].SetItemInfo(sprite, storeItemId);
            selectItemList[0].gameObject.SetActive(true);
        }
        else if(!selectItemList[1].IsSelected)
        {
            selectItemList[1].SetItemInfo(sprite, storeItemId);
            selectItemList[1].gameObject.SetActive(true);
        }
        else if(!selectItemList[2].IsSelected && GameMgr.Instance.curLevel != 1)
        {
            selectItemList[2].SetItemInfo(sprite, storeItemId);
            selectItemList[2].gameObject.SetActive(true);
        }
    }

    public void ReturnSelectorItem2Store(object[] _objs)
    {

        if (_objs.Length <= 0 || _objs == null)
        {
            Debug.Log("error");
            return;
        }
        long storeItemId = (long)_objs[0];
        InstrumentStoreRefObj instrumentStoreRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
.Find(x => x.id == storeItemId);
        for (int i = 0; i < selectItemList.Count; i++)
        {
            if(selectItemList[i].IsSelected && selectItemList[i].StoreItemId == storeItemId)
            {
                // 不仅设置状态 还要归还资源 设置当前持有的乐器
                selectItemList[i].gameObject.SetActive(false);
                selectItemList[i].IsSelected = false;
                PlayerMgr.Instance.instrumentIdList.Remove(instrumentStoreRefObj.instrumentId);
                PlayerMgr.Instance.AddHMLNoteNum(instrumentStoreRefObj.hightNoteNum
                    , instrumentStoreRefObj.middleNoteNum
                    , instrumentStoreRefObj.lowNoteNum);

                break;
            }
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
        MsgCenter.UnregisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_BEFORE_COMPLETE, ReturnSelectorItem2Store);
    }

    private void SetSelectBackGroundSkin()
    {
        int skinId = GameMgr.Instance.curLevel;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = sprites[skinId - 1];
        }
    }
}
