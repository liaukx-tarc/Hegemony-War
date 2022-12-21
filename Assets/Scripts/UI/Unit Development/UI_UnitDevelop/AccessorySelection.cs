using UnityEngine;
using UnityEngine.EventSystems;

public class AccessorySelection : MonoBehaviour, IPointerDownHandler
{ 
    public AccessoryProperty property;

    public void OnPointerDown(PointerEventData eventData)
    {
        WorldController.instance.uiController.ClickSound();
        if (Input.GetMouseButtonDown(0))
        {
            WorldController.instance.uiController.accessoriesUI.dragObj.rectTransform.position = Input.mousePosition;
            WorldController.instance.uiController.accessoriesUI.dragObj.image.sprite = property.icon;
            WorldController.instance.uiController.accessoriesUI.dragObj.accessory = this;

            WorldController.instance.uiController.accessoriesUI.dragObj.image.gameObject.SetActive(true);
            WorldController.instance.uiController.accessoriesUI.dragObj.isDraging = true;
        }
    }

    public void AccessoryFunction() { }
}
