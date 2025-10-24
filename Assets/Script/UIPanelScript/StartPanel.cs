using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartPanel : UIPanelBase
{
    public Button btnGo;
    public Button btnExitGame;
    public Button btnSetting;
    private VideoPlayer _videoPlayer;
    protected override void OnShow()
    {
        btnGo.onClick.AddListener(() =>
        {
            btnGo.enabled = false;
            AudioMgr.Instance.PlaySfx("开始游戏");
            _videoPlayer = FindObjectOfType<VideoPlayer>();
            _videoPlayer.time = 0;
            _videoPlayer?.Play();
            AudioMgr.Instance.PauseBgm();

            _videoPlayer.loopPointReached += EndWithVideoPlay;
        });
        btnExitGame.onClick.AddListener(() =>
        {

        });
        btnSetting.onClick.AddListener(() =>
        {

        });
    }
    protected override void OnHide(Action onHideFinished)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          {
        btnGo.onClick.RemoveAllListeners();
        btnExitGame.onClick.RemoveAllListeners();
        btnSetting.onClick.RemoveAllListeners();

        onHideFinished?.Invoke();
    }


    public void EndWithVideoPlay(VideoPlayer source)
    {
        AudioMgr.Instance.ResumeBgm();

        SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
        TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
    }
}
