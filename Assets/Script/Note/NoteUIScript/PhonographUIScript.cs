using GJFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhonographUIScript : MonoBehaviour, IPointerClickHandler
{
    bool currentPauseState = true;

    bool hasStartPlay = false;
    private void OnEnable()
    {
        currentPauseState = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        AudioMgr.Instance.PlaySfx("黑胶");
        // 打开留声机 暂停时间流逝
        currentPauseState = !currentPauseState;

        if(!hasStartPlay)
        {
            hasStartPlay = true;
            AudioMgr.Instance.PlayBgm("土耳其");
        }
        else
        {
            if (currentPauseState)
                AudioMgr.Instance.PauseBgm();
            else
                AudioMgr.Instance.ResumeBgm();
        }

        NoteMgr.instance.SetPauseState(currentPauseState);
        // 点击过就标记开始
        NoteMgr.instance.IsStart = true;
    }

    private void Start()
    {
        hasStartPlay = false;
        currentPauseState = true;
    }

}
