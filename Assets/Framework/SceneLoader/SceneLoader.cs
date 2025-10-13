using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GJFramework
{
    [System.Serializable]
    public class TransitionPreset
    {
        public EPanelType PresetPanelType;
        public string PresetName;
    }

    public class SceneLoader : SingletonMono<SceneLoader>
    {
        [SerializeField] private List<TransitionPreset> transitionPresets;
        [SerializeField] private string _defaultPresetName = "defaultPresetName";
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        private string _closeSceneName;
        private string _presetName;
        private Dictionary<string, EPanelType> presetDict;

        protected override void Awake()
        {
            base.Awake();
            MsgCenter.RegisterMsg(MsgConst.OnTransitionFinnish, loadScene);
            presetDict = new Dictionary<string, EPanelType>();
            foreach (var preset in transitionPresets)
            {
                if (!presetDict.ContainsKey(preset.PresetName))
                {
                    presetDict.Add(preset.PresetName, preset.PresetPanelType);
                }
            }
        }

        public void LoadSceneWithTransition(string loadSceneName, string closeSceneName, string presetName = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            string usedPresetName = presetName ?? _defaultPresetName;

            _presetName = usedPresetName;
            _loadSceneMode = loadSceneMode;
            _closeSceneName = closeSceneName;
            if (!presetDict.TryGetValue(_presetName, out var PresetPanelType))
            {
                Debug.LogError("没有这个过渡Panel");
                return;
            }
            PlayTransitionPanel(PresetPanelType, loadSceneName);
        }

        private void loadScene(object[] _objs)
        {
            if (_objs == null || _objs.Length == 0)
            {
                Debug.LogError("过渡动画回调传入的SceneName有误");
                return;
            }

            string sceneName = _objs[0].ToString();
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"场景 {sceneName} 不存在");
                return;
            }
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, _loadSceneMode);
            if (asyncOperation == null) return;

            asyncOperation.allowSceneActivation = false;
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        private IEnumerator MonitorLoadingProgress(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            // 仅在“叠加加载且存在多个场景”时，才手动卸载旧场景
            if (!string.IsNullOrEmpty(_closeSceneName) && _loadSceneMode == LoadSceneMode.Additive && SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(_closeSceneName).completed += _ =>
                {
                    _closeSceneName = null;
                };
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

        private void PlayTransitionPanel(EPanelType targetPanelType, string sceneName)
        {
            UIPanelBase transitionPanel = PanelUIMgr.Instance.GetCachedPanel(targetPanelType);

            if (transitionPanel == null)
            {
                if (!PanelUIMgr.Instance.OpenPanel(targetPanelType))
                {
                    Debug.LogError($"过渡面板 {targetPanelType} 加载失败！");
                    return;
                }
                transitionPanel = PanelUIMgr.Instance.GetCachedPanel(targetPanelType);
            }
            else
            {
                PanelUIMgr.Instance.OpenPanel(targetPanelType);
            }

            if (transitionPanel is TransitionPanelBase transitionPanelImpl)
            {
                transitionPanelImpl.StartTransition(sceneName);
            }
        }

        private void OnDestroy()
        {
            MsgCenter.UnregisterMsg(MsgConst.OnTransitionFinnish, loadScene);
        }
    }
}