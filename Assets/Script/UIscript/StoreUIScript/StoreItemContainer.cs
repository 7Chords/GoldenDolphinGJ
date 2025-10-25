using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemContainer : SingletonMono<StoreItemContainer>
{
    // 由于字典不能序列化，这里用列表初始化商品购买状态
    [SerializeField] private List<StoreContainerItem> initStoreItemContainer;
    // 商品id -> 是否购买
    public Dictionary<long, bool> storeItemList;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Init();
        SetAllChildItemGrayState();
    }
    // 将所有子商品设置是否为灰色状态
    private void SetAllChildItemGrayState(long curSelectorStoreItemId = -1)
    {
        foreach (var item in initStoreItemContainer)
        {
            InstrumentRefObj tempInstrumentRefObj = item.InstrumentStoreRefObj == null ? null :
                SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == item.InstrumentStoreRefObj.instrumentId);

            if(tempInstrumentRefObj == null)
            {
                Debug.Log("tempInstrumentRefObj is null");
                return;
            }
            item.SetInfo(tempInstrumentRefObj.unlockLevelId);

            item.SetGrayState(curSelectorStoreItemId, true);
        }
    }
    private void Start()
    {
        MsgCenter.RegisterMsg(MsgConst.ON_STORE_ITEM_SELECT, OnStoreItemSelect);
        MsgCenter.RegisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_WHILE_DOTWEEN_COMPLETE, OnSelectorItemCancle);
    }

    public void SetChildInfo()
    {

    }    

    // 维护商品购买状态
    public void SetStoreItemState(long storeId, bool isBuy)
    {
        if(storeItemList == null)
            storeItemList = new Dictionary<long, bool>();

        storeItemList[storeId] = isBuy;
    }

    public void Init()
    {
        storeItemList = new Dictionary<long, bool>();
        foreach (var item in initStoreItemContainer)
        {
            storeItemList[item.StoreItemId] = false;
            item.Init();
        }
    }
    public void OnStoreItemSelect(object[] _objs)
    {
        if (_objs.Length <= 0 || _objs == null)
        {
            Debug.Log("error");
            return;
        }
        long storeItemId = (long)_objs[1];
        SetStoreItemState(storeItemId, true);
        SetAllChildItemGrayState(storeItemId);
    }

    public void OnSelectorItemCancle(object[] _objs)
    {
        if (_objs.Length <= 0 || _objs == null)
        {
            Debug.Log("error");
            return;
        }
        long storeItemId = (long)_objs[0];
        SetStoreItemState(storeItemId, false);
    }

    private void OnDestroy()
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_STORE_ITEM_SELECT, OnStoreItemSelect);
        MsgCenter.UnregisterMsg(MsgConst.ON_SELECTOR_INSTRUMENT_CANCLE_WHILE_DOTWEEN_COMPLETE, OnSelectorItemCancle);
    }
}
