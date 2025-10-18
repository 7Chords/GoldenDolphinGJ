using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToBattleBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(PlayerMgr.Instance.instrumentIdList.Count == 0)
        {
            Debug.Log("Error当前无角色 ,直接返回");
            return;
        }
        SceneLoader.Instance.AddNextScenePanel(EPanelType.BattlePanel);
        TransitionMgr.Instance.StarTransition("BattleScene", "FadeInAndOutTransition");
    }
}
