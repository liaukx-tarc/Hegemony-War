using UnityEngine;
using UnityEngine.EventSystems;

public class TopBar : MonoBehaviour, IDragHandler
{
    public RectTransform uiPanel;

    public void OnDrag(PointerEventData eventData)
    {
        uiPanel.anchoredPosition += eventData.delta / UI_Controller.canvas.scaleFactor;
    }
}
