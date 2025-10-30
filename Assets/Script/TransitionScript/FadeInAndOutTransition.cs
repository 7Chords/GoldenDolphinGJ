using DG.Tweening;
using GJFramework;
using UnityEngine;

public class FadeInAndOutTransition : TransitionBase
{
    // 淡入过渡（非协程）
    protected override void EnterTransition()
    {
        if (string.IsNullOrEmpty(tragetSceneName))
        {
            Debug.LogError("SceneName IsNullOrEmpty!");
            isTransitioning = false;
            return;
        }

        // 本地缓存，防止回调时引用到已被 Unity 回收的字段
        var localCanvas = canvasGroup;
        if (localCanvas == null)
        {
            Debug.LogWarning("FadeInAndOutTransition: canvasGroup is null, abort EnterTransition");
            isTransitioning = false;
            return;
        }

        // 先杀死可能存在的旧动画，避免冲突
        try { localCanvas.DOKill(false); } catch { }

        // DOTween 控制 alpha 从 0 到 1（淡入）
        localCanvas.alpha = 0; // 起始透明度
        localCanvas.DOFade(1, transitionDuration) // 目标值、持续时间
            .SetEase(Ease.InOutQuad) // 缓动曲线
            .OnComplete(() =>
            {
                // 在回调里先检查对象与组件是否仍有效
                if (this == null || localCanvas == null)
                {
                    isTransitioning = false;
                    return;
                }

                // 进入过渡完成后发送消息并开始退出过渡（非协程）
                MsgCenter.SendMsg(MsgConst.ON_TRANSITION_IN, tragetSceneName);
                ExitTransition();
            });
    }

    // 淡出过渡（非协程）
    protected override void ExitTransition()
    {
        var localCanvas = canvasGroup;
        if (localCanvas == null)
        {
            // 仍然要尝试发送消息并结束状态
            MsgCenter.SendMsgAct(MsgConst.ON_TRANSITION_OUT);
            gameObject.SetActive(false);
            isTransitioning = false;
            return;
        }

        // 先杀死可能存在的旧动画
        try { localCanvas.DOKill(false); } catch { }

        // DOTween 控制 alpha 从 1 到 0（淡出）
        localCanvas.alpha = 1; // 起始透明度
        localCanvas.DOFade(0, transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 回调安全检查
                if (this == null)
                {
                    isTransitioning = false;
                    return;
                }

                MsgCenter.SendMsgAct(MsgConst.ON_TRANSITION_OUT);
                gameObject.SetActive(false);
                isTransitioning = false;
            });
    }
}