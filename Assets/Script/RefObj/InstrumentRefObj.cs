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
    public string instrumentName;
    public string instrumentDesc;
    public int attack;
    public int health;
    protected override void _parseFromString()
    {
        id = getLong("id");
        instrumentType = (EInstrumentType)getEnum("instrumentType", typeof(EInstrumentType));
        instrumentName = getString("instrumentName");
        instrumentDesc = getString("instrumentDesc");
        attack = getInt("attack");
        health = getInt("health");

    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrument";
}
