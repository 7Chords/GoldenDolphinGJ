using System;
using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    // 基础面板类
    public class UIPanelBase : MonoBehaviour
    {
        public EPanelType PanelType;

        // 显示面板
        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        // 隐藏面板
        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        public void PlayTransition(string _sceneName)
        {
            OnTransition(_sceneName);
        }
        // 销毁面板
        public void Destroy()
        {
            OnDestroy();
            Destroy(gameObject);
        }

        // 显示时的回调（供子类重写）
        protected virtual void OnShow() { }

        // 隐藏时的回调（供子类重写）
        protected virtual void OnHide() { }

        // 销毁时的回调（供子类重写）
        protected virtual void OnDestroy() { }

        protected virtual void OnTransition(string _sceneName) { }

    }

}

