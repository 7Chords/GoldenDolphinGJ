using System;
using System.Collections;
using System.Collections.Generic;
using GJFramework;
using UnityEngine;

public class GameMgr : SingletonPersistent<GameMgr>
{
    public Transform UIRoot;
    public Transform TransitionRoot;

    private int playerMaxLevel;
    private int _curLevel;

    public int PlayerMaxLevel
    {
        get
        {
            return playerMaxLevel;
        }
        set
        {
            playerMaxLevel = value;
        }
    }
    public int curLevel
    {
        get
        {
            return _curLevel;
        }
        set
        {
            _curLevel = value;
        }
    }

    [Header("玩家姓名")]
    public string playerName;

    protected override void Awake()
    {
        base.Awake();
        GameInit();
    }

    void Start()
    {
        PanelUIMgr.Instance.OpenPanel(EPanelType.StartPanel);
        AudioMgr.Instance.PlayBgm("背景音乐");
    }


    private void GameInit()
    {
        playerMaxLevel = 1;
        //todo:test
        //_curLevel = 3;
        SCRefDataMgr.Instance.Init();

        if (UIRoot != null) PanelUIMgr.Instance.panelRoot = UIRoot;
        else Debug.LogError("UI Root is Null");
        //DontDestroyOnLoad(this.gameObject);

        //DontDestroyOnLoad(UIRoot);

        if (TransitionRoot != null) TransitionMgr.Instance.transitionRoot = TransitionRoot;
        else Debug.LogError("TransitionRoot is Null");

        //DontDestroyOnLoad(TransitionRoot);
    }

    


}
