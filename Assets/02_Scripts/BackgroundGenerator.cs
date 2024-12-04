using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundGenerator : MonoBehaviour, IPointerClickHandler {
    float lastClickTime = 0f;
    float doubleClickTimeThreshold = 0.3f;
    public Pause p;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickTimeThreshold)
        {
            // 더블 클릭 감지
            p.PauseOnClick();
            Debug.Log('a');
        }
        else
        {
            // 첫 번째 클릭 시간 기록
            lastClickTime = Time.time;
        }
    }
}
