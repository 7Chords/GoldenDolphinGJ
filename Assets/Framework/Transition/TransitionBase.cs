
using System;
using UnityEngine;

namespace GJFramework
{
    /// <summary>
    /// 过渡效果基类（抽象类）
    /// 仅提供基础框架，具体过渡效果由子类重写实现
    /// 已修改为非协程实现：EnterTransition/ExitTransition 为同步方法（void）
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
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
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
            canvasGroup.alpha = 1;
            isTransitioning = false;
            gameObject.SetActive(true);
            OnShowImmediately();
        }

        /// <summary>
        /// 立即隐藏（无过渡效果）
        /// </summary>
        public virtual void HideImmediately()
        {
            canvasGroup.alpha = 0;
            isTransitioning = false;
            gameObject.SetActive(false);
            OnHideImmediately();
        }

        /// <summary>
        /// 立即显示后的回调（子类可重写）
        /// </summary>
        protected virtual void OnShowImmediately() { }

        /// <summary>
        /// 立即隐藏后的回调（子类可重写）
        /// </summary>
        protected virtual void OnHideImmediately() { }

        public void OnHide()
        {
            gameObject.SetActive(false);
        }

        public void OnShow()
        {
            gameObject.SetActive(true);
        }
    }
}