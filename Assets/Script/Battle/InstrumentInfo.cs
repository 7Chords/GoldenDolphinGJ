using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 乐器信息类
/// </summary>
public class InstrumentInfo
{
    public int health;
    public int attack;
    public int heal;
    public int buff;
    public int skillPoint;

    public InstrumentRefObj refObj;
    public List<SkillRefObj> skillRefList;
    public InstrumentResRefObj resRefObj;
    public InstrumentInfo(InstrumentRefObj refObj)
    {
        this.refObj = refObj;
        health = refObj.health;
        attack = refObj.attack;
        heal = refObj.heal;
        buff = refObj.buff;
        skillPoint = 0;

        skillRefList = new List<SkillRefObj>();
        SkillRefObj tmpRefObj = null;
        for (int i = 0; i < refObj.skillIdList.Count; i++)
        {
            tmpRefObj = SCRefDataMgr.Instance.skillRefList.refDataList.Find(x => x.id ==
                refObj.skillIdList[i]);
            if (tmpRefObj != null)
                skillRefList.Add(tmpRefObj);
        }
        resRefObj = SCRefDataMgr.Instance.instrumentResRefList.refDataList.
            Find(x => x.id == refObj.resSkinIdList[GameMgr.Instance.curLevel - 1]);
    }
}
