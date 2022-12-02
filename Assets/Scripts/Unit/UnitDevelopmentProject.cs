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
    Player player;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI turnText;
    public Slider progressBar;
    public ProjectType projectType;

    //Cost
    public TextMeshProUGUI completeDPText;
    public TextMeshProUGUI maxDPText;
    public TextMeshProUGUI budgetText;

    public string projectName;
    public int projectBudget;
    public int remainDevelopCost;

    public UnitTemplate originalTemplate;
    public UnitTemplate newTemplate;

    public int developmentPoint = 0;
    public List<Building> developmentCenters = new List<Building>();

    public void ProjectCreate(Player player, Building developmentCenter, ProjectType projectType, string projectName, int projectBudget, int developCost, UnitTemplate originalTemplate, UnitTemplate newTemplate)
    {
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
        this.newTemplate = newTemplate;

        nameText.text = projectName;
        turnText.text = CalculateRemainTurn().ToString();
        progressBar.minValue = 0;
        progressBar.maxValue = developCost;
        progressBar.value = 0;

        WorldController.startTurnFunction += ProjectCompleteCheck;
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


        if (remainDevelopCost < 0)
        {
            EndProject();
        }
    }

    public void EndProject()
    {
        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                player.unitTemplateList.Add(newTemplate);
                break;

            case ProjectType.UnitModify: //Same function

            case ProjectType.UnitUpgrade:
                originalTemplate = newTemplate;
                break;
        }

        WorldController.startTurnFunction -= ProjectCompleteCheck;
        GameObject.Destroy(this.gameObject);
    }
}
