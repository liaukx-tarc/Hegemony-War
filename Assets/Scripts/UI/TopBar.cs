using UnityEngine;
using UnityEngine.EventSystems;

public class TopBar : MonoBehaviour, IDragHandler
{
    public RectTransform uiPanel;
    public Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        if (WorldController.instance.uiController.canvas != null)
            uiPanel.anchoredPosition += eventData.delta / WorldController.instance.uiController.canvas.scaleFactor;

        else
            uiPanel.anchoredPosition += eventData.delta /canvas.scaleFactor;
    }
}
