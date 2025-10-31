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
        EnemyResRefObj enemyResRefObj = SCRefDataMgr.Instance.enemyResRefList.refDataList.Find(x => x.id == levelRefObj.enemyResId);
        imgBg.sprite = Resources.Load<Sprite>(enemyResRefObj.levelPreviewBgPath);
        imgPoint.sprite = Resources.Load<Sprite>(enemyResRefObj.levelPreviewPointDecPath);
        imgEnemyName.sprite = Resources.Load<Sprite>(enemyResRefObj.levelEnemyNameImgPath);
        imgEnemyName.SetNativeSize();
        txtEnemyHealth.text = levelRefObj.enemyHealth.ToString();
        txtEnemyAttack.text = levelRefObj.enemyAttack.ToString();
        if(levelRefObj.level == 3)
        {
            string str = "";
            string[] descArr = levelRefObj.enemyDesc.Split(";");
            str += descArr[0] + descArr[1];
            txtEnemyDesc.text = str;
        }
        else
        {
            txtEnemyDesc.text = levelRefObj.enemyDesc;
        }

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
            GameObject instruemntItem = GameObject.Instantiate(instrumentDescItemPrefab, instrumentSVTransform);
            //instruemntItem.transform.SetParent(instrumentSVTransform);
            instruemntItem.GetComponent<InstrumentDescItem>().SetInfo(resRefList[i]);
            _itemList.Add(instruemntItem);
        }

        if(recommendItemList!= null)
        {
            for(int i =0;i< recommendItemList.Count;i++)
            {
                recommendItemList[i].SetInfo(levelRefObj, i);
            }
        }
    }

    public void Hide()
    {
        if(_itemList != null)
        {
            foreach (var go in _itemList)
            {
                Destroy(go);
            }
            _itemList.Clear();
        }
    }
}
