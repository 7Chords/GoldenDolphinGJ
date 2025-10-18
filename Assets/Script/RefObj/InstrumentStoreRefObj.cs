using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentStoreRefObj : SCRefDataCore
{
    public long id;
    public long instrumentId;
    public int hightNoteNum;
    public int lowNoteNum;
    public int middleNoteNum;
    public EInstrumentEffectType effectType;
    public string instrumentName;
    protected override void _parseFromString()
    {
        id = getLong("id");
        instrumentId = getLong("instrumentId");
        hightNoteNum = getInt("hightNoteNum");
        lowNoteNum = getInt("lowNoteNum");
        middleNoteNum = getInt("middleNoteNum");
        effectType = (EInstrumentEffectType)getEnum("effectType", typeof(EInstrumentEffectType));
        instrumentName = getString("instrumentName");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrumentStore";
}