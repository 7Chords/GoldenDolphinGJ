using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemPictureSelector : MonoBehaviour
{
    [SerializeField] private Image curShowImage;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private GameObject[] unlockShowGo;// 解锁展示的文字

    // 如果当前关卡小于解锁关卡，则显示锁定图片

    public bool SetInfo(long unlockLevelId)
    {
        bool isUnLock = unlockLevelId <= GameMgr.Instance.curLevel;
        // 真就是解锁状态 
        if (isUnLock)
            curShowImage.sprite = itemSprites[1]; // 解锁图片
        else
            DoOnLock();

        return isUnLock;
    }
    // 默认是显示的 所以不用去处理显示
    private void DoOnLock()
    {
        // 对列表进行隐藏
        foreach (var go in unlockShowGo)
        {
            go.SetActive(false);
        }
        curShowImage.sprite = itemSprites[0]; // 锁定图片
    }

}
