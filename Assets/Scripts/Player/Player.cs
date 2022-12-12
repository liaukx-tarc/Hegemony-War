using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Data")]
    public List<UnitTemplate> unitTemplateList = new List<UnitTemplate>();

    //Unit List
    public List<Unit> unitList = new List<Unit>();
    public List<GameObject> projectList = new List<GameObject>();
    public int projectCount;//Not include upgrade Progress

    //Building List
    public List<City> cityList = new List<City>();

    //Turn Function
    public WorldController.PlayerStartFunction playerStartFunction;
    public WorldController.PlayerEndFunction playerEndFunction;

    //Resource
    public int money;
    public int moneyIncome;
    public int sciencePoint;

    //Testing
    public int buildNum = 0;

    [Header("Prefabs")]
    public GameObject unitListObj;
    public GameObject buildingListObj;

    // Start is called before the first frame update
    protected void Start()
    {

    }

    private void Update()
    {

    }

    public void UpdateResource()
    {
        moneyIncome = 0;
        sciencePoint = 0;

        foreach (City city in cityList)
        {
            moneyIncome += city.moneyIncome;
            sciencePoint += city.sciencePointIncome;
        }
    }
}
