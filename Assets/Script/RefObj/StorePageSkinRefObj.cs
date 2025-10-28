using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePageSkinRefObj : SCRefDataCore
{
    public long id;
    public string storeBackGround;
    public string backBtn;
    public string storeText;
    public string NoteBackGround;
    public string selectContainerBackground;
    protected override void _parseFromString()
    {
        id = getLong("id");
        storeBackGround = getString("storeBackGround");
        backBtn = getString("backBtn");
        storeText = getString("storeText");
        NoteBackGround = getString("NoteBackGround");
        selectContainerBackground = getString("selectContainerBackground");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "storePageSkin";
}
