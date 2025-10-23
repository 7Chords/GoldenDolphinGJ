using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击处理器
/// </summary>
public static class AttackHandler
{

    private static EInstrumentEffectType lastAttackEffectType = EInstrumentEffectType.None;
    private static int lastAttackValue = 0;
    private static List<IDamagable> lastAttackTargetList;
    public static void DealAttack(EInstrumentEffectType effectType, IDamagable attacker,List<IDamagable> targetList)
    {
        if (!BattleMgr.instance.gameStarted)
            return;
        attacker.Attack();

        EInstrumentEffectType curType;
        bool isCopy = false;
        List<IDamagable> curAttackTargetList;

        if (effectType == EInstrumentEffectType.CopyLast)
        {
            curType = lastAttackEffectType;
            isCopy = true;
            curAttackTargetList = lastAttackTargetList;
        }
        else
        {
            curType = effectType;
            curAttackTargetList = targetList;
        }

        switch (curType)
        {
            case EInstrumentEffectType.Attack:

                foreach (var target in curAttackTargetList)
                {
                    if (target == null)
                        continue;
                    if(isCopy)
                    {
                        target.TakeDamage(lastAttackValue);
                    }
                    else
                    {
                        target.TakeDamage(attacker.GetAttackAmount());
                        lastAttackValue = attacker.GetAttackAmount();
                        lastAttackEffectType = EInstrumentEffectType.Attack;
                    }

                }
                break;
            case EInstrumentEffectType.Heal:
                foreach (var target in curAttackTargetList)
                {
                    if (target == null)
                        continue;
                    if (isCopy)
                    {
                        target.TakeHeal(lastAttackValue);
                    }
                    else
                    {
                        target.TakeHeal(attacker.GetHealAmount());
                        lastAttackValue = attacker.GetHealAmount();
                        lastAttackEffectType = EInstrumentEffectType.Heal;
                    }
                }
                break;
            case EInstrumentEffectType.Buff:
                foreach (var target in curAttackTargetList)
                {
                    if (target == null)
                        continue;
                    if (isCopy)
                    {
                        target.TakeBuff(lastAttackValue);
                    }
                    else
                    {
                        target.TakeBuff(attacker.GetBuffAmount());
                        lastAttackValue = attacker.GetBuffAmount();
                        lastAttackEffectType = EInstrumentEffectType.Buff;
                    }
                }
                break;
            case EInstrumentEffectType.None:
                Debug.Log("无效的攻击类型！！！");
                break;
        }
    }
}
