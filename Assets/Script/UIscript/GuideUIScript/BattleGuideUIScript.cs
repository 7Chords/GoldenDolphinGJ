using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleGuideUIScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<GameObject> guideUIList1;
    [SerializeField] private List<GameObject> guideUIList2;
    private GameObject curGuideUI;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (guideUIList1 == null || guideUIList2 == null) return;

        foreach (GameObject guideUI in guideUIList1)
        {
            guideUI.SetActive(false);
        }
        foreach (GameObject guideUI in guideUIList2)
        {
            guideUI.SetActive(false);
        }

        if (GameMgr.Instance.curLevel == 1 && GameMgr.Instance.IsLookLevel1Guide)
        {
            level1Guide();
            GameMgr.Instance.IsLookLevel1Guide = true;
        }
        else if (GameMgr.Instance.curLevel == 2 && GameMgr.Instance.IsLookLevel2Guide)
        {
            level2Guide();
            GameMgr.Instance.IsLookLevel2Guide = true;
        }
        else PanelUIMgr.Instance.ClosePanel(EPanelType.BattleGuidePanel);
    }

    private void level1Guide()
    {

        curGuideUI = guideUIList1[0];
        curGuideUI.SetActive(true);
    }

    private void level2Guide()
    {
        curGuideUI = guideUIList2[0];
        curGuideUI.SetActive(true);
    }
    public void ShowNextUIGuide1()
    {
        // 显示下一个引导UI，若无则关闭引导面板
        int curIndex = guideUIList1.IndexOf(curGuideUI);
        if (curIndex < guideUIList1.Count - 1)
        {
            curGuideUI.SetActive(false);
            curGuideUI = guideUIList1[curIndex + 1];
            curGuideUI.SetActive(true);
        }
        else PanelUIMgr.Instance.ClosePanel(EPanelType.BattleGuidePanel);
    }
    public void ShowNextUIGuide2()
    {
        // 显示下一个引导UI，若无则关闭引导面板
        int curIndex = guideUIList2.IndexOf(curGuideUI);
        if (curIndex < guideUIList2.Count - 1)
        {
            curGuideUI.SetActive(false);
            curGuideUI = guideUIList2[curIndex + 1];
            curGuideUI.SetActive(true);
        }
        else PanelUIMgr.Instance.ClosePanel(EPanelType.BattleGuidePanel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameMgr.Instance.curLevel == 1) ShowNextUIGuide1();
        else if(GameMgr.Instance.curLevel == 2) ShowNextUIGuide2();
    }
}
