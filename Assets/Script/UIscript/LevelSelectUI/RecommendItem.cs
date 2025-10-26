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

    public void SetInfo(BattleLevelRefObj levelRefObj, int index)
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

        // 根据index获取对应的ID子集
        List<long> targetIds = GetTargetIds(levelRefObj.recommendinstrumentsIdList, index);

        for (int i = 0; i < targetIds.Count; i++)
        {
            InstrumentRefObj refObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
                .Find(x => x.id == targetIds[i]);
            GameObject go = GameObject.Instantiate(recommendInstrumentPrefab);
            go.GetComponent<Image>().sprite = Resources.Load<Sprite>(refObj.instrumentPreviewIconPath);
            go.transform.SetParent(recommondItemGroup);
            InstrumentStoreRefObj storeRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
                .Find(x => x.instrumentId == targetIds[i]);
            totalHigh += storeRefObj.hightNoteNum;
            totalMiddle += storeRefObj.middleNoteNum;
            totalLow += storeRefObj.lowNoteNum;
            _itemList.Add(go);
        }

        txtHighAmount.text = "×" + totalHigh.ToString();
        txtMiddleAmount.text = "×" + totalMiddle.ToString();
        txtLowAmount.text = "×" + totalLow.ToString();
    }

    /// <summary>
    /// 根据index获取目标ID列表
    /// </summary>
    /// <param name="idList">原始ID列表</param>
    /// <param name="index">0表示取前半部分，1表示取后半部分</param>
    /// <returns>处理后的ID列表</returns>
    private List<long> GetTargetIds(List<long> idList, int index)
    {
        if (idList == null || idList.Count == 0)
            return new List<long>();

        int count = idList.Count;
        int halfCount = count / 2;

        if (index == 0)
        {
            // 取前半部分
            return idList.GetRange(0, halfCount);
        }
        else if (index == 1)
        {
            // 取后半部分
            return idList.GetRange(halfCount, count - halfCount);
        }
        else
        {
            // 如果index不是0或1，返回空列表或根据需求处理
            Debug.LogWarning($"Invalid index: {index}. Expected 0 or 1.");
            return new List<long>();
        }
    }
}