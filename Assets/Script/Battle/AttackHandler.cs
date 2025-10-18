using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击处理器
/// </summary>
public static class AttackHandler
{
    public static void DealAttack(EInstrumentEffectType effectType, IDamagable attacker,List<IDamagable> targetList)
    {
        attacker.Attack();

        switch(effectType)
        {
            case EInstrumentEffectType.Attack:
                foreach (var target in targetList)
                {
                    if (target == null)
                        continue;
                    target.TakeDamage(attacker.GetAttackAmount());
                }
                break;
            case EInstrumentEffectType.Heal:
                foreach (var target in targetList)
                {
                    if (target == null)
                        continue;
                    target.TakeHeal(attacker.GetHealAmount());
                }
                break;
            case EInstrumentEffectType.Buff:
                foreach (var target in targetList)
                {
                    if (target == null)
                        continue;
                    target.TakeBuff(attacker.GetBuffAmount());
                }
                break;
        }

        Debug.Log("Attack");
    }
}
