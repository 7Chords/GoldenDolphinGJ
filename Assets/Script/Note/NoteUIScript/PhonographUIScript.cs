using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhonographUIScript : MonoBehaviour, IPointerClickHandler
{
    bool currentPauseState = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        // 打开留声机 暂停时间流逝
        currentPauseState = !currentPauseState;
        NoteMgr.instance.SetPauseState(currentPauseState);

    }
}
