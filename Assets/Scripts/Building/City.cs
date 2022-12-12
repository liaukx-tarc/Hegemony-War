using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
    public Player player;
    public List<Area> areaList;
    public List<MapCell> controlledCellList;

    [Header("Function Status")]
    public bool isProduceGround;
    public bool isProduceNaval;
    public bool isProduceAirForce;
    public bool isDevelopmentGround;
    public bool isDevelopmentNaval;
    public bool isDevelopmentAirForce;

    [Header("Resource")]
    public int moneyIncome;
    public int foodIncome;
    public int productivityIncome;
    public int sciencePointIncome;
    public int developmentPointIncome;

    override public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell)
    {
        base.CreateBuilding(rendererCpn, colliderCpn, buildingProperty, belongCell);
        areaList = new List<Area>();

        moneyIncome = 0;
        foodIncome = 0;
        productivityIncome = 0;
        sciencePointIncome = 0;
        developmentPointIncome = 0;

        isProduceGround = false;
        isProduceNaval = false;
        isProduceAirForce = false;
        isDevelopmentGround = false;
        isDevelopmentNaval = false;
        isDevelopmentAirForce = false;

        WorldController.currentPlayer.playerStartFunction += CityTurn;
    }

    public void CityTurn()
    {
        
    }
}
