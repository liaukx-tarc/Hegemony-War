using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Data")]
    public Color playerColor;

    //Resource
    [Header("Resource")]
    public int money;
    public int moneyIncome;
    public int sciencePoint; 

    //Unit List
    [Header("Unit")]
    public List<Unit> unitList = new List<Unit>();
    public List<UnitTemplate> unitTemplateList = new List<UnitTemplate>();
    public List<GameObject> projectList = new List<GameObject>();
    public int projectCount;//Not include upgrade Progress

    //City List
    [Header("City")]
    public int cityNum = 0;
    public List<City> cityList = new List<City>();

    //Turn Function
    [Header("Function")]
    public WorldController.PlayerStartFunction playerStartFunction;
    public WorldController.PlayerEndFunction playerEndFunction;

    [Header("Prefabs")]
    public GameObject unitListObj;
    public GameObject buildingListObj;

    public void UpdateResource()
    {
        moneyIncome = 0;
        sciencePoint = 0;

        foreach (GameObject project in projectList)
        {
            moneyIncome -= project.GetComponent<UnitDevelopmentProject>().projectBudget;
        }

        foreach (City city in cityList)
        {
            moneyIncome += city.moneyIncome;
            sciencePoint += city.sciencePointIncome;
        }

        foreach (Unit unit in unitList)
        {
            moneyIncome -= unit.property.maintanceCost;
        }
    }
}
