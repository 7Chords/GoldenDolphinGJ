using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 乐器信息类
/// </summary>
public class InstrumentInfo
{
    public EInstrumentType instrumentType;
    public string instrumentName;
    public string instrumentDesc;
    public int health;
    public int attack;
    public string instrumentIconPath;
    public string instrumentBgPath;

    public InstrumentInfo(EInstrumentType instrumentType, string instrumentName, string instrumentDesc, int health, int attack, string instrumentIconPath, string instrumentBgPath)
    {
        this.instrumentType = instrumentType;
        this.instrumentName = instrumentName;
        this.instrumentDesc = instrumentDesc;
        this.health = health;
        this.attack = attack;
        this.instrumentIconPath = instrumentIconPath;
        this.instrumentBgPath = instrumentBgPath;
    }
}
