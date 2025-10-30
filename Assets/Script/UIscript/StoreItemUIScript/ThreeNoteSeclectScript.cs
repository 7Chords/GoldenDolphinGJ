using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeNoteSeclectScript : MonoBehaviour
{
    [SerializeField] private Sprite[] noteSprites;
    [SerializeField] private Image noteImage;
    [SerializeField] private Text noteNumText;

    public void SetNoteImageInfo(int index)
    {
        if(index < 0 || index >= noteSprites.Length)
            return;
        noteImage.sprite = noteSprites[index];
        noteImage.SetNativeSize();
    }

    public void SetNoteNumText(int num)
    {
        noteNumText.text = 'X' + num.ToString();
    }
}
