using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectPageBackScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.AddNextScenePanel(EPanelType.LevelSelectPanel);
        TransitionMgr.Instance.StarTransition("LevelSelectScene", "FadeInAndOutTransition");
    }
}
