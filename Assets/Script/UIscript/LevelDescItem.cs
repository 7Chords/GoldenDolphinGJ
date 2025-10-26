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

    public List<GameObject> itemList;
    public void SetInfo(BattleLevelRefObj levelRefObj)
    {
        if (itemList == null)
            itemList = new List<GameObject>();
        else
        {
            foreach(var go in itemList)
            {
                Destroy(go);
            }
            itemList.Clear();
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
            itemList.Add(instruemntItem);
        }
    }
}
