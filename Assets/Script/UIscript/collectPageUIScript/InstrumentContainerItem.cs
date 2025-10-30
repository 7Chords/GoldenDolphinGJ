using GJFramework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
// 用于在容器中表示单个乐器的简单组件
// 继承自 MonoBehaviour，只有一个用于序列化引用的 Image 字段
// 提供 SetInfo 方法设置该图片的显示内容
/// </summary>
public class InstrumentContainerItem : MonoBehaviour
{
    [Header("乐器图标（序列化引用）")]
    [SerializeField]
    private Image instrumentImage;
    private long currentInstrumentId;
    
    [Header("显隐指示图标（序列化引用）")]
    [SerializeField]
    private Image visibilityImage;

    /// <summary>
    /// 用于在代码中设置显示的图片（Sprite）。
    /// 传入 null 会隐藏图片显示。
    /// </summary>
    public void SetInfo(Sprite _sprite, Sprite _sprite2, long _currentInstrumentId)
    {
        if (instrumentImage == null)
        {
            Debug.LogWarning("InstrumentContainerItem: instrumentImage 未绑定，请在 Inspector 中设置引用。");
            return;
        }
        currentInstrumentId = _currentInstrumentId;
        instrumentImage.sprite = _sprite;
        instrumentImage.enabled = _sprite != null;
        visibilityImage.sprite = _sprite2;
        visibilityImage.enabled = _sprite2 != null;
    }

    public void refreshIcon()
    {
        // 需要根据某些条件来设置 visibilityImage 的显隐
        ApplyVisibilityBasedOnCondition();
    }
    public void ApplyVisibilityBasedOnCondition()
    {
        // todo: 根据实际条件设置 visible 变量
        InstrumentStoreRefObj instrumentStoreRefObj = SCRefDataMgr.Instance.instrumentStoreRefList.refDataList
            .Find(x => x.instrumentId == currentInstrumentId);
        
        if(instrumentStoreRefObj == null)
        {
            Debug.LogError("instrumentStoreRefObj is null！");
            return;
        }

        // 当前资源足够某个乐器所需要的 则显示指示图标
        bool visible = PlayerMgr.Instance.GetNoteNum(NoteType.HightNote) >= instrumentStoreRefObj.hightNoteNum &&
                       PlayerMgr.Instance.GetNoteNum(NoteType.LowNote) >= instrumentStoreRefObj.lowNoteNum &&
                       PlayerMgr.Instance.GetNoteNum(NoteType.MiddleNote) >= instrumentStoreRefObj.middleNoteNum;


        if(visible)
        {
            instrumentImage.enabled = true;
            visibilityImage.enabled = false;
        }
        else
        {
            instrumentImage.enabled = false;
            visibilityImage.enabled = true;
        }

        InstrumentRefObj instrumentRefObj = SCRefDataMgr.Instance.instrumentRefList.refDataList
            .Find(x => x.id == currentInstrumentId);

        bool isNeedToshow = GameMgr.Instance.curLevel >= instrumentRefObj.unlockLevelId;
        this.gameObject.SetActive(isNeedToshow);
    }

    /// <summary>
    /// 控制 visibilityImage 的显隐（具体实现：直接设置 Image 的 enabled）。
    /// </summary>
    /// <param name="visible">是否显示指示图标</param>
    public void SetVisibility(bool visible)
    {
        if (visibilityImage == null)
        {
            Debug.LogWarning("InstrumentContainerItem: visibilityImage 未绑定，请在 Inspector 中设置引用。");
            return;
        }

        visibilityImage.enabled = visible;
    }
}