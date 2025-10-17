using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevelRefObj : SCRefDataCore
{
    public long id;
    public int level;
    public EEnemyType enemyType;
    public string enemyName;
    public int enemyHealth;
    public int enemyAttack;
    public string enemyIconPath;
    public string enemyBgPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        level = getInt("level");
        enemyType = (EEnemyType)getEnum("enemyType", typeof(EEnemyType));
        enemyName = getString("enemyName");
        enemyHealth = getInt("enemyHealth");
        enemyAttack = getInt("enemyAttack");
        enemyIconPath = getString("enemyIconPath");
        enemyBgPath = getString("enemyBgPath");

    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "battle_level";
}
