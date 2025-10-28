using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    public EEnemyType enemyType;
    public string enemyName;
    public int enemyHealth;
    public int enemyAttack;
    public string enemyDesc;

    public EnemyResRefObj enemyResRefObj;
    public EnemyInfo(EEnemyType enemyType, string enemyName, int enemyHealth, int enemyAttack,long enemyResId, string enemyDesc)
    {
        this.enemyType = enemyType;
        this.enemyName = enemyName;
        this.enemyHealth = enemyHealth;
        this.enemyAttack = enemyAttack;
        this.enemyDesc = enemyDesc;
        enemyResRefObj = SCRefDataMgr.Instance.enemyResRefList.refDataList.Find(x => x.id == enemyResId);
    }
}
