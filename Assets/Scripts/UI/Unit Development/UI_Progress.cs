using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Progress : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI projectNameText;

    public TextMeshProUGUI maxHpText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI maintanceCostText;
    public TextMeshProUGUI produceCostText;

    public UnitDevelopmentProject selectedProject;
    public bool isAddingCity;
    public Image failImage;

    [Header("Button")]
    public Color viewColor;
    public Color addCityColor;
    public TextMeshProUGUI buttonText;
    public Image buttonBgr;

    public void OpenProgressUI()
    {
        gameObject.SetActive(true);

        infoPanel.SetActive(false);

        if(isAddingCity)
        {
            buttonBgr.color = addCityColor;
            buttonText.text = "ADD";
        }

        else
        {
            buttonBgr.color = viewColor;
            buttonText.text = "VIEW";
        }
    }

    public void DisablePlayerProject()
    {
        if (WorldController.instance.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.instance.currentPlayer;
            foreach (GameObject project in player.projectList)
            {
                project.SetActive(false);
            }
        }
    }

    public void ActivePlayerProject()
    {
        if (WorldController.instance.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.instance.currentPlayer;
            foreach (GameObject project in player.projectList)
            {
                project.SetActive(true);
            }
        }
    }
    public void ViewAddButton()
    {
        if(isAddingCity)
        {
            if(selectedProject.developmentCenters.Count < 7)
            {
                selectedProject.AddDevelopmentCenter(WorldController.instance.uiController.buildingUIController.selectedCity);

                isAddingCity = false;
                WorldController.instance.uiController.CloseAllUI();
            }
            
            else
            {
                StartCoroutine(AddCityFail());
            }
        }

        else
        {
            gameObject.SetActive(false);
            WorldController.instance.uiController.accessoriesUI.gameObject.SetActive(true);

            WorldController.instance.uiController.accessoriesUI.showTemplateDetail(selectedProject.unitProperty);
        }
        
    }

    public IEnumerator AddCityFail()
    {
        failImage.enabled = true;

        yield return new WaitForSeconds(0.5f);

        failImage.enabled = false;
    }

    public void ShowProjectDetail(UnitDevelopmentProject project)
    {
        selectedProject = project;
        projectNameText.text = project.projectName;

        maxHpText.text = project.unitProperty.maxHp.ToString();
        armorText.text = project.unitProperty.armor.ToString();
        damageText.text = project.unitProperty.damage.ToString();
        rangeText.text = project.unitProperty.range.ToString();
        speedText.text = project.unitProperty.speed.ToString();
        weightText.text = project.unitProperty.weight.ToString();

        maintanceCostText.text = project.unitProperty.maintanceCost.ToString();
        produceCostText.text = project.unitProperty.produceCost.ToString();

        ChangeTag(project.unitProperty);
        ChangeCity(project.developmentCenters);

        infoPanel.SetActive(true);
    }

    public List<TextMeshProUGUI> tagTextList;

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

        unitTagsList.Sort(WorldController.instance.uiController.accessoriesUI.CompareListByUnitTag);
        for (int i = 0; i < tagTextList.Count; i++)
        {
            if (i < unitTagsList.Count)
            {
                tagTextList[i].text = unitTagsList[i].ToString();
                tagTextList[i].transform.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tagTextList[0].transform.parent.GetComponent<RectTransform>());
            }

            else
            {
                tagTextList[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public List<DevelopmentCenter_Slot> developmentCenterSlotList;

    void ChangeCity(List<City> developmentCenters)
    {
        for (int i = 0; i < developmentCenterSlotList.Count; i++)
        {
            if (i < developmentCenters.Count)
            {
                developmentCenterSlotList[i].city = developmentCenters[i];
                developmentCenterSlotList[i].gameObject.SetActive(true);
                developmentCenterSlotList[i].cityName.text = developmentCenters[i].cityName;
                developmentCenterSlotList[i].cityNameBgr.color = developmentCenters[i].player.playerColor;
                developmentCenterSlotList[i].developmentPointText.text = developmentCenters[i].developmentPointIncome.ToString();
            }
            else
            {
                developmentCenterSlotList[i].gameObject.SetActive(false);
            }
        }
    }

    public void CancelProject()
    {
        do
        {
            selectedProject.RemoveDevelopmentCenter(selectedProject.developmentCenters[0]);

        } while (selectedProject.developmentCenters.Count > 0);


        selectedProject.player.projectList.Remove(selectedProject.gameObject);
        selectedProject.player.playerStartFunction -= selectedProject.ProjectCompleteCheck;
        WorldController.instance.playerStartFunction -= selectedProject.ProjectCompleteCheck;
        
        Destroy(selectedProject.gameObject);
        selectedProject = null;

        infoPanel.SetActive(false);
    }
}
