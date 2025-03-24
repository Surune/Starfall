using UnityEngine;
using UnityEngine.EventSystems;

public class DragToMoveHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject Character;
    public Transform Trans;
    float floatZ;
    Vector3 fixedpos;

    public void OnDrag(PointerEventData eventData)
    {
        if (Time.timeScale != 0)
        {
            Trans.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, Trans.position.y, floatZ));
            Trans.position = new Vector3(Trans.position.x, fixedpos.y, fixedpos.z);

            bool limited = false;

            Vector3 worldpos = Camera.main.WorldToViewportPoint(Trans.transform.position);

            if (worldpos.x < 0f)
            {
                worldpos.x = 0f;
                limited = true;
            }
            if (worldpos.y < 0f)
            {
                worldpos.y = 0f;
                limited = true;
            }
            if (worldpos.x > 1f)
            {
                worldpos.x = 1f;
                limited = true;
            }
            if (worldpos.y > 1f)
            {
                worldpos.y = 1f;
                limited = true;
            }
            if (limited)
            {
                Trans.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        fixedpos = Trans.position;
        floatZ = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);
        if (Time.timeScale != 0)
        {
            OnDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //do nothing
    }
}
