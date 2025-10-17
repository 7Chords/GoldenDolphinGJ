using GJFramework;

public class CardRefObj : SCRefDataCore
{
    public CardRefObj()
    {

    }
    public CardRefObj(string _assetPath, string _sheetName) : base(_assetPath, _sheetName)
    {
    }

    public long id;
    public string cardName;
    public string cardDesc;
    protected override void _parseFromString()
    {
        id = getLong("id");
        cardName = getString("cardName");
        cardDesc = getString("cardDesc");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "card";
}
