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
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        // 下一个场景预加载的面板类型列表
        private List<EPanelType> _nextSceneEPanelTypes;
        protected override void Awake()
        {
            base.Awake();
            MsgCenter.RegisterMsg(MsgConst.ON_TRANSITION_IN, LoadScene);
            _nextSceneEPanelTypes = new List<EPanelType>();
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
            if (string.IsNullOrEmpty(tragetSceneName))
            {
                Debug.LogError("过渡动画回调传入的SceneName为空或类型错误");
                return;
            }

            if (!SceneExists(tragetSceneName))
            {
                Debug.LogError($"Scene '{tragetSceneName}' is not in Build Settings");
                return;
            }

            PanelUIMgr.Instance.OpenPanelFromList(
                _nextSceneEPanelTypes,
                () =>
                {
                    // 同步加载场景
                    SceneManager.LoadScene(tragetSceneName, _loadSceneMode);
                    ClearNextScenePanelList();
                });
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
            _loadSceneMode = mode;
        }

        public void SetNexScenePanel(List<EPanelType> panelTypeList)
        {
            _nextSceneEPanelTypes = panelTypeList;
        }
        public void AddNextScenePanel(EPanelType panelType)
        {
            _nextSceneEPanelTypes.Add(panelType);
        }

        private void ClearNextScenePanelList()
        {
            _nextSceneEPanelTypes.Clear();
        }
    }
}