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
    public EInstrumentType instrumentType;
    public string instrumentName;
    protected override void _parseFromString()
    {
        id = getLong("id");
        instrumentId = getLong("instrumentId");
        hightNoteNum = getInt("hightNoteNum");
        lowNoteNum = getInt("lowNoteNum");
        middleNoteNum = getInt("middleNoteNum");
        instrumentType = (EInstrumentType)getEnum("instrumentType", typeof(EInstrumentType));
        instrumentName = getString("instrumentName");
    }

    public static string assetPath => "RefData/ExportTxt";

    public static string sheetName => "instrumentStore";
}