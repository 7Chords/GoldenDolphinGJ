using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillStarItem : MonoBehaviour
{
    public Sprite fullStar;
    public Sprite emptyStar;

    public Image imgStar;
    public void SetState(bool full)
    {
        imgStar.sprite = full ? fullStar : emptyStar;
    }
}
