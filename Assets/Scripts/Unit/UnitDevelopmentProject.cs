using System.Collections.Generic;
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
    public List<City> developmentCenters = new List<City>();

    const string unlimitedString = "X";

    public void ProjectCreate(Player player, City developmentCenter, ProjectType projectType, string projectName, int projectBudget, int developCost, UnitTemplate originalTemplate, UnitProperty unitProperty, TagController.UnitFunction runtimeFunction)
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

        remainDevelopCost = developCost;
        completeDPText.text = "0";
        maxDPText.text = developCost.ToString();

        this.originalTemplate = originalTemplate;

        nameText.text = projectName;

        int remainTurn = CalculateRemainTurn();
        
        if(remainTurn > 0)
        {
            turnText.text = remainTurn.ToString();
        }
        else
        {
            turnText.text = unlimitedString;
        }
        
        progressBar.minValue = 0;
        progressBar.maxValue = developCost;
        progressBar.value = 0;

        this.unitProperty = unitProperty;
        this.runtimeFunction = runtimeFunction;

        WorldController.instance.currentPlayer.playerStartFunction += ProjectCompleteCheck;
        WorldController.instance.playerStartFunction += ProjectCompleteCheck;
    }

    public int CalculateRemainTurn()
    {
        if(developmentPoint!=0)
        {
            int remainTurn = remainDevelopCost / developmentPoint;
            if (remainDevelopCost % developmentPoint != 0)
                remainTurn++;
            return remainTurn;
        }
        
        return 0;
    }

    public void AddDevelopmentCenter(City city)
    {
        developmentCenters.Add(city);
        city.developmentProject = this;
        UpdateProjectData();
    }

    public void RemoveDevelopmentCenter(City city)
    {
        developmentCenters.Remove(city);
        city.developmentProject = null;
        UpdateProjectData();
    }

    public void ProjectCompleteCheck()
    {
        UpdateProjectData();
        remainDevelopCost -= developmentPoint;
        progressBar.value += developmentPoint;
        completeDPText.text = progressBar.value.ToString();

        int remainTurn = CalculateRemainTurn();
        if(remainTurn > 0)
        {
            turnText.text = remainTurn.ToString();
        }
        else
        {
            turnText.text = unlimitedString;
        }

        if (remainDevelopCost <= 0)
        {
            EndProject();
            Destroy(this.gameObject);
        }
    }

    public void EndProject()
    {
        player.projectList.Remove(this.gameObject);
        WorldController.instance.currentPlayer.playerStartFunction -= ProjectCompleteCheck;
        WorldController.instance.playerStartFunction -= ProjectCompleteCheck;

        do
        {
            RemoveDevelopmentCenter(developmentCenters[0]);

        } while (developmentCenters.Count > 0);

        switch (projectType)
        {
            case ProjectType.UnitDevelopment: //Same Function

            case ProjectType.UnitModify:
                GameObject tempTemplate = Instantiate(unitTemplatePrefab, WorldController.instance.uiController.unitTemplateListUI.unitTemplateListObj.transform);
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

    public void UpdateProjectData()
    {
        developmentPoint = 0;

        foreach (City city in developmentCenters)
        {
            developmentPoint += city.developmentPointIncome;
        }

        int remainTurn = CalculateRemainTurn();
        if (remainTurn > 0)
        {
            turnText.text = remainTurn.ToString();
        }
        else
        {
            turnText.text = unlimitedString;
        }
    }

    public void ShowDetail()
    {
        WorldController.instance.uiController.ClickSound();
        WorldController.instance.uiController.progressUI.ShowProjectDetail(this);
    }
}
