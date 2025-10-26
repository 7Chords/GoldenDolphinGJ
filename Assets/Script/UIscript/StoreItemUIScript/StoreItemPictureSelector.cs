using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemPictureSelector : MonoBehaviour
{
    [SerializeField] private Image curShowImage;
    [SerializeField] private Sprite[] itemSprites;

    // 如果当前关卡小于解锁关卡，则显示锁定图片

    public bool SetInfo(long unlockLevelId)
    {
        bool isLock = unlockLevelId <= GameMgr.Instance.curLevel;

        if (isLock)
            curShowImage.sprite = itemSprites[1]; // 解锁图片
        else 
            curShowImage.sprite = itemSprites[0]; // 锁定图片

        return isLock;
    }

}
