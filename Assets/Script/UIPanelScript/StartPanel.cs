using GJFramework;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartPanel : UIPanelBase
{
    public Button btnGo;

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


        //SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
        //TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
    }

    public void EndWithVideoPlay(VideoPlayer source)
    {
        AudioMgr.Instance.ResumeBgm();

        SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
        TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
    }
    protected override void OnHide(Action onHideFinished)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          {
        btnGo.onClick.RemoveAllListeners();

        onHideFinished?.Invoke();
    }
}
