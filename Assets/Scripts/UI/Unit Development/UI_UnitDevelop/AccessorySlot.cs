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
        if (WorldController.instance.uiController.accessoriesUI.dragObj.isDraging)
        {
            if(equitAccessory(WorldController.instance.uiController.accessoriesUI.dragObj.accessory.property))
            {
                WorldController.instance.uiController.accessoriesUI.dragObj.isDraging = false;
                WorldController.instance.uiController.accessoriesUI.dragObj.image.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0) && WorldController.instance.uiController.accessoriesUI.dragObj.isDraging)
        {
            if(equitAccessory(WorldController.instance.uiController.accessoriesUI.dragObj.accessory.property))
            {
                WorldController.instance.uiController.accessoriesUI.dragObj.isDraging = false;
                WorldController.instance.uiController.accessoriesUI.dragObj.image.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1) && isEquip && WorldController.instance.uiController.accessoriesUI.accessorySlotState)
        {
            removeAccessory();
        }
    }

   public bool equitAccessory(AccessoryProperty accessory)
    {
        if (isEquip)
            removeAccessory();

        if(WorldController.instance.uiController.accessoriesUI.weight + accessory.weight > WorldController.instance.uiController.accessoriesUI.transportProperty.load)
        {
            StartCoroutine(redEffect());
            return false; //transport overload
        }

        foreach (AccessoriesTypes accessoryType in accessory.accessoryTypes)
        {
            if (accessoryType == type)
            {
                this.accessory = accessory;
                WorldController.instance.uiController.accessoriesUI.equipAccessory(accessory, slotIndex);
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
        WorldController.instance.uiController.accessoriesUI.removeAccessory(accessory, slotIndex);
        showingIcon.enabled = false;
        isEquip = false;
    }

    IEnumerator redEffect()
    {
        WorldController.instance.uiController.accessoriesUI.weightText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        WorldController.instance.uiController.accessoriesUI.weightText.color = Color.white;
    }
}
