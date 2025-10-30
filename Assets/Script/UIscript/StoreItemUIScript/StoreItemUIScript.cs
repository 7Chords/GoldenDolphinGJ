using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUIScript : MonoBehaviour
{
    [SerializeField] private Text effectText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text desc;
    [SerializeField] private Image effctImage;// 如果无攻击力就隐藏effct文本
    [SerializeField] private Image charactorImage;
    [SerializeField] private Image nameImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image lineImage;
    public void SetInfo(long instrumentId)
    {
        InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
            .Find(x => x.id == instrumentId);

        if(instrumentRefObj == null)
        {
            Debug.Log("instrumentRefObj is Null");
            return;
        }

        desc.text = instrumentRefObj.instrumentDesc;
        healthText.text = instrumentRefObj.health.ToString();
        SetInfoByEffectType(instrumentRefObj);
        nameImage.sprite = Resources.Load<Sprite>(instrumentRefObj.instrumentNamePath);
        nameImage.SetNativeSize();
        nameImage.rectTransform.localScale = Vector3.one * 0.85f;
        charactorImage.sprite = Resources.Load<Sprite>(instrumentRefObj.characterStoreHeadPath);
    }

    private void SetInfoByEffectType(InstrumentRefObj instrumentRefObj)
    {
        string path = "UI/EffectIcon/";

        EInstrumentEffectType type = instrumentRefObj.effectType;
        switch (type)
        {
            case EInstrumentEffectType.Heal:
                effectText.text = instrumentRefObj.heal.ToString();
                break;
            case EInstrumentEffectType.Attack:
                effectText.text = instrumentRefObj.attack.ToString();
                break;
            case EInstrumentEffectType.Buff:
                effectText.text = instrumentRefObj.buff.ToString();
                break;
            case EInstrumentEffectType.CopyLast:
                effectText.text = "";
                break;
            default:
                effctImage.enabled = false;
                break;
        }

        effctImage.sprite = Resources.Load<Sprite>(path + instrumentRefObj.effectTypeIconPath);
    }

    
}
