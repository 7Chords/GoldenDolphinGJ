using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collectPageSkinSetter : MonoBehaviour
{
    [SerializeField] private Image collectIcon;
    [SerializeField] private Image noteCollectBackGround;
    [SerializeField] private Image charactorBackGround;
    [SerializeField] private Image phonograph;
    [SerializeField] private Image phonographBackGround;
    [SerializeField] private Image SliderBackGroundImage;
    [SerializeField] private Image ProgressBar;
    [SerializeField] private Image SliderTank;
    //[SerializeField] private Image line;
    [SerializeField] private Image backBtnImage;
    [SerializeField] private Image exchangeConditions;
    //[SerializeField] private Image group;
    [SerializeField] private Image inArrow;
    [SerializeField] private Image outArrow;
    [SerializeField] private Image exchangeBackground;
    public void SetCollectPageSkinInfo(CollectPageSkinRefObj skinRefObj)
    {
        if (collectIcon == null || charactorBackGround == null || phonograph == null)
        {
            Debug.LogError("查看是否真的引用到了，懒得写一堆null");
            return;
        }

        string path = "UI/CollectPageSkin/";

        noteCollectBackGround.sprite = Resources.Load<Sprite>(path + skinRefObj.noteCollectBackGround);
        collectIcon.sprite = Resources.Load<Sprite>(path + skinRefObj.collectIcon);
        charactorBackGround.sprite = Resources.Load<Sprite>(path + skinRefObj.noteCollectBackGround);
        phonograph.sprite = Resources.Load<Sprite>(path + skinRefObj.phonograph);
        phonographBackGround.sprite = Resources.Load<Sprite>(path + skinRefObj.phonographBackGround);
        SliderBackGroundImage.sprite = Resources.Load<Sprite>(path + skinRefObj.SliderBackGroundImage);
        ProgressBar.sprite = Resources.Load<Sprite>(path + skinRefObj.ProgressBar);
        SliderTank.sprite = Resources.Load<Sprite>(path + skinRefObj.SliderTank);
        //line.sprite = Resources.Load<Sprite>(path + skinRefObj.line);
        backBtnImage.sprite = Resources.Load<Sprite>(path + skinRefObj.backBtnImage);
        exchangeConditions.sprite = Resources.Load<Sprite>( path + skinRefObj.exchangeConditions);
        inArrow.sprite = Resources.Load<Sprite>(path + skinRefObj.inArrow);
        outArrow.sprite = Resources.Load<Sprite>(path + skinRefObj.outArrow);
        exchangeBackground.sprite = Resources.Load<Sprite>(path + skinRefObj.exchangeBackground);
        //group.sprite = Resources.Load<Sprite>(path + skinRefObj.group);
    }
}