using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRefObj : SCRefDataCore
{
    public SkillRefObj()
    {

    }
    public SkillRefObj(string _assetPath, string _sheetName) : base(_assetPath, _sheetName)
    {
    }
    public long id;
    public string skillName;
    public string skillDesc;
    public ESkillType skillType;
    public List<long> skillUserList;
    public EPassiveSkillTriggerType passiveSkillTriggerType;
    protected override void _parseFromString()
    {
        id = getLong("id");
        skillName = getString("skillName");
        skillDesc = getString("skillDesc");
        skillType = (ESkillType)getEnum("skillType",typeof(ESkillType));
        skillUserList = getList<long>("skillUserList");
        passiveSkillTriggerType = (EPassiveSkillTriggerType)getEnum("passiveSkillTriggerType", typeof(EPassiveSkillTriggerType));

    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "skill";

}
