using System;
using UnityEngine;
using DG.Tweening;

namespace GJFramework
{
    /// <summary>
    /// 过渡效果基类（抽象类）
    /// 仅提供基础框架，具体过渡效果由子类重写实现
    /// </summary>
    public abstract class TransitionBase : MonoBehaviour
    {
        [Header("过渡基础设置")]
        [Tooltip("过渡动画持续时间")]
        [SerializeField] protected float transitionDuration = 0.5f;

        protected string tragetSceneName;

        [SerializeField] protected CanvasGroup canvasGroup;
        protected bool isTransitioning = false;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // 初始化状态（子类可根据需求重写）
            InitializeState();
        }

        public void SetTragetSceneName(string _tragetSceneName)
        {
            tragetSceneName = _tragetSceneName;
        }

        /// <summary>
        /// 初始化显示状态
        /// </summary>
        protected virtual void InitializeState()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = true;
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 开始进入过渡（同步）
        /// </summary>
        public void StartEnterTransition()
        {
            if (isTransitioning) return;

            gameObject.SetActive(true);
            isTransitioning = true;
            EnterTransition();
        }

        /// <summary>
        /// 进入过渡（抽象方法，子类必须实现） - 非协程版
        /// </summary>
        protected abstract void EnterTransition();

        /// <summary>
        /// 退出过渡（抽象方法，子类必须实现） - 非协程版
        /// </summary>
        protected abstract void ExitTransition();

        /// <summary>
        /// 立即显示（无过渡效果）
        /// </summary>
        public virtual void ShowImmediately()
        {
            if (canvasGroup != null) canvasGroup.alpha = 1;
            isTransitioning = false;
            gameObject.SetActive(true);
            OnShowImmediately();
        }

        /// <summary>
        /// 立即隐藏（无过渡效果）
        /// </summary>
        public virtual void HideImmediately()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0;
            isTransitioning = false;
            gameObject.SetActive(false);
            OnHideImmediately();
        }

        protected virtual void OnShowImmediately() { }
        protected virtual void OnHideImmediately() { }

        /// <summary>
        /// 外部安全关闭/销毁过渡：先停止 Tween，再销毁 GameObject
        /// </summary>
        public void CloseTransition()
        {
            // 停掉所有针对 canvasGroup 或该 GameObject 的 Tweens，防止回调访问已销毁对象
            try
            {
                if (canvasGroup != null)
                    canvasGroup.DOKill(false);
                DOTween.Kill(gameObject, false);
            }
            catch (Exception) { }

            isTransitioning = false;

            // 先隐藏再销毁，避免瞬间可见性问题
            try
            {
                if (gameObject != null)
                    gameObject.SetActive(false);
            }
            catch (Exception) { }

            // 延迟销毁一个 frame 更安全（可改为 0）
            try
            {
                if (this != null && this.gameObject != null)
                    Destroy(this.gameObject);
            }
            catch (Exception) { }
        }

        // Unity 生命周期销毁，确保任何未被显式 Close 的情况也能安全 Kill Tween
        protected virtual void OnDestroy()
        {
            try
            {
                if (canvasGroup != null)
                    canvasGroup.DOKill(false);
                DOTween.Kill(gameObject, false);
            }
            catch (Exception) { }
        }
    }
}