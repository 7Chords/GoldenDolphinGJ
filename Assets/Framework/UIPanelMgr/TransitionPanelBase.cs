using DG.Tweening;
using GJFramework;
using UnityEngine;
using UnityEngine.UI;

// 过渡面板继承自 UIPanelBase
public class TransitionPanelBase : UIPanelBase
{
    private string targetSceneName; // 目标场景名（需要加载的场景）
    // 此处补充一些过度动画所需要的参数




    // 开始过渡动画的入口方法
    public void StartTransition(string sceneName)
    {
        targetSceneName = sceneName;
        // 播放动画
        onTransition();
    }

    // 动画开始
    private void onTransition()
    {
        // 过渡动画刚开始播放 就开始加载场景
        MsgCenter.SendMsg(MsgConst.OnTransitionFinnish, targetSceneName);
        // 过渡动画逻辑


        /*currentTween = canvasGroup.DOFade(0, fadeOutTime)
           .OnComplete(onTransitionComplete);*/
    }

    // 过渡动画结束的回调
    private void onTransitionComplete()
    {
        Hide();
        targetSceneName = null;
    }

    // 重写基类的 OnShow 方法
    protected override void OnShow()
    {
        base.OnShow();

    }
    protected override void OnHide()
    {
        base.OnHide();
    }
}