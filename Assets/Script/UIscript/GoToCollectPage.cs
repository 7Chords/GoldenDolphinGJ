using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToCollectPage : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioMgr.Instance.PlaySfx("木头按钮");
        SceneLoader.Instance.AddNextScenePanel(EPanelType.NoteCollectPanel);
        TransitionMgr.Instance.StarTransition("NoteCollectScene", "FadeInAndOutTransition");
    }
}