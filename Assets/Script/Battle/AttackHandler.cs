using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击处理器
/// </summary>
public static class AttackHandler
{
    public static void DealAttack(IDamagable attacker,IDamagable target)
    {
        attacker.Attack();
        target.TakeDamage(attacker.GetAttackAmount());

        Debug.Log("Attack");
    }
}
