using DG.Tweening;
using GJFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
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
    public Text txtEnemyDesc;

    public List<Color> levelColorList;


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

    private bool _isSecondStage;//是否处于第二阶段
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
        _isSecondStage = false;
        _enemyInfo = enemInfo;
        _maxHealth = _enemyInfo.enemyHealth;
        RefreshShow();
        switch(_enemyInfo.enemyName)
        {
            case "变异十六分音符":
                EnemyContinueEffectHandler.RegContinueEffect(EEnemyActionType.Heal, 3, (int)(_maxHealth * 0.2f));
                break;
            case "变异升号音符":
                EnemyContinueEffectHandler.RegContinueEffect(EEnemyActionType.Heal, 3, (int)(_maxHealth * 0.2f));
                break;
            default:
                break;
        }
    }

    private void RefreshShow()
    {
        if (_enemyInfo == null)
            return;
        imgEnemyIcon.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.enemyBodyPath);
        txtAttack.text = AddColorForRichText(_enemyInfo.enemyAttack.ToString(), levelColorList[GameMgr.Instance.curLevel - 1]);
        txtName.text = AddColorForRichText(_enemyInfo.enemyName, levelColorList[GameMgr.Instance.curLevel - 1]);
        txtHealth.text = AddColorForRichText(_enemyInfo.enemyHealth + "/" + _maxHealth, levelColorList[GameMgr.Instance.curLevel - 1]);
        imgHealthBar.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHealthBarPath);
        imgHealthHolder.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHealthHolderPath);
        imgHead.sprite = Resources.Load<Sprite>(_enemyInfo.enemyResRefObj.levelEnemyHeadPath);

        if(GameMgr.Instance.curLevel == 3)
        {
            string[] splitDescArr = _enemyInfo.enemyDesc.Split(";");
            txtEnemyDesc.text = AddColorForRichText(_isSecondStage? splitDescArr[1]: splitDescArr[0],
                levelColorList[GameMgr.Instance.curLevel - 1]);
        }
        else
        {
            txtEnemyDesc.text = AddColorForRichText(_enemyInfo.enemyDesc,
                levelColorList[GameMgr.Instance.curLevel - 1]);
        }
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
        if(_enemyInfo.enemyName == "变异升号音符" && !_isSecondStage)
        {
            _isSecondStage = true;
            _enemyInfo.enemyHealth = _maxHealth;
            RefreshShow();
            EnemyContinueEffectHandler.RegContinueEffect(EEnemyActionType.Buff, 1, 8);

        }
        else
        {
            MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_DEAD);
        }
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
                EnemyContinueEffectHandler.TickContinueEffect();
            }));


            float extraTime = 0.1f;

            List<IDamagable> damagableList = new List<IDamagable>();


            if(_enemyInfo.enemyName == "变异升号音符" && _isSecondStage)
            {
                InstrumentItem item = BattleMgr.instance.instrumentItemList[Random.Range(0, BattleMgr.instance.instrumentItemList.Count)];
                while(item.hasDead)
                {
                    item = BattleMgr.instance.instrumentItemList[Random.Range(0, BattleMgr.instance.instrumentItemList.Count)];
                }
                damagableList.Add(item);
            }
            else
            {
                foreach (var item in BattleMgr.instance.instrumentItemList)
                {
                    damagableList.Add(item as IDamagable);
                }
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
             }));

            seq.Append(DOVirtual.DelayedCall(extraTime, () =>
            {
                MsgCenter.SendMsgAct(MsgConst.ON_ENEMY_ACTION_OVER);
                _flag = false;
            }));

            _tweenContainer.RegDoTween(seq);
        }

        
    }

    private string ColorToString(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    public string AddColorForRichText(string txt, Color color)
    {
        string richTextColor = "#" + ColorToString(color);
        return string.Format("<color={0}>{1}</color>", richTextColor, txt);
    }
}
