using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessorySelection : MonoBehaviour, IPointerDownHandler
{ 
    public AccessoryProperty property;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Input.GetMouseButtonDown(0))
        {
            DragObj.rectTransform.position = Input.mousePosition;
            DragObj.image.sprite = property.icon;
            DragObj.accessory = this;

            DragObj.image.gameObject.SetActive(true);
            DragObj.isDraging = true;
        }
    }

    public void AccessoryFunction() { }
}