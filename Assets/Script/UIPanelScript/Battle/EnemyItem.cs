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
    public Text txtAttack;
    public Text txtHealth;
    public Text txtName;

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
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_enemyInfo == null)
            return;
        txtAttack.text = _enemyInfo.enemyAttack.ToString();
        txtHealth.text = _enemyInfo.enemyHealth.ToString();
        txtName.text = _enemyInfo.enemyName;
    }
}
