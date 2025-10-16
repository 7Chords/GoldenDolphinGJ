using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GJFramework
{

    public class SceneLoader : SingletonPersistent<SceneLoader>
    {
        private bool isAllPreLoad = false;
        private LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        // 下一个场景预加载的面板类型列表
        private List<EPanelType> nextSceneEPanelTypes;
        protected override void Awake()
        {
            base.Awake();
            MsgCenter.RegisterMsg(MsgConst.ON_TRANSITION_IN, LoadScene);
            nextSceneEPanelTypes = new List<EPanelType>();
            DontDestroyOnLoad(this.gameObject);
        }

        // 直接加载场景
        public void LoadScene(string tragetSceneName)
        {
            if (string.IsNullOrEmpty(tragetSceneName))
            {
                Debug.LogError("currentSceneName is Null");
                return;
            }
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(tragetSceneName, loadSceneMode);
            if (asyncOperation == null) return;

            asyncOperation.allowSceneActivation = false;
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        // 过渡动画回调加载场景 加载场景名通过参数传递

        private void LoadScene(object[] _objs)
        {
            if (_objs == null || _objs.Length == 0)
            {
                Debug.LogError("过渡动画回调传入的SceneName有误");
                return;
            }
            string tragetSceneName = _objs[0] as string;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(tragetSceneName, loadSceneMode);
            PanelUIMgr.Instance.OpenPanelFromList(
                nextSceneEPanelTypes,
                () =>
                {
                    isAllPreLoad = true;
                });
            
            if (asyncOperation == null) return;

            asyncOperation.allowSceneActivation = false;
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        private IEnumerator MonitorLoadingProgress(AsyncOperation operation)
        {
            while (!operation.isDone && isAllPreLoad)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                    // 重置预加载状态
                    isAllPreLoad = false;
                    ClearNextScenePanelList();
                }
                yield return null;
            }
        }

        private bool SceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneName)
                    return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            MsgCenter.UnregisterMsg(MsgConst.ON_TRANSITION_IN, LoadScene);
        }

        public void SetLoadSceneMode(LoadSceneMode mode)
        {
            loadSceneMode = mode;
        }

        public void SetNexScenePanel(List<EPanelType> panelTypeList)
        {
            nextSceneEPanelTypes = panelTypeList;
        }
        public void AddNextScenePanel(EPanelType panelType)
        {
            nextSceneEPanelTypes.Add(panelType);
        }

        private void ClearNextScenePanelList()
        {
            nextSceneEPanelTypes.Clear();
        }
    }
}