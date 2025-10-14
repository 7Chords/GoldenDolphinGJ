using GJFramework;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BagPanel : UIPanelBase
{
    [SerializeField] private GameObject btnGo;
    [SerializeField] private GameObject btnGo2;
    protected override void OnShow()
    {
        if(btnGo == null) Debug.LogError("btnGo IS NULL");
        Button btn = btnGo.GetComponent<Button>();
        if (btn == null) Debug.LogError("btn IS NULL");
        btn.onClick.AddListener(() =>
        {
            Debug.Log("BagPanel Btn Clicked");
            PanelUIMgr.Instance.GoBack();
        });
        Debug.Log($"{this.name} is Show!");
        Button btn2 = btnGo2.GetComponent<Button>();
        btn2.onClick.AddListener(() =>
        {
            OnLoadScene();
        });
    }

    protected override void OnHide()
    {
        Debug.Log($"{this.name} is Hide!");
    }

    public void OnLoadScene()
    {
        SceneLoader.Instance.AddNextScenePanel(EPanelType.StorePanel);
        TransitionMgr.Instance.StarTransition("SampleScene", "FadeInAndOutTransition");
    }
}
