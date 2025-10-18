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
            SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
            SceneLoader.Instance.LoadScene("LevelSelectScene");
        });
    }
    protected override void OnHide(Action onHideFinished)
    {
        btnGo.onClick.RemoveAllListeners();

        onHideFinished?.Invoke();
    }
}
