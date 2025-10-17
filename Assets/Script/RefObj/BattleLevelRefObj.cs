using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevelRefObj : SCRefDataCore
{
    public long id;
    public EEnemyType enemyType;
    public int enemyHealth;
    public int enemyAttack;
    public string enemyIconPath;
    public string enemyBgPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        enemyType = (EEnemyType)getEnum("enemyType", typeof(EEnemyType));
        enemyHealth = getInt("enemyHealth");
        enemyAttack = getInt("enemyAttack");
        enemyIconPath = getString("enemyIconPath");
        enemyBgPath = getString("enemyBgPath");

    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "battle_level";
}
