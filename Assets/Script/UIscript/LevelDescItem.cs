using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDescItem : MonoBehaviour
{

    public Image imgBg;
    public Image imgPoint;
    public Image imgEnemyName;
    public Text txtEnemyDesc;
    public Text txtEnemyHealth;
    public Text txtEnemyAttack;
    public void SetInfo(BattleLevelRefObj _levelRefObj)
    {
        imgBg.sprite = Resources.Load<Sprite>(_levelRefObj.levelPreviewBgPath);
        imgPoint.sprite = Resources.Load<Sprite>(_levelRefObj.levelPreviewPointDecPath);
        imgEnemyName.sprite = Resources.Load<Sprite>(_levelRefObj.levelEnemyNameImgPath);


    }
}
