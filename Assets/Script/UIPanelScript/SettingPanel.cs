using DG.Tweening;
using GJFramework;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIPanelBase
{
    public Slider sldBgm;
    public Slider sldSfx;
    public Button btnClose;

    public CanvasGroup canvasGroup;
    public float fadeInDuration;
    public float fadeOutDuration;

    public Text txtBgmFactor;
    public Text txtSfxFactor;


    private TweenContainer _tweenContainer;

    protected override void OnShow()
    {
        btnClose.enabled = false;
        _tweenContainer = new TweenContainer();
        canvasGroup.alpha = 0;
        _tweenContainer.RegDoTween(canvasGroup.DOFade(1, fadeInDuration)
            .OnComplete(()=> 
            {
                btnClose.enabled = true;
            }));
        sldBgm.value = AudioMgr.Instance.bgmVolumeFactor;
        sldSfx.value = AudioMgr.Instance.sfxVolumeFactor;
        btnClose.onClick.AddListener(() =>
        {
            AudioMgr.Instance.PlaySfx("木头按钮");
            PanelUIMgr.Instance.ClosePanel(EPanelType.SettingPanel);
        });
        sldBgm.onValueChanged.AddListener((float newVal) =>
        {
            AudioMgr.Instance.ChangeBgmVolume(newVal);
            RefreshShow();
        });
        sldSfx.onValueChanged.AddListener((float newVal) =>
        {
            AudioMgr.Instance.ChangeSfxVolume(newVal);
            RefreshShow();
        });
        RefreshShow();
    }

    private void RefreshShow()
    {
        txtBgmFactor.text = ((int)(AudioMgr.Instance.bgmVolumeFactor * 100)).ToString() + "%";
        txtSfxFactor.text = ((int)(AudioMgr.Instance.sfxVolumeFactor * 100)).ToString() + "%";

    }

    protected override void OnHide(Action onHideFinished)
    {

        btnClose.onClick.RemoveAllListeners();
        sldBgm.onValueChanged.RemoveAllListeners();
        sldSfx.onValueChanged.RemoveAllListeners();
        canvasGroup.alpha = 1;
        //_tweenContainer.RegDoTween(canvasGroup.DOFade(0, fadeOutDuration)
        //    .OnComplete(() =>
        //    {
        //        onHideFinished?.Invoke();
        //    }));
        onHideFinished?.Invoke();

    }

    private void OnDestroy()
    {
        _tweenContainer?.KillAllDoTween();
        _tweenContainer = null;
    }


}
