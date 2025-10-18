using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToCollectPage : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.AddNextScenePanel(EPanelType.NoteCollectPanel);
        TransitionMgr.Instance.StarTransition("NoteCollectScene", "FadeInAndOutTransition");
    }
}