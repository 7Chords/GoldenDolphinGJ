using GJFramework;
using System;
using UnityEngine.UI;

public class StartPanel : UIPanelBase
{
    public Button btnGo;
    protected override void OnShow()
    {
        btnGo.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlaySfx("开始游戏");
            SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
            TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
        });
    }
    protected override void OnHide(Action onHideFinished)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          {
        btnGo.onClick.RemoveAllListeners();

        onHideFinished?.Invoke();
    }
}
