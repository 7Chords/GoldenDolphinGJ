using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackUI : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PanelUIMgr.Instance.OpenPanel(EPanelType.LevelSelectPanel);
        PanelUIMgr.Instance.ClosePanel(EPanelType.StorePanel);
    }
}
