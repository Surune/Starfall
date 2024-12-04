using UnityEngine;
using UnityEngine.EventSystems;

public class DragToMoveHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
    public GameObject character;
    public Transform trans;
    private float floatZ;
    private Vector3 fixedpos;

    public void OnDrag(PointerEventData eventData) {
        if (Time.timeScale != 0) {
            trans.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, trans.position.y, floatZ));
            trans.position = new Vector3(trans.position.x, fixedpos.y, fixedpos.z);

            bool limited = false;
        
            Vector3 worldpos = Camera.main.WorldToViewportPoint(trans.transform.position);

            if (worldpos.x < 0f) {
                worldpos.x = 0f;
                limited = true;
            }
            if (worldpos.y < 0f) {
                worldpos.y = 0f;
                limited = true;
            } 
            if (worldpos.x > 1f) {
                worldpos.x = 1f;
                limited = true;
            }
            if (worldpos.y > 1f) {
                worldpos.y = 1f;
                limited = true;
            }
            if (limited) { 
                trans.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        fixedpos = trans.position;
        floatZ = Vector3.Distance(Camera.main.transform.position, this.gameObject.transform.position);
        if (Time.timeScale != 0) OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) { }
}
