using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 乐器信息类
/// </summary>
public class InstrumentInfo
{
    public int health;
    public int attack;
    public int heal;
    public int buff;

    public InstrumentRefObj refObj;

    public InstrumentInfo(InstrumentRefObj refObj)
    {
        this.refObj = refObj;
        health = refObj.heal;
        attack = refObj.attack;
        heal = refObj.heal;
        buff = refObj.buff;
    }
}
