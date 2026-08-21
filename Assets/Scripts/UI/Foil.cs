using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
public class Foil : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [Header("Target")]
    [SerializeField] private RectTransform cardRoot;
    [SerializeField] private Image foilOverlay;
    
    [Header("Foil")]
    [SerializeField] private float rotateAmount = 10f;
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float opacity = 0.8f;
    
    private Material foilMat;
    private Quaternion targetRotation;

    private void Awake()
    {
        targetRotation = Quaternion.identity;

        foilMat = Instantiate(foilOverlay.material);
        foilOverlay.material = foilMat;
    }

    private void Update()
    {
        cardRoot.localRotation = Quaternion.Lerp(cardRoot.localRotation, targetRotation, Time.unscaledDeltaTime * smooth);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardRoot,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        var size = cardRoot.rect.size;

        var nx = Mathf.Clamp(localPoint.x / (size.x * 0.5f), -1f, 1f);
        var ny = Mathf.Clamp(localPoint.y / (size.y * 0.5f), -1f, 1f);

        var rotY = -nx * rotateAmount;
        var rotX = ny * rotateAmount;

        targetRotation = Quaternion.Euler(rotX, rotY, 0f);

        var foilPos = Vector4.zero;
        foilPos.x = Mathf.InverseLerp(-1f, 1f, nx);
        foilPos.y = Mathf.InverseLerp(-1f, 1f, ny);

        foilMat.SetVector("_FoilPosition", foilPos);
        foilMat.SetFloat("_Opacity", opacity);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        targetRotation = Quaternion.identity;
        foilMat.SetFloat("_Opacity", 0f);
    }
}
}
