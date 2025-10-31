using DG.Tweening;
using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public struct InstrumentCost
{
    public ENoteType noteType;
    public int amount;

    public InstrumentCost(ENoteType noteType, int amount)
    {
        this.noteType = noteType;
        this.amount = amount;
    }
}

public class InstrumentDescItem : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image imgHead;
    public Image imgProp;
    public Text txtHealth;
    public Text txtProp;
    public GameObject goChgCheck;
    public Image imgChg_1;
    public Image imgChg_2;
    public Text txtChg_1;
    public Text txtChg_2;
    public Button btnItem;


    public List<GameObject> goIsCopyShowList;
    public List<GameObject> goNotCopyShowList;
    public GameObject goBounceShow;

    [Header("移入放大持续时间")]
    public float enterBiggerDuration;
    [Header("移入放大缩放")]
    public float enterBiggerScale;
    [Header("移出缩小持续时间")]
    public float exitSmallerDuration;
    [Header("移出缩小缩放")]
    public float exitSmallerScale;
    private TweenContainer _tweenContainer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _tweenContainer?.RegDoTween(transform.DOScale(enterBiggerScale, enterBiggerDuration));
        goChgCheck.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tweenContainer?.RegDoTween(transform.DOScale(exitSmallerScale, exitSmallerDuration));
        goChgCheck.SetActive(false);

    }

    public void SetInfo(InstrumentRefObj instrumentRefObj)
    {
        _tweenContainer = new TweenContainer();

        if(instrumentRefObj.effectType == EInstrumentEffectType.CopyLast)
        {
            foreach(var go in goIsCopyShowList)
            {
                go.SetActive(true);
            }
            foreach (var go in goNotCopyShowList)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (var go in goIsCopyShowList)
            {
                go.SetActive(false);
            }
            foreach (var go in goNotCopyShowList)
            {
                go.SetActive(true);
            }
        }
        if(instrumentRefObj.instrumentName == "长笛")
            goBounceShow.SetActive(true);
        else
            goBounceShow.SetActive(false);

        imgHead.sprite = Resources.Load<Sprite>(instrumentRefObj.instrumentPreviewIconPath);
        imgProp.sprite = Resources.Load<Sprite>(instrumentRefObj.instrumentAttackIconPath);
        txtHealth.text = instrumentRefObj.health.ToString();
        switch (instrumentRefObj.effectType)
        {
            case EInstrumentEffectType.Attack:
                txtProp.text = instrumentRefObj.attack.ToString();
                break;
            case EInstrumentEffectType.Heal:
                txtProp.text = instrumentRefObj.heal.ToString();
                break;
            case EInstrumentEffectType.Buff:
                txtProp.text = instrumentRefObj.buff.ToString();
                break;
            case EInstrumentEffectType.CopyLast:
                break;
        }
        goChgCheck.SetActive(false);
        InstrumentStoreRefObj storeRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
            .Find(x => x.instrumentId == instrumentRefObj.id);

        List<InstrumentCost> costList = new List<InstrumentCost>();
        if (storeRefObj.hightNoteNum > 0)
            costList.Add(new InstrumentCost(ENoteType.High, storeRefObj.hightNoteNum));
        if (storeRefObj.middleNoteNum > 0)
            costList.Add(new InstrumentCost(ENoteType.Middle, storeRefObj.middleNoteNum));
        if (storeRefObj.lowNoteNum > 0)
            costList.Add(new InstrumentCost(ENoteType.Low, storeRefObj.lowNoteNum));

        if (costList.Count == 1)
        {
            imgChg_2.gameObject.SetActive(false);
            txtChg_2.text = "";
        }

        for (int i =0;i<costList.Count;i++)
        {
            if (i == 0)
            {
                imgChg_1.sprite = GetNoteSprite(costList[i].noteType);
                txtChg_1.text = "×" + costList[i].amount.ToString();
            }
            else if(i == 1)
            {
                imgChg_2.sprite = GetNoteSprite(costList[i].noteType);
                txtChg_2.text = "×" + costList[i].amount.ToString();

            }
        }
    }

    private Sprite GetNoteSprite(ENoteType noteType)
    {
        switch (noteType)
        {
            case ENoteType.High:
                return Resources.Load<Sprite>("UI/Icon/高音符");
            case ENoteType.Middle:
                return Resources.Load<Sprite>("UI/Icon/中音符");
            case ENoteType.Low:
                return Resources.Load<Sprite>("UI/Icon/低音符");
            default:
                return null;
        }

    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }

}
