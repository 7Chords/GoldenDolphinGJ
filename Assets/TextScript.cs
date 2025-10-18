using GJFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PanelUIMgr.Instance.OpenPanel(EPanelType.NoteCollectPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
