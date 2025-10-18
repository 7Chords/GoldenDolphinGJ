using UnityEngine;

public class SliderNoteBinder : MonoBehaviour
{
    [Tooltip("拖入挂有 SimpleSliderUI 的对象")]
    [SerializeField] private SimpleSliderUI slider;

    [Tooltip("用于计算比例的总时长（秒），例如计时上限）")]
    [SerializeField] private float maxTotalTime = 5f;

    [Tooltip("是否使用平滑过渡（DOTween）而不是直接设置）")]
    [SerializeField] private bool useAnimate = false;

    [Tooltip("当 useAnimate 为 true 时的过渡时长（秒）")]
    [SerializeField] private float animateDuration = 0.2f;

    void Update()
    {
        if (slider == null || NoteMgr.instance == null) return;

        float currentTime = NoteMgr.instance.TotalPauseTime; // 你提供的值
        float total = Mathf.Max(0.0001f, maxTotalTime);

        if (useAnimate)
        {
            slider.AnimateToByTime(currentTime, total, animateDuration);
        }
        else
        {
            slider.SetByTime(currentTime, total);
        }
    }
}