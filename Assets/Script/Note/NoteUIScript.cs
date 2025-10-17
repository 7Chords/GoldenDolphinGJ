using DG.Tweening;
using GJFramework;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NoteType
{
    HightNote,// 高音
    MiddleNote,// 中音符
    LowNote,// 低音符
    Default// 默认
}

public class NoteUIScript : MonoBehaviour, IPointerClickHandler
{
    public NoteType noteType;// 音符的类别
    public float existTime;// 存在时间
    [SerializeField] private float maxTime;// 随机时间
    [SerializeField] private float minTime;
    [SerializeField] private Image noteImage;// 音符对应的图片
    [SerializeField] private RectTransform transform;// 对应的UI框架
    private bool isPlayingAnimation = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        var tween1 = transform.DOSizeDelta(new Vector2(200, 200), 0.5f).SetEase(Ease.OutBack);

        // 改变透明度
        var tween2 = noteImage.DOFade(0.3f, 0.5f); // 0.3 表示更透明
    }

    private void Awake()
    {

        
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        // 每帧减少时间
        existTime -= Time.deltaTime;
        if(existTime <= 1f && isPlayingAnimation)
        {

        }
    }

    private void Init()
    {
        // 初始化的时候获得一个随机的时间
        existTime = Random.Range(maxTime, minTime);
    }

    

}
