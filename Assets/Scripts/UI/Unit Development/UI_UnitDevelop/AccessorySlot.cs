using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessorySlot : MonoBehaviour, IDropHandler, IPointerUpHandler, IPointerDownHandler
{
    public AccessoriesTypes type;
    public AccessoryProperty accessory;
    public Image showingIcon;
    public bool isEquip = false;
    public int slotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        if (DragObj.isDraging)
        {
            if(equitAccessory(DragObj.accessory.property))
            {
                DragObj.isDraging = false;
                DragObj.image.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0) && DragObj.isDraging)
        {
            if(equitAccessory(DragObj.accessory.property))
            {
                DragObj.isDraging = false;
                DragObj.image.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1) && isEquip && UI_AccessoriesEquip.accessorySlotState)
        {
            removeAccessory();
        }
    }

   public bool equitAccessory(AccessoryProperty accessory)
    {
        if (isEquip)
            removeAccessory();

        if(UI_Controller.accessoriesUI.weight + accessory.weight > UI_Controller.accessoriesUI.transportProperty.load)
        {
            StartCoroutine(redEffect());
            Debug.Log("OverLoad");
            return false; //transport overload
        }

        foreach (AccessoriesTypes accessoryType in accessory.accessoryTypes)
        {
            if (accessoryType == type)
            {
                this.accessory = accessory;
                UI_Controller.accessoriesUI.equipAccessory(accessory, slotIndex);
                showingIcon.sprite = accessory.icon;
                showingIcon.enabled = true;
                isEquip = true;
                break;
            }
        }

        return true;
    }

    public void removeAccessory()
    {
        UI_Controller.accessoriesUI.removeAccessory(accessory, slotIndex);
        showingIcon.enabled = false;
        isEquip = false;
    }

    IEnumerator redEffect()
    {
        UI_Controller.accessoriesUI.weightText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        UI_Controller.accessoriesUI.weightText.color = Color.white;
    }
}
