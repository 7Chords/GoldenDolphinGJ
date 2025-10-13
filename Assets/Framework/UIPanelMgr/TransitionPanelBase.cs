using DG.Tweening;
using UnityEngine;


namespace GJFramework
{
    // 中间基类
    public class TransitionPanelBase : UIPanelBase
    {
        // 通用配置
        [Header("过渡通用配置")]
        [SerializeField] protected float transitionDuration = 0.5f; // 过渡总时长

        // 通用变量
        protected string targetSceneName;
        protected Tween currentTween; // 管理动画，防止重复/内存泄漏

        // 过渡入口
        public void StartTransition(string sceneName)
        {
            targetSceneName = sceneName;
            // 触发子类的具体过渡逻辑 由子类实现
            OnTransitionStart();
        }

        // 过渡开始后，先执行子类动画，动画完成后发消息给SceneLoader
        protected void TriggerTransitionComplete()
        {
            // 过渡动画完成  通知SceneLoader加载场景
            MsgCenter.SendMsg(MsgConst.OnTransitionFinnish, targetSceneName);
            // 执行后续清理
            OnTransitionCleanup();
        }

        // 具体过渡动画逻辑
        protected virtual void OnTransitionStart() { }

        // 过渡完成后的清理
        protected virtual void OnTransitionCleanup() { }

        // 通用动画清理
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (currentTween != null && currentTween.IsActive())
            {
                currentTween.Kill();
            }
            DOTween.Clear(false);
        }
    }
}
