using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 乐器信息类
/// </summary>
public class InstrumentInfo
{
    public EInstrumentType instrumentType;
    public EInstrumentEffectType effectType;
    public string instrumentName;
    public string instrumentDesc;
    public int health;
    public int attack;
    public int heal;
    public int buff;
    public string instrumentIconPath;
    public string instrumentBgPath;
    public string instrumentBodyPath;
    public string instrumentBodyBgPath;
    public string instrumentNamePath;
    public string instrumentBodyBgWithChaPath;
    public string instrumentAttackIconPath;
    public string instrumentAttackSoundPath;
    public string instrumentHurtSoundPath;
    public bool hasTogetherSkill;
    public int canUseSkillPoint;
    public List<long> canTogetherIdList;

    public InstrumentInfo(EInstrumentType instrumentType, EInstrumentEffectType effectType, string instrumentName, string instrumentDesc, int health, int attack, int heal, int buff, string instrumentIconPath, string instrumentBgPath, string instrumentBodyPath, string instrumentBodyBgPath, string instrumentNamePath, string instrumentBodyBgWithChaPath, string instrumentAttackIconPath, string instrumentAttackSoundPath, string instrumentHurtSoundPath, bool hasTogetherSkill, int canUseSkillPoint, List<long> canTogetherIdList)
    {
        this.instrumentType = instrumentType;
        this.effectType = effectType;
        this.instrumentName = instrumentName;
        this.instrumentDesc = instrumentDesc;
        this.health = health;
        this.attack = attack;
        this.heal = heal;
        this.buff = buff;
        this.instrumentIconPath = instrumentIconPath;
        this.instrumentBgPath = instrumentBgPath;
        this.instrumentBodyPath = instrumentBodyPath;
        this.instrumentBodyBgPath = instrumentBodyBgPath;
        this.instrumentNamePath = instrumentNamePath;
        this.instrumentBodyBgWithChaPath = instrumentBodyBgWithChaPath;
        this.instrumentAttackIconPath = instrumentAttackIconPath;
        this.instrumentAttackSoundPath = instrumentAttackSoundPath;
        this.instrumentHurtSoundPath = instrumentHurtSoundPath;
        this.hasTogetherSkill = hasTogetherSkill;
        this.canUseSkillPoint = canUseSkillPoint;
        this.canTogetherIdList = canTogetherIdList;
    }
}
