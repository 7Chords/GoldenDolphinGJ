using GJFramework;

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
    public string instrumentBgPath;

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
        instrumentBgPath = getString("instrumentBgPath");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrument";
}
