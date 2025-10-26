using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuideUIScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<GameObject> guideUIList;

    private GameObject curGuideUI;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (guideUIList == null) return;
        foreach (GameObject guideUI in guideUIList)
        {
            guideUI.SetActive(false);
        }
        curGuideUI = guideUIList[0];
        curGuideUI.SetActive(true);
    }
    public void ShowNextUIGuide()
    {
        // 显示下一个引导UI，若无则关闭引导面板
        int curIndex = guideUIList.IndexOf(curGuideUI);
        if (curIndex < guideUIList.Count - 1)
        {
            curGuideUI.SetActive(false);
            curGuideUI = guideUIList[curIndex + 1];
            curGuideUI.SetActive(true);
        }
        else PanelUIMgr.Instance.ClosePanel(EPanelType.GuidePanel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowNextUIGuide();
    }
}
