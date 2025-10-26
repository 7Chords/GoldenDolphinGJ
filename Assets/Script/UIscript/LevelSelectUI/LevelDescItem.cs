using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDescItem : MonoBehaviour
{

    public Image imgBg;
    public Image imgPoint;
    public Image imgEnemyName;
    public Text txtEnemyDesc;
    public Text txtEnemyHealth;
    public Text txtEnemyAttack;

    public Transform instrumentSVTransform;
    public GameObject instrumentDescItemPrefab;

    private List<GameObject> _itemList;
    public List<RecommendItem> recommendItemList;
    public void SetInfo(BattleLevelRefObj levelRefObj)
    {
        if (_itemList == null)
            _itemList = new List<GameObject>();
        else
        {
            foreach(var go in _itemList)
            {
                Destroy(go);
            }
            _itemList.Clear();
        }    

        imgBg.sprite = Resources.Load<Sprite>(levelRefObj.levelPreviewBgPath);
        imgPoint.sprite = Resources.Load<Sprite>(levelRefObj.levelPreviewPointDecPath);
        imgEnemyName.sprite = Resources.Load<Sprite>(levelRefObj.levelEnemyNameImgPath);
        imgEnemyName.SetNativeSize();
        txtEnemyHealth.text = levelRefObj.enemyHealth.ToString();
        txtEnemyAttack.text = levelRefObj.enemyAttack.ToString();


        List<InstrumentRefObj> instrumentRefList = SCRefDataMgr.Instance.instrumentRefList.refDataList;
        List<InstrumentRefObj> resRefList = new List<InstrumentRefObj>();
        for (int i = 0;i< instrumentRefList.Count;i++)
        {
            if(instrumentRefList[i].unlockLevelId <= levelRefObj.level)
            {
                resRefList.Add(instrumentRefList[i]);
            }
        }
        for (int i = 0; i < resRefList.Count; i++)
        {
            GameObject instruemntItem = GameObject.Instantiate(instrumentDescItemPrefab);
            instruemntItem.transform.SetParent(instrumentSVTransform);
            instruemntItem.GetComponent<InstrumentDescItem>().SetInfo(resRefList[i]);
            _itemList.Add(instruemntItem);
        }

        if(recommendItemList!= null)
        {
            foreach(var item in recommendItemList)
            {
                item.SetInfo(levelRefObj);
            }
        }
    }
}
