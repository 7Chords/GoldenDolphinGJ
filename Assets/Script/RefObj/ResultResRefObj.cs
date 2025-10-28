using GJFramework;

public class ResultResRefObj : SCRefDataCore
{


    public ResultResRefObj()
    {

    }
    public ResultResRefObj(string _assetPath, string _sheetName) : base(_assetPath, _sheetName)
    {
    }
    public long id;
    public string loseBgPath;
    public string winBgPath;


    protected override void _parseFromString()
    {
        id = getLong("id");
        loseBgPath = getString("loseBgPath");
        winBgPath = getString("winBgPath");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "result_res";
}
