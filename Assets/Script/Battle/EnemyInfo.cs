using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    public EEnemyType enemyType;
    public string enemyName;
    public int enemyHealth;
    public int enemyAttack;
    public string enemyIconPath;
    public string enemyBgPath;

    public EnemyInfo(EEnemyType enemyType, string enemyName,int enemyHealth, int enemyAttack, string enemyIconPath, string enemyBgPath)
    {
        this.enemyType = enemyType;
        this.enemyName = enemyName;
        this.enemyHealth = enemyHealth;
        this.enemyAttack = enemyAttack;
        this.enemyIconPath = enemyIconPath;
        this.enemyBgPath = enemyBgPath;
    }
}
