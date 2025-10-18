
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

        // 先杀死可能存在的旧动画，避免冲突
        canvasGroup.DOKill();

        // DOTween 控制 alpha 从 0 到 1（淡入）
        canvasGroup.alpha = 0; // 起始透明度
        canvasGroup.DOFade(1, transitionDuration) // 目标值、持续时间
            .SetEase(Ease.InOutQuad) // 缓动曲线
            .OnComplete(() =>
            {
                // 进入过渡完成后发送消息并开始退出过渡（非协程）
                MsgCenter.SendMsg(MsgConst.ON_TRANSITION_IN, tragetSceneName);
                ExitTransition();
            });
    }

    // 淡出过渡（非协程）
    protected override void ExitTransition()
    {
        // 先杀死可能存在的旧动画
        canvasGroup.DOKill();

        // DOTween 控制 alpha 从 1 到 0（淡出）
        canvasGroup.alpha = 1; // 起始透明度
        canvasGroup.DOFade(0, transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                MsgCenter.SendMsgAct(MsgConst.ON_TRANSITION_OUT);
                gameObject.SetActive(false);
                isTransitioning = false;
            });
    }
}