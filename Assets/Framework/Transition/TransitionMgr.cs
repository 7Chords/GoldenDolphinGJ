using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMgr : Singleton<TransitionMgr>
{
    private const string TRANSITION_PANEL_RESOURCE_PATH = "UI/TransitionsPrefabs/";
    // 面板根节点
    public Transform transitionRoot;

    private TransitionBase _curTransition;

    /// <summary>
    /// tragetSceneName = 目标场景名字
    /// transitionName = 过渡预制体名字
    /// </summary>
    /// <param name="tragetSceneName">目标场景名字</param>
    /// <param name="transitionName">过渡预制体名字</param>
    public void StarTransition(string tragetSceneName, string transitionName)
    {
        // 根据传入的名字加载过渡预制体
        TransitionBase tempTransition = LoadPanelFromResources(transitionName);
        if (tempTransition == null) return;

        // 设置要跳转的scene名字
        tempTransition.SetTragetSceneName(tragetSceneName);

        tempTransition.StartEnterTransition();
    }

    private TransitionBase LoadPanelFromResources(string transitionName)
    {
        string resourcePath = TRANSITION_PANEL_RESOURCE_PATH + transitionName;
        GameObject transitionsPrefab = Resources.Load<GameObject>(resourcePath);
        if (transitionsPrefab == null)
        {
            Debug.LogError($"找不到面板预制体: {resourcePath}");
            return null;
        }

        // 实例化面板并设置父节点
        GameObject transitionObj = GameObject.Instantiate(transitionsPrefab, transitionRoot);
        transitionObj.name = transitionName;

        // 获取面板组件
        TransitionBase transitionPanle = transitionObj.GetComponent<TransitionBase>();
        if (transitionPanle == null)
        {
            Debug.LogError($"面板 {transitionName} 缺少 TransitionBase 组件");
            // 销毁刚创建的 GameObject（不要访问 null 的 transitionPanle）
            GameObject.Destroy(transitionObj);
            return null;
        }

        // 安全关闭并删除旧过渡（由 CloseTransition 负责 Kill Tween 并销毁）
        if (_curTransition != null)
        {
            _curTransition.CloseTransition();
            _curTransition = null;
        }

        // 存储下一个过渡引用
        _curTransition = transitionPanle;

        return transitionPanle;
    }
}
