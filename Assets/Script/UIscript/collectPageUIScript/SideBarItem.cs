using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBarItem : MonoBehaviour
{
    private InstrumentRefObj _instrumentRefObj;

    public Image imgHead;
    public Image imgNote_1;
    public Image imgNote_2;
    public Text txtNote_1;
    public Text txtNote_2;

    public void SetInfo(InstrumentRefObj storeRefObj)
    {
        _instrumentRefObj = storeRefObj;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_instrumentRefObj == null)
            return;
        InstrumentStoreRefObj storeRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
            .Find(x => x.instrumentId== _instrumentRefObj.id);
        if (storeRefObj == null)
            return;
        imgHead.sprite = Resources.Load<Sprite>(_instrumentRefObj.instrumentIconPath);
        //临时不好的写法
        int noteCount_1 = 0;
        int noteCount_2 = 0;
        if(storeRefObj.hightNoteNum == 0)
        {
            noteCount_1 = storeRefObj.middleNoteNum;
            noteCount_2 = storeRefObj.lowNoteNum;
            imgNote_1.sprite = Resources.Load<Sprite>("UI/Icon/中音符");
            imgNote_2.sprite = Resources.Load<Sprite>("UI/Icon/低音符");
        }
        else if(storeRefObj.middleNoteNum == 0)
        {
            noteCount_1 = storeRefObj.hightNoteNum;
            noteCount_2 = storeRefObj.lowNoteNum;
            imgNote_1.sprite = Resources.Load<Sprite>("UI/Icon/高音符");
            imgNote_2.sprite = Resources.Load<Sprite>("UI/Icon/低音符");
        }
        else
        {
            noteCount_1 = storeRefObj.hightNoteNum;
            noteCount_2 = storeRefObj.middleNoteNum;
            imgNote_1.sprite = Resources.Load<Sprite>("UI/Icon/高音符");
            imgNote_2.sprite = Resources.Load<Sprite>("UI/Icon/中音符");
        }
        txtNote_1.text = "×" + noteCount_1.ToString();
        txtNote_2.text = "×" + noteCount_2.ToString();
    }
}
