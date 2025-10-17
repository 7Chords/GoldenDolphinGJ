using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    public EEnemyType enemyType;
    public int enemyHealth;
    public int enemyAttack;
    public string enemyIconPath;
    public string enemyBgPath;

    public EnemyInfo(EEnemyType enemyType, int enemyHealth, int enemyAttack, string enemyIconPath, string enemyBgPath)
    {
        this.enemyType = enemyType;
        this.enemyHealth = enemyHealth;
        this.enemyAttack = enemyAttack;
        this.enemyIconPath = enemyIconPath;
        this.enemyBgPath = enemyBgPath;
    }
}
