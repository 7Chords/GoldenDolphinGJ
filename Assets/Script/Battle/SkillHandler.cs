using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能处理器
/// </summary>
public static class SkillHandler
{

    public static float bounceRate = 0.2f; 
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
            case "小号和小提琴的合击":
                {
                    int attackerDamage = 0;
                    int totalDamage = 0;
                    for(int i =0;i<senderList.Count;i++)
                    {
                        if((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Attack)
                        {
                            attackerDamage = (senderList[i] as InstrumentItem).instrumentInfo.attack;
                            totalDamage += attackerDamage;
                        }
                        else if((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Buff)
                        {
                            totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.buff;
                        }
                    }
                    totalDamage += (int)(attackerDamage * 0.5f);
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                }
                break;
            case "小号和手风琴的合击":
                {
                    int attackerDamage = 0;
                    int totalDamage = 0;
                    for (int i = 0; i < senderList.Count; i++)
                    {
                        if ((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Attack)
                        {
                            attackerDamage = (senderList[i] as InstrumentItem).instrumentInfo.attack;
                            totalDamage += attackerDamage;
                        }
                        else if ((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Buff)
                        {
                            totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.buff;
                        }
                    }
                    totalDamage += (int)(attackerDamage * 0.5f);
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                }
                break;
            case "小号和单簧管的合击":
                {
                    int attackerDamage = 0;
                    int totalDamage = 0;
                    for (int i = 0; i < senderList.Count; i++)
                    {
                        if ((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Attack)
                        {
                            attackerDamage = (senderList[i] as InstrumentItem).instrumentInfo.attack;
                            totalDamage += attackerDamage;
                        }
                        else if ((senderList[i] as InstrumentItem).instrumentInfo.refObj.effectType == EInstrumentEffectType.Buff)
                        {
                            totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.buff;
                        }
                    }
                    totalDamage += (int)(attackerDamage * 0.5f);
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                }
                break;
            case "长笛和小提琴的合击":
                {
                    int totalDamage = 0;
                    for (int i = 0; i < senderList.Count; i++)
                    {
                        totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.attack;
                    }
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                    bounceRate = 0.4f;
                }
                break;
            case "长笛和手风琴的合击":
                {
                    int totalDamage = 0;
                    for (int i = 0; i < senderList.Count; i++)
                    {
                        totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.attack;
                    }
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                    bounceRate = 0.4f;

                }
                break;
            case "长笛和单簧管的合击":
                {
                    int totalDamage = 0;
                    for (int i = 0; i < senderList.Count; i++)
                    {
                        totalDamage += (senderList[i] as InstrumentItem).instrumentInfo.attack;
                    }
                    BattleMgr.instance.enemyItem.TakeDamage(totalDamage);
                    bounceRate = 0.4f;

                }
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
                        enemyItem.TakeDamage((int)(damage * bounceRate));
                    }
                    bounceRate = 0.2f;
                });
                break;
            default:
                break;
        }
    }


}
