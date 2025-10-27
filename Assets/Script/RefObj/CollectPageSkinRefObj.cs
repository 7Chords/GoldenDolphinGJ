using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPageSkinRefObj : SCRefDataCore
{
    long id;
    public string collectIcon;
    public string charactorBackGround;
    public string phonograph;
    public string phonographBackGround;
    public string SliderBackGroundImage;
    public string ProgressBar;
    public string SliderTank;
    public string line;
    public string backBtnImage;
    public string exchangeConditions;
    public string group;

    protected override void _parseFromString()
    {
        id = getLong("id");
        collectIcon = getString("collectIcon");
        charactorBackGround = getString("charactorBackGround");
        phonograph = getString("phonograph");
        phonographBackGround = getString("phonographBackGround");
        SliderBackGroundImage = getString("SliderBackGroundImage");
        ProgressBar = getString("ProgressBar");
        SliderTank = getString("SliderTank");
        line = getString("line");
        backBtnImage = getString("backBtnImage");
        exchangeConditions = getString("exchangeConditions");
        group = getString("group");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "collectPageSkin";
}
