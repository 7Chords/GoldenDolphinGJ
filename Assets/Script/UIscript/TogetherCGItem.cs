using DG.Tweening;
using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogetherCGItem : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public CanvasGroup cgCanvasGroup;

    public Image imgContent;

    public float fadeInDuration;
    public float fadeOutDuration;

    public float cgFadeInDuration;

    private TweenContainer _tweenContainer;

    public void Show(Sprite showCG,float showTime)
    {
        _tweenContainer = new TweenContainer();
        canvasGroup.alpha = 0;
        cgCanvasGroup.alpha = 0;
        imgContent.sprite = showCG;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1, fadeInDuration));
        seq.Append(cgCanvasGroup.DOFade(1, cgFadeInDuration));
        seq.AppendInterval(showTime);
        seq.Append(canvasGroup.DOFade(0, fadeOutDuration)).OnComplete(() =>
        {
            Destroy(gameObject);
        });

        _tweenContainer.RegDoTween(seq);

    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }
}
