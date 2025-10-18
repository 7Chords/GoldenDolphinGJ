using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToBattleBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.AddNextScenePanel(EPanelType.BattlePanel);
        SceneLoader.Instance.LoadScene("BattleScene");
    }
}
