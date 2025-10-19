using DG.Tweening;
using GJFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NoteType
{
    HightNote,// 高音
    MiddleNote,// 中音符
    LowNote,// 低音符
    Trash1,// 垃圾音符1
    Trash2,// 垃圾音符2
    Trash3,// 垃圾音符3
    Trash4,// 垃圾音符4
}

public class NoteUIScript : MonoBehaviour, IPointerClickHandler
{
    private bool isUseful = false;
    private NoteType noteType;// 音符的类别
    private float existTime;// 存在时间
    [SerializeField] private float maxTime;// 随机时间
    [SerializeField] private float minTime;
    [SerializeField] private Image noteImage;// 音符对应的图片引用
    [SerializeField] private RectTransform myTransform;// 对应的UI框架
    private bool isPlayingAnimation = false; 
    private TweenContainer tweenContainer;
    [SerializeField] private List<Sprite> noteSpritList;// 音符对应的图片列表
    private bool isDestory = false;
    private void Awake()
    {
        tweenContainer = new TweenContainer();

    }
    private void Start()
    {
        // 初始化
        Init();
    }

    private void Update()
    {
        // 每帧减少时间 如果时间小于1秒并且没有播放动画 则播放消失动画
        DoInupdate();
    }

    private void DoInupdate()
    {
        if((NoteMgr.instance.isEnd || NoteMgr.instance.isBack) && !isDestory)
        {
            isDestory = true;
            OnNoteDisappear();
        }

        // 如果没有暂停则减少时间
        if (!NoteMgr.instance.IsCurrentPause)
        {
            // 如果有未播放的动画则继续播放
            if (isPlayingAnimation)
            {
                tweenContainer.ResumeAllDoTween();
            }
            existTime -= Time.deltaTime;
            if(existTime <= 1f && !isPlayingAnimation)
            {
                OnNoteDisappear();
            }
        }

    }
    private void Init()
    {
        // 音符种类随机
        noteType = GetRandomEnumAndSetImage();
        noteImage.SetNativeSize();
        // 音符颜色随机
        if (noteImage == null)
        {
            Debug.Log("noteImage is null");
            return;
        }
        // 初始化的时候获得一个随机的时间
        existTime = Random.Range(maxTime, minTime);

        Sequence seq = DOTween.Sequence();
        // 更改大小
        // 获取原始尺寸（动画前的初始尺寸）
        Vector2 originalSize = myTransform.sizeDelta;
        // 计算原始宽高比（关键：保持这个比例不变）
        float aspectRatio = originalSize.x / originalSize.y;

        // 目标尺寸
        Vector2 targetSize = originalSize * 0.5f;

        // 执行动画（使用计算后的目标尺寸，保持比例）
        seq.Append(myTransform.DOSizeDelta(targetSize, 1f).SetEase(Ease.OutBack));
        // 改变透明度
        seq.Join(noteImage.DOFade(1f, 0f).SetEase(Ease.InQuad));
        tweenContainer.RegDoTween(seq);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击播放的动画和时间到了一样 如果点击了看看是什么类型并且对应加上数量
        if (!isPlayingAnimation && NoteMgr.instance.IsCurrentPause && isUseful)
        {
            switch (noteType)
            {
                case NoteType.HightNote:
                    AudioMgr.Instance.PlaySfx("high");
                    break;
                case NoteType.MiddleNote:
                    AudioMgr.Instance.PlaySfx("middle");
                    break;
                case NoteType.LowNote:
                    AudioMgr.Instance.PlaySfx("low");
                    break;
            }

            NoteMgr.instance.ReduceRemainNoteNum();
            PlayerMgr.Instance.AddNoteNum(noteType);
            isPlayingAnimation = true;
            tweenContainer.PauseAllDoTween();
            OnNoteDisappear();
        }
    }
    private void OnDestroy()
    {
        tweenContainer?.KillAllDoTween();
        tweenContainer = null;
    }

    private void OnNoteDisappear()
    {

        isPlayingAnimation = true;
        // 点击的时候播放回收的动画
        Sequence seq = DOTween.Sequence();
        seq.Append(myTransform.DOSizeDelta(new Vector2(0, 0), 1f).SetEase(Ease.OutBack));
        // 播放完就直接Destroy
        seq.Join(noteImage.DOFade(0f, 1f).SetEase(Ease.InQuad).OnComplete(() => {
            Destroy(this.gameObject);
        }));
        tweenContainer.RegDoTween(seq);
    }

    public NoteType GetRandomEnumAndSetImage()
    {

        Sprite spriteHightNote = noteSpritList[2];
        Sprite spriteLowNote = noteSpritList[1];
        Sprite spriteMiddleNote = noteSpritList[0];

        // 5 : 3 : 2 高 中 低
        int temp = Random.Range(0, 18);
        if (temp >= 0 && temp <= 9) isUseful = true;

        if (temp >= 0 && temp <= 4)
        {
            noteImage.sprite = spriteHightNote;
            return NoteType.HightNote;
        }
        else if (temp >= 5 && temp <= 7)
        {
            noteImage.sprite = spriteMiddleNote;
            return NoteType.MiddleNote;
        }
        else if(temp >= 8 && temp <= 9)
        {
            noteImage.sprite = spriteLowNote;
            return NoteType.LowNote;
        }
        // 如果随机到无用的音符的话
        temp = Random.Range(3, 7);
        noteImage.sprite = noteSpritList[temp];
        if (temp == 3) return NoteType.Trash1;
        else if (temp == 4) return NoteType.Trash2;
        else if (temp == 5) return NoteType.Trash3;
        else return NoteType.Trash4; 
    }
}
