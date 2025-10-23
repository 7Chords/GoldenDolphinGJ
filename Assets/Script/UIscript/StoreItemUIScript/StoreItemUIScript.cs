using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUIScript : MonoBehaviour
{
    [SerializeField] private Text roleText;
    [SerializeField] private Text attackOrBufferText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text desc;
    [SerializeField] private Text name;
    [SerializeField] private Text totalText;
    public void SetInfo(long instrumentId)
    {
        InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
            .Find(x => x.id == instrumentId);

        if(instrumentRefObj == null)
        {
            Debug.Log("instrumentRefObj is Null");
            return;
        }

        roleText.text = "  定位：" + GetRoleTypeString(instrumentRefObj.instrumentRoleTypeList);
        int attackValue = instrumentRefObj.attack + instrumentRefObj.buff + instrumentRefObj.heal;
        attackOrBufferText.text = "  攻击/增益：" + attackValue.ToString();
        int health = instrumentRefObj.health;
        healthText.text = "  生命：" + health.ToString();
        desc.text = "  描述：" + instrumentRefObj.instrumentDesc + "  ";
        name.text = "  名字：" + instrumentRefObj.instrumentName;

        string totalDesc = "\n" + name.text + "\n" + roleText.text + "\n" + attackOrBufferText.text + "\n" + healthText.text + "\n" + desc.text;
        totalText.text = totalDesc + "\n";
    }

    private string GetRoleTypeString(List<EInstrumentRoleTypeList> instrumentRoleTypeList)
    {
        string roleTypeStr = "";
        int tempCount = 0;
        for (int i = 0; i < instrumentRoleTypeList.Count; i++)
        {
            // 标志位
            int flag = tempCount;

            switch (instrumentRoleTypeList[i])
            {
                case EInstrumentRoleTypeList.DamageDealer:
                    roleTypeStr += "攻击 ";
                    tempCount++;
                    break;
                case EInstrumentRoleTypeList.Support:
                    roleTypeStr += "辅助 ";
                    tempCount++;
                    break;
                case EInstrumentRoleTypeList.Healer:
                    roleTypeStr += "治疗 ";
                    tempCount++;
                    break;
                case EInstrumentRoleTypeList.Buffer:
                    roleTypeStr += "增益 ";
                    tempCount++;
                    break;
            }
            if(flag != tempCount && tempCount < instrumentRoleTypeList.Count)
            {
                roleTypeStr += "- ";
            }
        }
        return roleTypeStr;
    }
}
