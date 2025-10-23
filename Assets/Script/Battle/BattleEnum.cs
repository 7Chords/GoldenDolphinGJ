public enum EInstrumentType
{
    Harp,// 竖琴
    Violin,// 提琴
    Trumpet,// 小号
    Accordion,//手风琴
    Clarinet,//单簧管
    Flute,//长笛
    Triangle,//三角铁
}

public enum ETurnType 
{
    Player,
    Enemy,
}

public enum EEnemyType
{
    MonsterNote,
}

public enum EInstrumentEffectType
{
    None,
    Attack,
    Heal,
    Buff,
    CopyLast,
}

public enum EEnemyActionType
{
    Attack,
    Heal,
    Buff,
}


public enum EBattleDecorationType
{
    HighFreq,
    MiddleFreq,
    LowFreq,
}

public enum EInstrumentRoleTypeList
{
    Support, // 辅助
    Healer,  // 治疗
    Buffer,  // 增益
    DamageDealer // 输出
}

public enum ESkillType
{
    Single,//个人
    Together,//合击
    Passive,//被动
}

//被动技能触发时机类型
public enum EPassiveSkillTriggerType
{
    None,
    Together,
    Hurt,
    TurnStart,
    TurnEnd,
    Attack,
}