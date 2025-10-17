using System;
using System.Collections;
using System.Collections.Generic;
using GJFramework;
using UnityEngine;

public class GameMgr : SingletonPersistent<GameMgr>
{
    public Transform UIRoot;
    public Transform TransitionRoot;


    private int _curLevel;
    public int curLevel => _curLevel;

    protected override void Awake()
    {
        base.Awake();
        GameInit();
    }

    void Start()
    {
        //StartCoroutine(Test1());
        
        PanelUIMgr.Instance.OpenPanel(EPanelType.StorePanel);
        PanelUIMgr.Instance.OpenPanel(EPanelType.BagPanel);
    }


    private void GameInit()
    {
        if (UIRoot != null) PanelUIMgr.Instance.panelRoot = UIRoot;
        else Debug.LogError("UI Root is Null");
        DontDestroyOnLoad(this.gameObject);

        DontDestroyOnLoad(UIRoot);

        if (TransitionRoot != null) TransitionMgr.Instance.transitionRoot = TransitionRoot;
        else Debug.LogError("TransitionRoot is Null");

        DontDestroyOnLoad(TransitionRoot);
    }

    


}
