using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class storeSkinSetter : MonoBehaviour
{
    [SerializeField] private Image storeBackGround;
    [SerializeField] private Image backBtn;
    [SerializeField] private Image storeText;
    [SerializeField] private Image NoteBackGround;
    [SerializeField] private Image selectContainerBackground;
    public void SetCollectPageSkinInfo(StorePageSkinRefObj storePageSkinRefObj)
    {
        if (storeBackGround == null || backBtn == null || storeText == null)
        {
            Debug.LogError("查看是否真的引用到了，懒得写一堆null");
            return;
        }
        string path = "UI/StorePageSkin/";
        storeBackGround.sprite = Resources.Load<Sprite>(path + storePageSkinRefObj.storeBackGround);
        backBtn.sprite = Resources.Load<Sprite>(path + storePageSkinRefObj.backBtn);
        storeText.sprite = Resources.Load<Sprite>(path + storePageSkinRefObj.storeText);
        NoteBackGround.sprite = Resources.Load<Sprite>(path + storePageSkinRefObj.NoteBackGround);
        selectContainerBackground.sprite = Resources.Load<Sprite>(path + storePageSkinRefObj.selectContainerBackground);
    }
}
