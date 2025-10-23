using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能处理器
/// </summary>
public static class SkillHandler
{
    public static void DealTogetherSkill(List<IDamagable> senderList,SkillRefObj skillRefObj)
    {
        if (senderList == null
            || senderList.Count == 0
            || skillRefObj == null)
            return;

        if (skillRefObj.skillType != ESkillType.Together)
            return;

        switch(skillRefObj.skillName)
        {
            case "合击1":
                break;
            case "合击2":
                break;
            case "合击3":
                break;
            case "合击4":
                break;
            default:
                break;
        }
    }

    public static void DealSingleSkill()
    {

    }

    public static void DealPassiveSkill()
    {

    }


}
