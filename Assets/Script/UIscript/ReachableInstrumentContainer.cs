using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachableInstrumentContainer : MonoBehaviour
{
    public List<InstrumentContainerItem> instrumentContainerItems;
    private long currentBattleLevelId = 1001;// 当前战斗关卡ID
    private List<long> curLevelInstrumentIdList;

    // Start is called before the first frame update
    void Start()
    {
        //currentBattleLevelId = GameMgr.Instance.curLevel;
        BattleLevelRefObj battleLevelRefObj = SCRefDataMgr.Instance.battleLevelRefList.refDataList
            .Find(x => x.id == currentBattleLevelId);
        if(battleLevelRefObj != null)
        {
            curLevelInstrumentIdList = battleLevelRefObj.recommendinstrumentsIdList;
        }
        else
        {
            Debug.LogError("battleLevelRefObj有误");
        }
        SetContainerItemInfo();

        MsgCenter.RegisterMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE, refresh);
    }

    private void SetContainerItemInfo()
    {
        // 因为乐器容器 严格 == 乐器id 所以索引index 可以共用
        for(int i = 0; i < instrumentContainerItems.Count; i++)
        {
            // 乐器id -> 乐器配表 -> 乐器图片 -> 设置UI
            InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == curLevelInstrumentIdList[i]);
           
            if (instrumentRefObj != null)
            {
                Sprite tempSprit = ResourceUtils.LoadSprite(instrumentRefObj.instrumentIconPath);
                Sprite tempSpriteUnlock = ResourceUtils.LoadSprite(instrumentRefObj.instrumentIconUnlockPath);
                if (tempSprit != null)
                    instrumentContainerItems[i].SetInfo(tempSprit, tempSpriteUnlock, curLevelInstrumentIdList[i]);
                else Debug.LogError("乐器图片有误");
            }
            else Debug.LogError("instrumentRefObj有误");
        }
        refresh();
    }

    private void refresh()
    {
        foreach(var item in instrumentContainerItems)
        {
            item.refreshIcon();
        }
    }

    private void OnDestroy()
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_NOTE_COUNT_CHANGE, refresh);
    }
}



