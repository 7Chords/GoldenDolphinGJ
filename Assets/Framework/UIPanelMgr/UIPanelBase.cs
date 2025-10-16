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
        public EPanelType panelType;

        // 显示面板
        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        // 隐藏面板
        public void Hide()
        {
            OnHide(()=>
            {
                gameObject.SetActive(false);
            });
        }

        public void PlayTransition(string _sceneName)
        {
            OnTransition(_sceneName);
        }
        // 销毁面板
        public void Destroy()
        {
            BeforeDestroy();
            Destroy(gameObject);
        }

        // 显示时的回调（供子类重写）
        protected virtual void OnShow() { }

        // 隐藏时的回调（供子类重写）
        protected virtual void OnHide(Action onHideFinished) { }

        // 销毁时的回调（供子类重写）
        protected virtual void BeforeDestroy() { }

        protected virtual void OnTransition(string _sceneName) { }

    }

}

