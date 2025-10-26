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
    public int enemyBuff;
    public int enemyHeal;

    public string enemyIconPath;
    public string enemyBgPath;
    public List<long> recommendinstrumentsIdList = new List<long>();// 推荐使用的乐器ID列表
    public string enemyBodyPath;
    public List<EEnemyActionType> enemyActionTypeList;
    public string bgmName;

    public string levelPreviewBgPath;
    public string levelPreviewPointDecPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        level = getInt("level");
        enemyType = (EEnemyType)getEnum("enemyType", typeof(EEnemyType));
        enemyName = getString("enemyName");
        enemyHealth = getInt("enemyHealth");
        enemyAttack = getInt("enemyAttack");
        enemyBuff = getInt("enemyBuff");
        enemyHeal = getInt("enemyHeal");
        enemyIconPath = getString("enemyIconPath");
        enemyBgPath = getString("enemyBgPath");
        recommendinstrumentsIdList = getList<long>("recommendinstrumentsIdList");
        enemyBodyPath = getString("enemyBodyPath");
        enemyActionTypeList = getList<EEnemyActionType>("enemyActionTypeList");
        bgmName = getString("bgmName");
        levelPreviewBgPath = getString("levelPreviewBgPath");
        levelPreviewPointDecPath = getString("levelPreviewPointDecPath");

    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "battle_level";
}
