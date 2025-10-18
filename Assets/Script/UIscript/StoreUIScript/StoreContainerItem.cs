using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using GJFramework;

public class StoreContainerItem : MonoBehaviour, IPointerClickHandler
{
    [Header("DOTween 点击缩放设置")]
    [SerializeField] private float clickScale = 1.15f;    // 放大目标比例
    [SerializeField] private float totalDuration = 0.22f; // 放大+缩小总时长（秒）
    [SerializeField] private Ease ease = Ease.OutBack;    // 缓动类型
    [SerializeField] private Image StoreItemImg;
    [SerializeField] private Sprite selectedSprite;
    private Vector3 originalScale;
    private Sequence clickSequence;
    [SerializeField] private long stroeItemId;// 商店商品Id
    
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //PlayerMgr.Instance.RemoveNoteNum();
        // 选中的话告知订阅者
        MsgCenter.SendMsg(MsgConst.ON_STORE_ITEM_SELECT, selectedSprite);
        // 终止已有动画
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
            clickSequence = null;
            transform.localScale = originalScale;
        }

        // 构建放大 -> 缩小 的 Sequence
        float half = Mathf.Max(0.001f, totalDuration * 0.5f);
        clickSequence = DOTween.Sequence();
        clickSequence.Append(transform.DOScale(originalScale * clickScale, half).SetEase(ease));
        clickSequence.Append(transform.DOScale(originalScale, half).SetEase(ease));
        clickSequence.OnComplete(() =>
        {
            transform.localScale = originalScale;
            clickSequence = null;
        });
    }

    private void OnDisable()
    {
        if (clickSequence != null && clickSequence.IsActive())
        {
            clickSequence.Kill();
            clickSequence = null;
            transform.localScale = originalScale;
        }
    }
}