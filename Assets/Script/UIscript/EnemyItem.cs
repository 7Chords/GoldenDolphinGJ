using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyItem : UIPanelBase,IDamagable
{
    #region Mono
    public Image imgEnemyIcon;
    public Text txtAttack;
    public Text txtHealth;
    public Text txtName;
    public Image imgHealthBar;
    public Image imgHealthHolder;
    public Image imgHead;

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


    [Header("切换到敌人回合时等待多久播放装饰")]
    public float attackWaitDecDuration;

    [Header("切换到敌人回合时攻击前的等待时间")]
    public float attackWaitDuration;
    #endregion

    private EnemyInfo _enemyInfo;
    private TweenContainer _tweenContainer;
    private int _maxHealth;
    private bool _flag;
    protected override void OnShow()
    {
        MsgCenter.RegisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);
        _tweenContainer = new TweenContainer();
        BattleMgr.instance.RegEnemyItem(this);
    }

    protected override void OnHide(Action onHideFinished)
    {
        MsgCenter.UnregisterMsgAct(MsgConst.ON_TURN_CHG, OnTurnChg);

        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
        onHideFinished?.Invoke();
        BattleMgr.instance.UnregEnemyItem();
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
        imgEnemyIcon.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.enemyBodyPath);
        imgEnemyIcon.SetNativeSize();
        txtAttack.text = _enemyInfo.enemyAttack.ToString();
        txtName.text = _enemyInfo.enemyName;
        txtHealth.text = _enemyInfo.enemyHealth + "/" + _maxHealth;
        imgHealthBar.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHealthBarPath);
        imgHealthHolder.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHealthHolderPath);
        imgHead.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHeadPath);
        Tween healthTween = imgHealthBar.DOFillAmount((float)_enemyInfo.enemyHealth / _maxHealth, healthBarChgDuration);
        _tweenContainer.RegDoTween(healthTween);
    }

    public void Attack()
    {
        MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_END_ATTACK);
    }

    public void TakeDamage(int damage)
    {
        _enemyInfo.enemyHealth = Mathf.Clamp(_enemyInfo.enemyHealth - damage, 0, _maxHealth);
        RefreshShow();
        AudioMgr.Instance.PlaySfx("怪物受击cut");
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
    public void TakeHeal(int healAmount)
    {
        _enemyInfo.enemyHealth = Mathf.Clamp(_enemyInfo.enemyHealth + healAmount, 0, _maxHealth);
        RefreshShow();
    }

    public int GetHealAmount()
    {
        return 0;
    }

    public void TakeBuff(int buffAmount)
    {
        _enemyInfo.enemyAttack += buffAmount;
        RefreshShow();

    }
    public int GetBuffAmount()
    {
        return 0;
    }
    public void Dead()
    {
        MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_DEAD);
    }
    private void OnTurnChg()
    {
        if (_flag)
            return;
        _flag = true;
        if (BattleMgr.instance.curTurn == ETurnType.Enemy)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(DOVirtual.DelayedCall(attackWaitDecDuration, () =>
            {
                AudioMgr.Instance.PlaySfx("怪物攻击cut");
                MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_START_ATTACK);
            }));


            float extraTime = 0.1f;

            List<IDamagable> damagableList = new List<IDamagable>();
            foreach (var item in BattleMgr.instance.instrumentItemList)
            {
                damagableList.Add(item as IDamagable);
            }
            foreach (var item in damagableList)
            {
                if((item as InstrumentItem).instrumentInfo.skillRefList.
                    Find(x=>x.passiveSkillTriggerType == EPassiveSkillTriggerType.Hurt) != null)
                {
                    extraTime += 1.5f;
                }
            }

            seq.Append(DOVirtual.DelayedCall(attackWaitDuration - attackWaitDecDuration, () =>
             {

                 AttackHandler.EnemyDealAttack(EInstrumentEffectType.Attack, this, damagableList);
                 //MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_ACTION_OVER); 
                 // _flag = false;
             }));

            seq.Append(DOVirtual.DelayedCall(extraTime, () =>
            {
                MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_ACTION_OVER);
                _flag = false;
            }));

            _tweenContainer.RegDoTween(seq);
        }

        
    }
}
