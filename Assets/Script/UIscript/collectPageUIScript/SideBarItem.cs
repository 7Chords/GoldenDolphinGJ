using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBarItem : MonoBehaviour
{
    private InstrumentStoreRefObj _storeRefObj;

    public Image imgHead;
    public Image imgNote_1;
    public Image imgNote_2;
    public Text txtNote_1;
    public Text txtNote_2;

    public void SetInfo(InstrumentStoreRefObj storeRefObj)
    {
        _storeRefObj = storeRefObj;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_storeRefObj == null)
            return;
        InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
            .Find(x => x.id == _storeRefObj.instrumentId);
        if (instrumentRefObj == null)
            return;
        imgHead.sprite = Resources.Load<Sprite>(instrumentRefObj.instrumentIconPath);
        
    }
}
