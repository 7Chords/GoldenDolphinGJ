using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResRefObj : SCRefDataCore
{
    public long id;
    public string enemyBodyPath;
    public string levelPreviewBgPath;
    public string levelPreviewPointDecPath;
    public string levelEnemyNameImgPath;
    public string levelBgPath;
    public string levelBottomBgPath;
    public string levelTopBgPath;
    public string levelEnemyHealthHolderPath;
    public string levelEnemyHealthBarPath;
    public string levelEnemyHeadPath;
    public string levelExitBtnPath;
    public string levelStarPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        enemyBodyPath = getString("enemyBodyPath");
        levelPreviewBgPath = getString("levelPreviewBgPath");
        levelPreviewPointDecPath = getString("levelPreviewPointDecPath");
        levelEnemyNameImgPath = getString("levelEnemyNameImgPath");
        levelBgPath = getString("levelBgPath");
        levelBottomBgPath = getString("levelBottomBgPath");
        levelTopBgPath = getString("levelTopBgPath");
        levelEnemyHealthHolderPath = getString("levelEnemyHealthHolderPath");
        levelEnemyHealthBarPath = getString("levelEnemyHealthBarPath");
        levelEnemyHeadPath = getString("levelEnemyHeadPath");
        levelExitBtnPath = getString("levelExitBtnPath");
        levelStarPath = getString("levelStarPath");

    }
    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "enemy_res";

}
