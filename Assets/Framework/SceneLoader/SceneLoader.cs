using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace GJFramework
{

    // 过度的效果是通过展示一个 panel来实现的
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
        //Single ：删除所有运行的Scene 然后加载这个scene
        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        //执行过一次后会置空
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
            // 默认预设 不设置也行 因为可能不会用到
            if (!presetDict.ContainsKey(_defaultPresetName))
            {
                Debug.LogWarning($"默认预设 {_defaultPresetName} 不存在，请在Inspector中配置！");
            }
        }

        /// <summary>
        /// 加载过渡场景
        /// </summary>
        /// <param name="loadSceneName">要加载到的场景</param>
        /// <param name="closeSceneName">要关闭的场景</param>
        /// <param name="presetName">过渡动画的名字</param>
        /// <param name="loadSceneMode">场景模式 暂时用不到 也没做逻辑</param>
        public void LoadSceneWithTransition(string loadSceneName, string closeSceneName, string presetName = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            string usedPresetName = presetName ?? _defaultPresetName;

            _presetName = usedPresetName;
            _loadSceneMode = loadSceneMode;
            _closeSceneName = closeSceneName;
            if (!presetDict.TryGetValue(_presetName, out var PresetPanelType))
            {
                Debug.LogError("没有这个过度Panel");
                return;
            }
            // 播放过渡动画 什么时候结束 怎么过渡这里不关心 只开启
            PlayTransitionPanel(PresetPanelType, loadSceneName);
        }

        // 加载场景函数
        private void loadScene(object[] _objs)
        {
            if (_objs == null || _objs.Length == 0) 
            {
                Debug.LogError("请检查过渡动画回调加载场景的传入SceneName是否有误");
            }

            string sceneName = _objs[0].ToString();
            // 检查场景是否存在
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"场景 {sceneName} 不存在");
                return;
            }
            // 异步加载新场景
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, _loadSceneMode);
            asyncOperation.allowSceneActivation = false; // 手动控制场景激活

            // 卸载旧场景（如果存在）
            if (!string.IsNullOrEmpty(_closeSceneName))
            {
                SceneManager.UnloadSceneAsync(_closeSceneName).completed += _ =>
                {
                    _closeSceneName = null;
                };
            }

            // 启动协程监控加载进度
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        private IEnumerator MonitorLoadingProgress(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0-1标准化
                Debug.Log($"加载进度: {progress * 100}%");

                // 当进度>=90%时允许激活场景
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
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

        // 取出并播放对应效果
        // 这里只负责打开 用Dotween的onComplete回调来关闭面板( ClosePanel(EpanelType))
        private void PlayTransitionPanel(EPanelType targetPanelType, string sceneName)
        {
            UIPanelBase transitionPanel = PanelUIMgr.Instance.GetCachedPanel(targetPanelType);

            // 如果缓存中没有，就通过PanleUIMgr加载并打开（自动加入缓存）
            if (transitionPanel == null)
            {
                // 调用OpenPanel
                if (!PanelUIMgr.Instance.OpenPanel(targetPanelType))
                {
                    Debug.LogError($"过渡面板 {targetPanelType} 加载失败！");
                    return; // 加载失败直接退出，避免后续报错
                }
                // 加载后重新从缓存获取实例
                transitionPanel = PanelUIMgr.Instance.GetCachedPanel(targetPanelType);
            }
            else
            {
                // 缓存中已有，直接打开
                PanelUIMgr.Instance.OpenPanel(targetPanelType);
            }

            if (transitionPanel is TransitionPanelBase transitionPanelImpl)
            {
                // 传入某个过渡panel 要切换到的场景名字
                transitionPanelImpl.StartTransition(sceneName); 
            }

        }
        private void OnDestroy()
        {
            MsgCenter.UnregisterMsg(MsgConst.OnTransitionFinnish, loadScene);
        }
    }
}