using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContinueEffectInfo
{
    public EEnemyActionType effectType;
    public int triggerTurn;
    public int amount;

    public ContinueEffectInfo(EEnemyActionType effectType, int triggerTurn,int amount)
    {
        this.effectType = effectType;
        this.triggerTurn = triggerTurn;
        this.amount = amount;
    }
}
public static class EnemyContinueEffectHandler
{
    public static ContinueEffectInfo curContinueEffectInfo;
    public static int effectCounter;

    public static void RegContinueEffect(EEnemyActionType effectType ,int triggerTurn,int amount)
    {
        curContinueEffectInfo = new ContinueEffectInfo(effectType, triggerTurn, amount);
        effectCounter = 0;
    }

    public static void TickContinueEffect()
    {
        if (curContinueEffectInfo == null)
            return;
        effectCounter++;
        if(effectCounter == curContinueEffectInfo.triggerTurn)
        {
            switch(curContinueEffectInfo.effectType)
            {
                case EEnemyActionType.Attack://没有处理
                    break;
                case EEnemyActionType.Heal:
                    BattleMgr.instance.enemyItem.TakeHeal(curContinueEffectInfo.amount);
                    break;
                case EEnemyActionType.Buff:
                    BattleMgr.instance.enemyItem.TakeBuff(curContinueEffectInfo.amount);
                    break;
            }
            effectCounter = 0;
        }
    }

    public static void UnregContinueEffect()
    {
        curContinueEffectInfo = null;
        effectCounter = 0;
    }

}
