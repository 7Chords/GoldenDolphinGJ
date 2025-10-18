using DG.Tweening;
using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗装饰物
/// </summary>
public class BattleDecoration : MonoBehaviour
{
    public float duration;
    public float fadeInDuration;
    public float fadeOutDuration;
    public CanvasGroup canvasGourp;
    private TweenContainer _tweenContainer;

    public void Init()
    {
        canvasGourp.alpha = 0;
        _tweenContainer = new TweenContainer();
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGourp.DOFade(1, fadeInDuration));
        seq.AppendInterval(duration);
        seq.Append(canvasGourp.DOFade(0, fadeOutDuration)).OnComplete(() =>
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
