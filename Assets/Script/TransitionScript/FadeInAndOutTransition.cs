using DG.Tweening;
using GJFramework;
using System.Collections;
using UnityEngine;

public class FadeInAndOutTransition : TransitionBase
{
    // 淡入过渡
    protected override IEnumerator EnterTransition()
    {
        if (string.IsNullOrEmpty(tragetSceneName))
        {
            Debug.LogError("SceneName IsNullOrEmpty!");
            yield return null;
        }

        // 先杀死可能存在的旧动画，避免冲突
        canvasGroup.DOKill();

        // DOTween 控制 alpha 从 0 到 1（淡入）
        canvasGroup.alpha = 0; // 起始透明度
        canvasGroup.DOFade(1, transitionDuration) // 目标值、持续时间
            .SetEase(Ease.InOutQuad) // 缓动曲线
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                MsgCenter.SendMsg(MsgConst.onTransitionIn, tragetSceneName);
                // 过渡进入完成后 发送信息 并开启过渡淡出
                StartCoroutine(ExitTransition());
            });

        yield return null;
    }

    // 淡出过渡
    protected override IEnumerator ExitTransition()
    {
        // 先杀死可能存在的旧动画
        canvasGroup.DOKill();


        // DOTween 控制 alpha 从 1 到 0（淡出）
        canvasGroup.alpha = 1; // 起始透明度
        canvasGroup.DOFade(0, transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                Debug.Log(11111);
                MsgCenter.SendMsgAct(MsgConst.onTransitionOut);
                gameObject.SetActive(false);
            });

        yield return null;
    }
}
