using GJFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyItem : UIPanelBase
{
    public Image imgEnemyBg;
    public Image imgEnemyIcon;

    private EnemyInfo _enemyInfo;
    protected override void OnShow()
    {
    }

    protected override void OnHide(Action onHideFinished)
    {
    }

    public void SetInfo(EnemyInfo enemInfo)
    {
        _enemyInfo = enemInfo;

    }
}
