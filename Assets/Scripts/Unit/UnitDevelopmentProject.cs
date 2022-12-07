using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ProjectType
{
    UnitDevelopment,
    UnitModify,
    UnitUpgrade
}

public class UnitDevelopmentProject : MonoBehaviour
{
    public Player player;

    [Header("Icon")]
    public Image iconBackground;
    public Image icon;

    public Color developmentColor;
    public Sprite developmentIcon;  
    
    public Color modifyColor;
    public Sprite modifyIcon; 
    
    public Color upgradeColor;
    public Sprite upgradeIcon;

    [Header("Basic Info")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI turnText;
    public Slider progressBar;
    public ProjectType projectType;

    //Cost
    [Header("Cost Info")]
    public TextMeshProUGUI completeDPText;
    public TextMeshProUGUI maxDPText;
    public TextMeshProUGUI budgetText;

    [Header("Data")]
    public string projectName;
    public int projectBudget;
    public int remainDevelopCost;

    public UnitTemplate originalTemplate;
    public GameObject unitTemplatePrefab;
    public UnitProperty unitProperty;
    public TagController.UnitFunction runtimeFunction;

    public int developmentPoint = 0;
    public List<Building> developmentCenters = new List<Building>();

    public void ProjectCreate(Player player, Building developmentCenter, ProjectType projectType, string projectName, int projectBudget, int developCost, UnitTemplate originalTemplate, UnitProperty unitProperty, TagController.UnitFunction runtimeFunction)
    {
        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                iconBackground.color = developmentColor;
                icon.sprite = developmentIcon;
                break;

            case ProjectType.UnitModify:
                iconBackground.color = modifyColor;
                icon.sprite = modifyIcon;
                break;

            case ProjectType.UnitUpgrade:
                iconBackground.color = upgradeColor;
                icon.sprite = upgradeIcon;
                break;
        }

        this.player = player;
        AddDevelopmentCenter(developmentCenter);
        this.projectType = projectType;

        this.projectName = projectName;
        this.projectBudget = projectBudget;
        budgetText.text = "- " + projectBudget.ToString();

        this.remainDevelopCost = developCost;
        completeDPText.text = "0";
        maxDPText.text = developCost.ToString();

        this.originalTemplate = originalTemplate;

        nameText.text = projectName;
        turnText.text = CalculateRemainTurn().ToString();
        progressBar.minValue = 0;
        progressBar.maxValue = developCost;
        progressBar.value = 0;

        this.unitProperty = unitProperty;
        this.runtimeFunction = runtimeFunction;

        WorldController.currentPlayer.playerStartFunction += this.ProjectCompleteCheck;
    }

    int CalculateRemainTurn()
    {
        int remainTurn = remainDevelopCost / developmentPoint;
        if (remainDevelopCost % developmentPoint != 0)
            remainTurn++;
        return remainTurn;
    }

    public void AddDevelopmentCenter(Building building)
    {
        developmentCenters.Add(building);
        Debug.Log("Builing Added");
        developmentPoint += 100;
        //developmentPoint += building.developmentPoint;
    }

    public void RemoveDevelopmentCenter(Building building)
    {
        developmentCenters.Remove(building);
        //developmentPoint -= building.developmentPoint;
    }

    public void ProjectCompleteCheck()
    {
        remainDevelopCost -= developmentPoint;
        progressBar.value += developmentPoint;
        completeDPText.text = progressBar.value.ToString();
        turnText.text = CalculateRemainTurn().ToString();


        if (remainDevelopCost <= 0)
        {
            EndProject();
            Destroy(this.gameObject);
        }
    }

    public void EndProject()
    {
        player.projectList.Remove(this.gameObject);
        UI_Controller.buildingUIController.UpdateUnitTemplateList();
        WorldController.currentPlayer.playerStartFunction -= ProjectCompleteCheck;
        WorldController.playerStartFunction -= ProjectCompleteCheck;

        switch (projectType)
        {
            case ProjectType.UnitDevelopment: //Same Function

            case ProjectType.UnitModify:
                GameObject tempTemplate = Instantiate(unitTemplatePrefab, UI_Controller.unitTemplateListUI.unitTemplateListObj.transform);
                UnitTemplate unitTemplate = tempTemplate.GetComponent<UnitTemplate>();
                unitTemplate.CreateTemplate(unitProperty, runtimeFunction);

                player.unitTemplateList.Add(unitTemplate);
                break;

            case ProjectType.UnitUpgrade:
                originalTemplate.property = unitProperty;
                originalTemplate.runTimeFunction = runtimeFunction;
                originalTemplate.UpdateTemplateInfo();
                break;
        }
    }

    public void ShowDetail()
    {
        UI_Controller.progressUI.ShowProjectDetail(projectName, unitProperty, developmentCenters);
        UI_Controller.selectedUnitTemplateProperty = unitProperty;
    }
}
