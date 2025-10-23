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

    public static void DealSingleSkill(IDamagable sender, SkillRefObj skillRefObj)
    {

    }

    public static void DealPassiveSkill(IDamagable sender, SkillRefObj skillRefObj, params object[] objs)
    {
        if (sender == null || skillRefObj == null)
            return;
        switch(skillRefObj.skillName)
        {
            case "反伤":
                (sender as InstrumentItem).BounceAttack(
                ()=> 
                {
                    int damage = (int)objs[0];
                    EnemyItem enemyItem = BattleMgr.instance.enemyItem;
                    if (enemyItem != null)
                    {
                        enemyItem.TakeDamage(damage / 2);
                    }
                });
                break;
            default:
                break;
        }
    }


}
