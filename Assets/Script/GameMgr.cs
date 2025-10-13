using System;
using System.Collections;
using System.Collections.Generic;
using GJFramework;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public Transform UIRoot;
    

    private void Awake()
    {
        if(UIRoot != null) PanelUIMgr.Instance._panelRoot = UIRoot;
        else Debug.LogError("UI Root is Null");
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //StartCoroutine(Test1());
        
        PanelUIMgr.Instance.OpenPanel(EPanelType.StorePanel);
        PanelUIMgr.Instance.OpenPanel(EPanelType.BagPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Test1()
    {
        yield return new WaitForSeconds(2f);
        PanelUIMgr.Instance.OpenPanel(EPanelType.BagPanel);
        yield return new WaitForSeconds(2f);
        PanelUIMgr.Instance.OpenPanel(EPanelType.StorePanel);
        yield return new WaitForSeconds(2f);
        PanelUIMgr.Instance.GoBack();

    }

    


}
