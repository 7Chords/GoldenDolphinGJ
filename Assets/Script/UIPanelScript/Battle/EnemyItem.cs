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
    //public Text txtHealth;
    public Text txtName;
    public Image imgHealthBar;

    private EnemyInfo _enemyInfo;

    private int _maxHealth;
    protected override void OnShow()
    {
    }

    protected override void OnHide(Action onHideFinished)
    {
    }

    public void SetInfo(EnemyInfo enemInfo)
    {
        _enemyInfo = enemInfo;
        _maxHealth = _enemyInfo.enemyHealth;
        RefreshShow();
    }

    private void RefreshShow()
    {
        if (_enemyInfo == null)
            return;
        imgEnemyBg.sprite = Resources.Load<Sprite>(_enemyInfo.enemyBgPath);
        imgEnemyIcon.sprite = Resources.Load<Sprite>(_enemyInfo.enemyIconPath);
        txtAttack.text = _enemyInfo.enemyAttack.ToString();
        //txtHealth.text = _enemyInfo.enemyHealth.ToString();
        txtName.text = _enemyInfo.enemyName;
        imgHealthBar.fillAmount = (float)_enemyInfo.enemyHealth / _maxHealth;
    }
}
