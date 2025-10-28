using GJFramework;

public class ResultResRefObj : SCRefDataCore
{

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
