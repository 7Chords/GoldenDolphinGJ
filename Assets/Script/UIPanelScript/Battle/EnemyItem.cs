using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyItem : UIPanelBase,IDamagable
{
    #region Mono
    public Image imgEnemyBg;
    public Image imgEnemyIcon;
    public Text txtAttack;
    //public Text txtHealth;
    public Text txtName;
    public Image imgHealthBar;

    [Header("受伤震动强度")]
    public float hurtShakeStrength;
    [Header("受伤震动时间")]
    public float hurtShakeDuration;
    [Header("受伤颜色")]
    public Color hurtColor;
    [Header("受伤颜色变化时间")]
    public float hurtColorFadeDuration;
    [Header("血条变化时间")]
    public float healthBarChgDuration;

    [Header("切换到敌人回合时攻击前的等待时间")]
    public float attackWaitDuration;
    #endregion

    private EnemyInfo _enemyInfo;
    private TweenContainer _tweenContainer;
    private int _maxHealth;
    protected override void OnShow()
    {
        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        _tweenContainer = new TweenContainer();
    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);

        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
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
        txtName.text = _enemyInfo.enemyName;
        Tween healthTween = imgHealthBar.DOFillAmount((float)_enemyInfo.enemyHealth / _maxHealth, healthBarChgDuration);
        _tweenContainer.RegDoTween(healthTween);
    }

    public void Attack()
    {

    }

    public void TakeDamage(int damage)
    {
        _enemyInfo.enemyHealth = Mathf.Clamp(_enemyInfo.enemyHealth - damage, 0, _maxHealth);
        RefreshShow();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(hurtShakeDuration, hurtShakeStrength, fadeOut: true));
        seq.Join(imgEnemyIcon.DOColor(hurtColor, hurtColorFadeDuration / 2));
        seq.Append(imgEnemyIcon.DOColor(Color.white, hurtColorFadeDuration / 2));
        _tweenContainer.RegDoTween(seq);

        if (_enemyInfo.enemyHealth == 0)
            Dead();
    }

    public int GetAttackAmount()
    {
        return _enemyInfo.enemyAttack;
    }

    public void Dead()
    {

    }
    private void OnTurnChg()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(attackWaitDuration).OnComplete(() =>
        {
            List<IDamagable> damagableList = new List<IDamagable>();
            foreach(var item in BattleMgr.instance.instrumentItemList)
            {
                damagableList.Add(item as IDamagable);
            }
            AttackHandler.DealAttack(this, damagableList);
        });
        _tweenContainer.RegDoTween(seq);
        
    }
}
