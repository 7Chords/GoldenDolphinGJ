using UnityEngine;
using UnityEngine.UI;

public class NoteItem : MonoBehaviour
{
    public Image notemage;
    public Text noteNumText;

    // 设置信息
    public void SetInfo(Image _image, Text _noteNumText)
    {
        notemage = _image;
        noteNumText = _noteNumText;
    }
}
