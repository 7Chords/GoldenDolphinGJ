using DG.Tweening;
using GJFramework;
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
    private float existTime;// 存在时间
    [SerializeField] private float maxTime;// 随机时间
    [SerializeField] private float minTime;
    [SerializeField] private Image noteImage;// 音符对应的图片
    [SerializeField] private RectTransform myTransform;// 对应的UI框架
    private bool isPlayingAnimation = false;
    private TweenContainer tweenContainer;
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
        // 每帧减少时间
        existTime -= Time.deltaTime;
        if(existTime <= 1f && !isPlayingAnimation)
        {
            OnNoteDisappear();
        }
    }
    private void Init()
    {
        // 音符种类随机
        noteType = GetRandomEnumAndSetImage();

        // 音符颜色随机
        if (noteImage == null)
        {
            Debug.Log("noteImage is null");
            return;
        }
        else noteImage.color = GetRandomColor();
        // 初始化的时候获得一个随机的时间
        existTime = Random.Range(maxTime, minTime);

        Sequence seq = DOTween.Sequence();
        // 更改大小
        seq.Append(myTransform.DOSizeDelta(new Vector2(200, 200), 1f).SetEase(Ease.OutBack));
        // 改变透明度
        seq.Join(noteImage.DOFade(1f, 0f).SetEase(Ease.InQuad));
        tweenContainer.RegDoTween(seq);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击播放的动画和时间到了一样 如果点击了看看是什么类型并且对应加上数量
        if (!isPlayingAnimation)
        {
            PlayerMgr.Instance.AddNoteNum(noteType);
            Debug.Log(noteType);
            Debug.Log(PlayerMgr.Instance.noteDic[noteType]);
            isPlayingAnimation = true;
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

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f); // RGB 随机 Alpha 为 1
    }

    public NoteType GetRandomEnumAndSetImage()
    {
        // todo: Set Note Image
        int temp = Random.Range(0, 3);
        if(temp == 0) return NoteType.HightNote;
        else if(temp == 1) return NoteType.MiddleNote;
        else return NoteType.LowNote;
    }

}
