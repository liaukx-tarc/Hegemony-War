using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UnitTemplateList : MonoBehaviour
{
    public GameObject unitTemplateUI;
    public GameObject unitTemplateListObj;
    public GameObject templateInfoPanel;
    public GameObject noneText;

    [Header("Tag List")]
    public List<TextMeshProUGUI> tagTextArray;
    public List<UnitTag> unitTagsList = new List<UnitTag>();

    [Header("Accessory Slots")]
    public AccessorySlot[] heavyWeaponSlots;
    public AccessorySlot[] mediumWeaponSlots;
    public AccessorySlot[] lightWeaponSlots;
    public AccessorySlot[] defenceEquipmentSlots;
    public AccessorySlot[] auxiliaryEquipmentSlots;
    public AccessorySlot fireControlSystemSlots;
    public AccessorySlot engineSlots;

    public UnitTemplate selectedTemplate;
    public UnitProperty selectedUnitProperty;

    public void UnitTemplateUI_Start()
    {
        WorldController.playerEndFunction += DisablePlayerUnitTemplate;
        WorldController.playerStartFunction += ActivePlayerUnitTemplate;
        UI_Controller.closeAllUIFunction += CloseTemplateUI;
    }

    public void OpenTemplateUI()
    {
        UI_Controller.closeAllUIFunction();
        unitTemplateUI.SetActive(true);

        if (WorldController.currentPlayer.unitTemplateList.Count > 0)
            noneText.SetActive(false);
        else
            noneText.SetActive(true);
    }

    public void CloseTemplateUI()
    {
        unitTemplateUI.SetActive(false);
        templateInfoPanel.SetActive(false);
        selectedTemplate = null;
    }

    public void ShowTemplateInfo(UnitTemplate unitTemplate)
    {
        selectedTemplate = unitTemplate;
        selectedUnitProperty = selectedTemplate.property;

        ChangeTag(selectedUnitProperty);

        for (int i = 0; i < selectedUnitProperty.accessoryProperty.Length; i++)
        {
            AccessoryProperty accessory = selectedUnitProperty.accessoryProperty[i];

            if (i < 3) //Heavy Weapon
            {
                if (accessory == null)
                {
                    heavyWeaponSlots[i].gameObject.SetActive(false);
                    continue;
                }

                heavyWeaponSlots[i].accessory = accessory;
                heavyWeaponSlots[i].showingIcon.sprite = accessory.icon;
                heavyWeaponSlots[i].showingIcon.enabled = true;
                heavyWeaponSlots[i].isEquip = true;
            }

            else if (i < 6)
            {
                if (accessory == null)
                {
                    mediumWeaponSlots[i - 3].gameObject.SetActive(false);
                    continue;
                }

                mediumWeaponSlots[i - 3].accessory = accessory;
                mediumWeaponSlots[i - 3].showingIcon.sprite = accessory.icon;
                mediumWeaponSlots[i - 3].showingIcon.enabled = true;
                mediumWeaponSlots[i - 3].isEquip = true;
            }

            else if (i < 9)
            {
                if (accessory == null)
                {
                    lightWeaponSlots[i - 6].gameObject.SetActive(false);
                    continue;
                }

                lightWeaponSlots[i - 6].accessory = accessory;
                lightWeaponSlots[i - 6].showingIcon.sprite = accessory.icon;
                lightWeaponSlots[i - 6].showingIcon.enabled = true;
                lightWeaponSlots[i - 6].isEquip = true;
            }

            else if (i < 12)
            {
                if (accessory == null)
                {
                    defenceEquipmentSlots[i - 9].gameObject.SetActive(false);
                    continue;
                }

                defenceEquipmentSlots[i - 9].accessory = accessory;
                defenceEquipmentSlots[i - 9].showingIcon.sprite = accessory.icon;
                defenceEquipmentSlots[i - 9].showingIcon.enabled = true;
                defenceEquipmentSlots[i - 9].isEquip = true;
            }

            else if (i < 16)
            {
                if (accessory == null)
                {
                    auxiliaryEquipmentSlots[i - 12].gameObject.SetActive(false);
                    continue;
                }

                auxiliaryEquipmentSlots[i - 12].accessory = accessory;
                auxiliaryEquipmentSlots[i - 12].showingIcon.sprite = accessory.icon;
                auxiliaryEquipmentSlots[i - 12].showingIcon.enabled = true;
                auxiliaryEquipmentSlots[i - 12].isEquip = true;
            }

            else if (i < 17)
            {
                if (accessory == null)
                {
                    fireControlSystemSlots.gameObject.SetActive(false);
                    continue;
                }

                fireControlSystemSlots.accessory = accessory;
                fireControlSystemSlots.showingIcon.sprite = accessory.icon;
                fireControlSystemSlots.showingIcon.enabled = true;
                fireControlSystemSlots.isEquip = true;
            }

            else if (i < 18)
            {
                if (accessory == null)
                {
                    engineSlots.gameObject.SetActive(false);
                    continue;
                }

                engineSlots.accessory = accessory;
                engineSlots.showingIcon.sprite = accessory.icon;
                engineSlots.showingIcon.enabled = true;
                engineSlots.isEquip = true;
            }
        }

        templateInfoPanel.SetActive(true);
    }

    public void ShowTemplateDetail()
    {
        WorldController.UI.OpenUnitDevelopmentUI();
        UI_Controller.accessoriesUI.showTemplateDetail(selectedUnitProperty);

        CloseTemplateUI();
    }

    void ChangeTag(UnitProperty unitProperty)
    {
        List<UnitTag> unitTagsList = new List<UnitTag>();

        foreach (AccessoryProperty accessory in unitProperty.accessoryProperty)
        {
            if (accessory != null && !unitTagsList.Contains(accessory.accessoryTag) && accessory.accessoryTag != UnitTag.None)
            {
                unitTagsList.Add(accessory.accessoryTag);
            }
        }

        unitTagsList.Sort(UI_AccessoriesEquip.CompareListByUnitTag);
        for (int i = 0; i < tagTextArray.Count; i++)
        {
            if (i < unitTagsList.Count)
            {
                tagTextArray[i].text = unitTagsList[i].ToString();
                tagTextArray[i].transform.parent.gameObject.SetActive(true);
            }

            else
            {
                tagTextArray[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void DisablePlayerUnitTemplate()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.currentPlayer;
            foreach (UnitTemplate unitTemplate in player.unitTemplateList)
            {
                unitTemplate.gameObject.SetActive(false);
            }
        }
    }

    public void ActivePlayerUnitTemplate()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.currentPlayer;
            foreach (UnitTemplate unitTemplate in player.unitTemplateList)
            {
                unitTemplate.gameObject.SetActive(true);
            }
        }
    }
}
