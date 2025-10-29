using GJFramework;
using System.Collections.Generic;
public class InstrumentRefObj : SCRefDataCore
{
    public InstrumentRefObj()
    {

    }
    public InstrumentRefObj(string _assetPath, string _sheetName) : base(_assetPath, _sheetName)
    {
    }

    public long id;
    public EInstrumentType instrumentType;
    public EInstrumentEffectType effectType;
    public string instrumentName;
    public string instrumentDesc;
    public int attack;
    public int health;
    public int heal;
    public int buff;
    public string instrumentIconPath;
    public string instrumentIconUnlockPath;
    public string instrumentBodyPath;
    public string instrumentNamePath;
    public string instrumentAttackIconPath;
    public string instrumentAttackSoundPath;
    public string instrumentHurtSoundPath;
    public long unlockLevelId;
    public bool hasTogetherSkill;
    public int canUseSkillPoint;
    public List<long> skillIdList;
    public List<EInstrumentRoleTypeList> instrumentRoleTypeList;
    public string instrumentPreviewIconPath;
    public List<long> resSkinIdList;
    public string effectTypeIconPath;
    public string characterStoreHeadPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        instrumentType = (EInstrumentType)getEnum("instrumentType", typeof(EInstrumentType));
        effectType = (EInstrumentEffectType)getEnum("effectType", typeof(EInstrumentEffectType));
        instrumentName = getString("instrumentName");
        instrumentDesc = getString("instrumentDesc");
        attack = getInt("attack");
        health = getInt("health");
        heal = getInt("heal");
        buff = getInt("buff");
        instrumentIconPath = getString("instrumentIconPath");
        instrumentIconUnlockPath = getString("instrumentIconUnlockPath");
        instrumentBodyPath = getString("instrumentBodyPath");
        instrumentNamePath = getString("instrumentNamePath");
        instrumentAttackIconPath = getString("instrumentAttackIconPath");
        instrumentAttackSoundPath = getString("instrumentAttackSoundPath");
        instrumentHurtSoundPath = getString("instrumentHurtSoundPath");
        unlockLevelId = getLong("unlockLevelId");
        hasTogetherSkill = getBool("hasTogetherSkill");
        canUseSkillPoint = getInt("canUseSkillPoint");
        skillIdList = getList<long>("skillIdList");
        instrumentRoleTypeList = getList<EInstrumentRoleTypeList>("instrumentRoleTypeList");
        instrumentPreviewIconPath = getString("instrumentPreviewIconPath");
        resSkinIdList = getList<long>("resSkinIdList");
        effectTypeIconPath = getString("effectTypeIconPath");
        characterStoreHeadPath = getString("characterStoreHeadPath");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrument";
}
