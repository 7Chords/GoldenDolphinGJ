using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhonographUIScript : MonoBehaviour, IPointerClickHandler
{
    bool currentPauseState = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioMgr.Instance.PlaySfx("黑胶");
        // 打开留声机 暂停时间流逝
        currentPauseState = !currentPauseState;
        if (currentPauseState)
            AudioMgr.Instance.PauseBgm();
        else
            AudioMgr.Instance.ResumeBgm();

        NoteMgr.instance.SetPauseState(currentPauseState);

    }
}
