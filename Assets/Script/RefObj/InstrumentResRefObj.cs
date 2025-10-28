using GJFramework;

public class InstrumentResRefObj : SCRefDataCore
{

    public InstrumentResRefObj()
    {

    }
    public InstrumentResRefObj(string _assetPath, string _sheetName) : base(_assetPath, _sheetName)
    {
    }
    public long id;
    public string instrumentBgPath;
    public string instrumentBodyBgPath;
    public string instrumentBodyBgWithChaPath;
    public string instrumentHealthHolderPath;
    protected override void _parseFromString()
    {
        id = getLong("id");
        instrumentBgPath = getString("instrumentBgPath");
        instrumentBodyBgPath = getString("instrumentBodyBgPath");
        instrumentBodyBgWithChaPath = getString("instrumentBodyBgWithChaPath");
        instrumentHealthHolderPath = getString("instrumentHealthHolderPath");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrument_res";
}
