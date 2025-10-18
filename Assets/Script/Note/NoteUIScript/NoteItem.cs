using GJFramework;
using UnityEngine;
using UnityEngine.UI;

public class NoteItem : MonoBehaviour
{
    public Text noteNumText;
    public NoteType noteType;

    // 设置信息
    public void SetInfo(int _noteNumText)
    {
        noteNumText.text = _noteNumText.ToString();
    }



}
