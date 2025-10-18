using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 简单 Slider 控制器（增加 DOTween 版本的 AnimateTo）
/// </summary>
public class SimpleSliderUI : MonoBehaviour
{
    [Header("进度填充图")]
    [SerializeField] private Image fillImage;

    [Range(0f, 1f)]
    [SerializeField] private float value = 0f;

    private Tween currentTween;

    private void Start()
    {
        ApplyValue(value);
    }

    public void SetValue(float v)
    {
        value = Mathf.Clamp01(v);
        ApplyValue(value);
    }

    // 根据当前时间和总时间立即设置填充
    public void SetByTime(float currentTime, float totalTime)
    {
        float ratio = (totalTime <= 0f) ? 0f : Mathf.Clamp01(currentTime / totalTime);
        SetValue(ratio);
    }

    // DOTween 版本的平滑过渡到指定比例
    public void AnimateTo(float target, float duration)
    {
        target = Mathf.Clamp01(target);
        // 用 DOTween 动态驱动 value，并在每帧回调设置视觉
        currentTween = DOTween.To(() => value, v => { value = v; ApplyValue(value); }, target, Mathf.Max(0.0001f, duration))
                             .SetEase(Ease.Linear)
                             .OnKill(() => currentTween = null)
                             .OnComplete(() => currentTween = null);
    }

    // 根据当前时间和总时间平滑过渡到对应比例
    public void AnimateToByTime(float currentTime, float totalTime, float duration)
    {
        float target = (totalTime <= 0f) ? 0f : Mathf.Clamp01(currentTime / totalTime);
        AnimateTo(target, duration);
    }

    private void ApplyValue(float v)
    {
        if (fillImage == null) return;

        if (fillImage.type == Image.Type.Filled)
        {
            fillImage.fillAmount = v;
            return;
        }

        var rt = fillImage.rectTransform;
        if (rt != null)
        {
            var s = rt.localScale;
            s.x = Mathf.Clamp01(v);
            rt.localScale = s;
        }
    }
}