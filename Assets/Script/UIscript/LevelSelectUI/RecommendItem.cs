using GJFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecommendItem : MonoBehaviour
{
    public GameObject recommendInstrumentPrefab;
    public Transform recommondItemGroup;
    public Text txtHighAmount;
    public Text txtMiddleAmount;
    public Text txtLowAmount;


    private List<GameObject> _itemList;
    public void SetInfo(BattleLevelRefObj levelRefObj)
    {
        if (_itemList == null)
            _itemList = new List<GameObject>();
        else
        {
            foreach (var go in _itemList)
            {
                Destroy(go);
            }
            _itemList.Clear();
        }

        int totalHigh = 0;
        int totalMiddle = 0;
        int totalLow = 0;

        for (int i =0;i<levelRefObj.recommendinstrumentsIdList.Count;i++)
        {
            InstrumentRefObj refObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == levelRefObj.recommendinstrumentsIdList[i]);
            GameObject go = GameObject.Instantiate(recommendInstrumentPrefab);
            go.GetComponent<Image>().sprite = Resources.Load<Sprite>(refObj.instrumentPreviewIconPath);
            go.transform.SetParent(recommondItemGroup);
            InstrumentStoreRefObj storeRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
                .Find(x => x.instrumentId == levelRefObj.recommendinstrumentsIdList[i]);
            totalHigh += storeRefObj.hightNoteNum;
            totalMiddle += storeRefObj.middleNoteNum;
            totalLow += storeRefObj.lowNoteNum;
            _itemList.Add(go);
        }
        txtHighAmount.text = "×" + totalHigh.ToString();
        txtMiddleAmount.text = "×" + totalMiddle.ToString();
        txtLowAmount.text = "×" + totalLow.ToString();

    }

}
