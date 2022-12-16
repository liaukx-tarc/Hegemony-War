using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitTemplateList : MonoBehaviour
{
    public RectTransform UT_RectTransform;
    Vector3 UT_Position;

    public GameObject unitTemplateUI;
    public GameObject unitTemplateListObj;
    public GameObject templateInfoPanel;
    public GameObject noneText;
    public TextMeshProUGUI templateLusrTitle;

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

    public bool isModify;
    public bool isUpgrade;

    [Header("Button")]
    public Image detailBtnBackground;
    public TextMeshProUGUI detailBtnText;
    public Color modifyColor;
    public Color upgradeColor;
    public Color viewColor;

    const string Template = " Template";
    const string View = "View";
    const string Modify = "Modify";
    const string Upgrade = "Upgrade";

    public void UnitTemplateUI_Start()
    {

        UI_Controller.closeAllUIFunction += CloseTemplateUI;

        UT_Position = UT_RectTransform.anchoredPosition;
    }

    public bool tempModify;
    public bool tempUpgrade;

    public void UpdateTemplateList()
    {
        UT_RectTransform.anchoredPosition = UT_Position;

        tempModify = isModify;
        tempUpgrade = isUpgrade;

        UI_Controller.closeAllUIFunction();
        WorldController.UI.EnableScreenBlock();

        isModify = tempModify;
        isUpgrade = tempUpgrade;

        unitTemplateUI.SetActive(true);

        if (isModify)
        {
            templateLusrTitle.text = Modify + Template;
            detailBtnBackground.color = modifyColor;
            detailBtnText.text = Modify;
        }

        else if (isUpgrade)
        {
            templateLusrTitle.text = Upgrade + Template;
            detailBtnBackground.color = upgradeColor;
            detailBtnText.text = Upgrade;
        }

        else
        {
            templateLusrTitle.text = View + Template;
            detailBtnBackground.color = viewColor;
            detailBtnText.text = View;
        }
    }

    public void OpenTemplateUI()
    {
        UpdateTemplateList();

        foreach (Player player in WorldController.playerList)
        {
            if(player == WorldController.currentPlayer)
            {
                foreach (UnitTemplate unitTemplate in player.unitTemplateList)
                {
                    unitTemplate.gameObject.SetActive(true);
                }
            }

            else
            {
                foreach (UnitTemplate unitTemplate in player.unitTemplateList)
                {
                    unitTemplate.gameObject.SetActive(false);
                }
            }
        }


        if (WorldController.currentPlayer.unitTemplateList.Count > 0)
            noneText.SetActive(false);
        else
            noneText.SetActive(true);
    }

    public void OpenTemplateUI(TransportType transportType)
    {
        int templateCount = 0;

        UpdateTemplateList();

        foreach (Player player in WorldController.playerList)
        {
            if (player == WorldController.currentPlayer)
            {
                foreach (UnitTemplate unitTemplate in player.unitTemplateList)
                {
                    if (unitTemplate.property.transportProperty.transportType == transportType)
                    {
                        unitTemplate.gameObject.SetActive(true);
                        templateCount++;
                    }
                    else
                        unitTemplate.gameObject.SetActive(false);
                }
            }

            else
            {
                foreach (UnitTemplate unitTemplate in player.unitTemplateList)
                {
                    unitTemplate.gameObject.SetActive(false);
                }
            }
        }


        if (templateCount > 0)
            noneText.SetActive(false);
        else
            noneText.SetActive(true);
    }

    public void CloseTemplateUI()
    {
        unitTemplateUI.SetActive(false);
        templateInfoPanel.SetActive(false);

        WorldController.UI.DisableScreenBlock();

        isModify = false;
        isUpgrade = false;
    }

    public void ShowTemplateInfo(UnitTemplate unitTemplate)
    {
        selectedTemplate = unitTemplate;

        ChangeTag(selectedTemplate.property);

        for (int i = 0; i < selectedTemplate.property.accessoryProperty.Length; i++)
        {
            AccessoryProperty accessory = selectedTemplate.property.accessoryProperty[i];

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
                heavyWeaponSlots[i].gameObject.SetActive(true);
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
                mediumWeaponSlots[i - 3].gameObject.SetActive(true);
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
                lightWeaponSlots[i - 6].gameObject.SetActive(true);
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
                defenceEquipmentSlots[i - 9].gameObject.SetActive(true);
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
                auxiliaryEquipmentSlots[i - 12].gameObject.SetActive(true);
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
                fireControlSystemSlots.gameObject.SetActive(true);
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
                engineSlots.gameObject.SetActive(true);
            }
        }

        templateInfoPanel.SetActive(true);
    }

    public void onClickFunction()
    {
        if (isModify)
            UI_Controller.accessoriesUI.UIOpen(UI_Controller.buildingUIController.selectedCity, ProjectType.UnitModify, selectedTemplate.property.transportProperty.transportType, selectedTemplate);

        else if (isUpgrade)
            UI_Controller.accessoriesUI.UIOpen(UI_Controller.buildingUIController.selectedCity, ProjectType.UnitUpgrade, selectedTemplate.property.transportProperty.transportType, selectedTemplate);

        else
            UI_Controller.accessoriesUI.showTemplateDetail(selectedTemplate.property);

        WorldController.UI.OpenAccessoriesUI();
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
}
