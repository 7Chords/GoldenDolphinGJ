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
    public List<long> recommendinstrumentsIdList;
    public List<EEnemyActionType> enemyActionTypeList;
    public string bgmName;
    public long collectPageSkinId;// 换皮id
    public long enemyResId;
    public string enemyDesc;
    public long resultSkinId;
    public long StorePageSkinId;
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
        recommendinstrumentsIdList = getList<long>("recommendinstrumentsIdList");
        enemyActionTypeList = getList<EEnemyActionType>("enemyActionTypeList");
        bgmName = getString("bgmName");
        collectPageSkinId = getLong("collectPageSkinId");
        enemyResId = getLong("enemyResId");
        enemyDesc = getString("enemyDesc");
        resultSkinId = getLong("resultSkinId");
        StorePageSkinId = getLong("StorePageSkinId");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "battle_level";
}
