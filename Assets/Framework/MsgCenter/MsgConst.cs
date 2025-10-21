namespace GJFramework
{
    /// <summary>
    /// 消息常量类 id不要重复！！！
    /// </summary>
    public class MsgConst
    {
        public const int ON_TRANSITION_IN = 1001; // 场景切换淡入结束
        public const int ON_TRANSITION_OUT = 1002; // 场景切换淡出结束

        public const int ON_BATTLE_START = 1003;
        public const int ON_BATTLE_TURN_CHG = 1004;

        public const int ON_NOTE_COUNT_CHANGE = 1005;// 音符的数量变更了

        public const int ON_INSTRUMENT_ACTION_OVER = 1006;//乐器行动结束
        public const int ON_ENEMY_ACTION_OVER = 1007;//敌人行动结束
        public const int ON_TURN_CHG = 1008;//回合轮转
        public const int ON_ENEMY_DEAD = 1009;//敌人阵亡
        public const int ON_INSTRUMENT_DEAD = 1010;//乐器阵亡

        public const int ON_INSTRUMENT_START_ATTACK = 1011;
        public const int ON_INSTRUMENT_END_ATTACK = 1012;

        public const int ON_ENEMY_START_ATTACK = 1013;//敌人开始攻击
        public const int ON_ENEMY_END_ATTACK = 1014;//敌人结束攻击

        public const int ON_STORE_ITEM_SELECT = 1015; // 选中商品
        public const int ON_STORE_OPEN = 1016; // 商店界面打开


        public const int ON_SELECTOR_INSTRUMENT_CANCLE = 2001; // 回退乐器选择
    }

    public class ConstVar
    {
        public const int MAX_NOTE_NUM = 9;// 最大的音符数量 
    }
}
