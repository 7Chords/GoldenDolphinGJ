using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击处理器
/// </summary>
public static class AttackHandler
{
    public static void DealAttack(IDamagable attacker,List<IDamagable> targetList)
    {
        attacker.Attack();
        foreach (var target in targetList)
        {
            if (target == null)
                continue;
            target.TakeDamage(attacker.GetAttackAmount());
        }

        Debug.Log("Attack");
    }
}
