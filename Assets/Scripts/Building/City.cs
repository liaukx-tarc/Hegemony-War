using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
    public Player player;
    public string cityName;
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

    //Development Project
    [Header("Project")]
    public UnitDevelopmentProject developmentProject;

    override public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell)
    {
        player.cityNum++;
        cityName = "City " + player.cityNum;

        base.CreateBuilding(rendererCpn, colliderCpn, buildingProperty, belongCell);
        areaList = new List<Area>();

        isProduceGround = false;
        isProduceNaval = false;
        isProduceAirForce = false;
        isDevelopmentGround = false;
        isDevelopmentNaval = false;
        isDevelopmentAirForce = false;

        CalculateIncome();

        WorldController.currentPlayer.playerStartFunction += CityTurn;
        WorldController.playerStartFunction += CityTurn;
    }

    public void CityTurn()
    {
        CheckProduce();
    }

    //City Produce
    [Header("Produce")]
    public int productivityNeed;
    public int productivityCompleted;

    public MapCell producingCell;
    public GameObject areaModel;
    public BuildingProperty producingArea;
    public UnitTemplate producingUnit;

    public void Produce(GameObject areaModel, BuildingProperty building, MapCell producingCell)
    {
        producingUnit = null;
        producingArea = building;
        this.producingCell = producingCell;
        this.areaModel = areaModel;

        productivityNeed = building.produceCost;
        productivityCompleted = 0;

        areaModel.SetActive(false);
        WorldController.buildingController.DisableBlueCells();
        UI_Controller.buildingUIController.UpdateProduceProgress();
    }

    public void Produce(UnitTemplate unit, MapCell producingCell)
    {          
        producingUnit = unit;
        producingArea = null;
        this.producingCell = producingCell;

        productivityNeed = unit.property.produceCost;
        productivityCompleted = 0;

        UI_Controller.buildingUIController.UpdateProduceProgress();
    }

    public void CheckProduce()
    {
        Debug.Log(cityName + " Produce");
        if (producingArea == null && producingUnit == null)
            return;

        productivityCompleted += productivityIncome;

        if (productivityCompleted < productivityNeed)
            return;

        if (producingUnit != null)
        {
            WorldController.unitController.GenerateUnit(WorldController.currentPlayer, producingUnit.property, producingCell);
        }

        else if(producingArea != null)
        {
            WorldController.buildingController.BuildArea(producingArea, areaModel, this, producingCell);
            CalculateIncome();
        }

        producingArea = null;
        producingUnit = null;
        producingCell = null; 
        areaModel = null;
    }

    public void CancelProduce()
    {
        Destroy(areaModel);

        producingArea = null;
        producingUnit = null;
        producingCell = null;
        areaModel = null;
    }

    public void CalculateIncome()
    {
        moneyIncome = 0;
        foodIncome = 0;
        productivityIncome = 200;
        sciencePointIncome = 0;
        developmentPointIncome = 0;

        //City Maintence Cost
        moneyIncome -= buildingProperty.maintenanceCost;

        foreach (MapCell cell in controlledCellList)
        {
            if (cell.mapType == (int)MapTypeName.Marsh)
            {
                foodIncome += 3;
            }

            else if (cell.mapType == (int)MapTypeName.Plain || cell.mapType == (int)MapTypeName.Coast)
            {
                foodIncome += 2;
            }

            else if (cell.mapType == (int)MapTypeName.Snow)
            {
                foodIncome += 1;
            }

            else if (cell.mapType == (int)MapTypeName.Desert)
            {
                moneyIncome += 2;
            }

            else if (cell.mapType == (int)MapTypeName.Forest)
            {
                productivityIncome += 2;
            }
        }

        List<MapCell> cellsInRange;

        foreach (Area area in areaList)
        {
            //Area Maintence Cost
            moneyIncome += area.buildingProperty.money;
            foodIncome += area.buildingProperty.food;
            productivityIncome += area.buildingProperty.productivity;
            sciencePointIncome += area.buildingProperty.sciencePoint;
            developmentPointIncome += area.buildingProperty.developmentPoint;
            moneyIncome -= area.buildingProperty.maintenanceCost;

            cellsInRange = area.belongCell.CheckCellInRange(area.buildingProperty.buildingRange);

            for (int i = 0; i < cellsInRange.Count; i++)
            {
                switch (area.buildingProperty.buildingType)
                {
                    case BuildingType.AgriculturalArea:
                        if (cellsInRange[i].building != null &&
                            cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AgriculturalArea)
                        {
                            foodIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                        {
                            foodIncome += 3;
                        }

                        else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
                        {
                            foodIncome += 2;
                        }

                        else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
                        {
                            foodIncome += 1;
                        }
                        break;

                    case BuildingType.ResearchCenter:
                        if (cellsInRange[i].building != null &&
                            cellsInRange[i].building.buildingProperty.buildingType == BuildingType.ResearchCenter)
                        {
                            sciencePointIncome += 5;
                        }
                        break;

                    case BuildingType.IndustrialArea:
                        if (cellsInRange[i].building != null &&
                            cellsInRange[i].building.buildingProperty.buildingType == BuildingType.IndustrialArea)
                        {
                            productivityIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Forest)
                        {
                            productivityIncome += 2;
                        }
                        break;

                    case BuildingType.CommercialCenter:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                        {
                            moneyIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
                        {
                            moneyIncome += 2;
                        }
                        break;

                    case BuildingType.Harbor:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                        {
                            moneyIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                        {
                            moneyIncome += 2;
                        }
                        break;

                    case BuildingType.Airport:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                        {
                            moneyIncome += 10;
                        }
                        break;

                    case BuildingType.MilitaryBase:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                        {
                            productivityIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Plain)
                        {
                            productivityIncome += 2;
                        }
                        break;

                    case BuildingType.NavalBase:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                        {
                            productivityIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                        {
                            productivityIncome += 2;
                        }
                        break;

                    case BuildingType.AirForceBase:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                        {
                            productivityIncome += 10;
                        }
                        break;

                    case BuildingType.MilitaryFactory:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                        {
                            developmentPointIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
                        {
                            developmentPointIncome += 2;
                        }
                        break;

                    case BuildingType.NavalShipyard:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                        {
                            developmentPointIncome += 5;
                        }

                        if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                        {
                            developmentPointIncome += 2;
                        }
                        break;

                    case BuildingType.FlightTestCenter:
                        if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                        {
                            developmentPointIncome += 10;
                        }
                        break;
                }
            }
        }
    }

    public void CancelProject()
    {
        developmentProject.RemoveDevelopmentCenter(this);
        developmentProject = null;
    }
}
