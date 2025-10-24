using DG.Tweening;
using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnEffect_StartPanel : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public float selectFadeDuration;
    public float unselectFadeDuration;
    public float selectScale;
    public float unselectScale;
    public Sprite selectSprite;
    public Sprite unselectSprite;
    public GameObject selectSpawnDecPrefab;

    public Image imgBtn;
    private TweenContainer _tweenContainer;

    private Stack<GameObject> _highlightStack;

    private void Awake()
    {
        _tweenContainer = new TweenContainer();
        _highlightStack = new Stack<GameObject>();
    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;

        // 清理所有高亮对象
        while (_highlightStack.Count > 0)
        {
            GameObject go = _highlightStack.Pop();
            if (go != null)
            {
                Destroy(go);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imgBtn.sprite = selectSprite;

        // 创建新的高亮对象
        GameObject highlightGO = Instantiate(selectSpawnDecPrefab);
        highlightGO.transform.SetParent(transform.parent);
        highlightGO.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().localPosition.y, 0);
        highlightGO.GetComponent<CanvasGroup>().alpha = 0;

        // 将新创建的对象放入栈中
        _highlightStack.Push(highlightGO);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(selectScale, selectFadeDuration));
        seq.Join(highlightGO.GetComponent<CanvasGroup>().DOFade(1, selectFadeDuration));

        _tweenContainer.RegDoTween(seq);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imgBtn.sprite = unselectSprite;

        // 检查栈中是否有对象
        if (_highlightStack.Count == 0)
        {
            // 如果没有高亮对象，只执行缩放动画
            Sequence scaleSeq = DOTween.Sequence();
            scaleSeq.Append(transform.DOScale(unselectScale, unselectFadeDuration));
            _tweenContainer.RegDoTween(scaleSeq);
            return;
        }

        // 从栈中取出最近创建的高亮对象
        GameObject highlightGO = _highlightStack.Pop();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(unselectScale, unselectFadeDuration));

        if (highlightGO != null)
        {
            seq.Join(highlightGO.GetComponent<CanvasGroup>().DOFade(0, unselectFadeDuration))
               .OnComplete(() => Destroy(highlightGO));
        }

        _tweenContainer.RegDoTween(seq);
    }
}