using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
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
    //public int sciencePointIncome;
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
        slider.value = slider.maxValue = maxHP;

        WorldController.instance.currentPlayer.playerStartFunction += CityTurn;
        WorldController.instance.playerStartFunction += CityTurn;
    }

    public void CityTurn()
    {
        foreach(Area area in areaList)
        {
            area.AreaTurn();
        }

        if (!isDestroy)
        {
            if(currentHp < maxHP)
            {
                currentHp += WorldController.instance.buildingController.restoreHP;
                currentHp = Mathf.Min(currentHp, maxHP);
                slider.value = currentHp;

                if (currentHp == maxHP)
                {
                    slider.gameObject.SetActive(false);
                }
            }

            CheckProduce();
        }     
    }

    //City Produce
    [Header("Produce")]
    public int productivityNeed;
    public int productivityCompleted;

    public MapCell producingCell;
    public GameObject areaModel;
    public BuildingProperty producingArea;
    public UnitTemplate producingUnit;

    //Produce Area
    public void Produce(GameObject areaModel, BuildingProperty building, MapCell producingCell)
    {
        producingUnit = null;
        producingArea = building;
        this.producingCell = producingCell;
        this.areaModel = areaModel;

        productivityNeed = building.produceCost;
        productivityCompleted = 0;

        areaModel.SetActive(false);
        WorldController.instance.buildingController.DisableBlueCells();
        WorldController.instance.uiController.buildingUIController.UpdateProduceProgress();
    }

    //Produce Unit
    public void Produce(UnitTemplate unit, MapCell producingCell)
    {          
        producingUnit = unit;
        producingArea = null;
        this.producingCell = producingCell;

        productivityNeed = unit.property.produceCost;
        productivityCompleted = 0;

        WorldController.instance.uiController.buildingUIController.UpdateProduceProgress();
    }

    public void CheckProduce()
    {
        if (producingArea == null && producingUnit == null)
            return;

        productivityCompleted += productivityIncome;

        if (productivityCompleted < productivityNeed)
            return;

        if (producingUnit != null)
        {
            WorldController.instance.unitController.GenerateUnit(player, producingUnit.property, producingCell);
        }

        else if(producingArea != null)
        {
            WorldController.instance.buildingController.BuildArea(producingArea, areaModel, this, producingCell);
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
        if(isDestroy)
        {
            moneyIncome = 0;
            foodIncome = 0;
            productivityIncome = 0;
            //sciencePointIncome = 0;
            developmentPointIncome = 0;
        }

        else
        {
            moneyIncome = buildingProperty.money;
            foodIncome = buildingProperty.food;
            productivityIncome = buildingProperty.productivity;
            //sciencePointIncome = buildingProperty.sciencePoint;
            developmentPointIncome = buildingProperty.developmentPoint;            
        }

        maxHP = buildingProperty.healthPoint;
        defense = buildingProperty.defense;
        damage = buildingProperty.damage;

        //City Maintence Cost
        moneyIncome -= buildingProperty.maintenanceCost;

        foreach (MapCell cell in controlledCellList)
        {
            if (cell.mapType == (int)MapTypeName.Marsh)
            {
                productivityIncome += 5;
            }

            else if (cell.mapType == (int)MapTypeName.Plain || cell.mapType == (int)MapTypeName.Coast)
            {
                moneyIncome += 5;
            }

            else if (cell.mapType == (int)MapTypeName.Snow)
            {
                developmentPointIncome += 5;
            }

            else if (cell.mapType == (int)MapTypeName.Desert)
            {
                moneyIncome += 7;
            }

            else if (cell.mapType == (int)MapTypeName.Forest)
            {
                productivityIncome += 10;
            }
        }

        List<MapCell> cellsInRange;

        foreach (Area area in areaList)
        {
            if (!area.isDestroy)
            {
                switch (area.buildingProperty.buildingType)
                {
                    case BuildingType.MilitaryBase:
                        maxHP += area.buildingProperty.healthPoint;
                        defense += area.buildingProperty.defense;
                        damage += area.buildingProperty.damage;
                        break;

                    case BuildingType.NavalBase:
                        maxHP += area.buildingProperty.healthPoint;
                        defense += area.buildingProperty.defense;
                        damage += area.buildingProperty.damage;
                        break;

                    case BuildingType.AirForceBase:
                        maxHP += area.buildingProperty.healthPoint;
                        defense += area.buildingProperty.defense;
                        damage += area.buildingProperty.damage;
                        break;
                }

                moneyIncome += area.buildingProperty.money;
                //foodIncome += area.buildingProperty.food;
                productivityIncome += area.buildingProperty.productivity;
                //sciencePointIncome += area.buildingProperty.sciencePoint;
                developmentPointIncome += area.buildingProperty.developmentPoint;

                //Area Maintence Cost
                moneyIncome -= area.buildingProperty.maintenanceCost;

                cellsInRange = area.belongCell.CheckCellInRange(area.buildingProperty.buildingRange);

                for (int i = 0; i < cellsInRange.Count; i++)
                {
                    if (cellsInRange[i].belongCity == null ||
                        (cellsInRange[i].belongCity != null && cellsInRange[i].belongCity.player == player))
                    {
                        switch (area.buildingProperty.buildingType)
                        {
                            //case BuildingType.AgriculturalArea:
                            //    if (cellsInRange[i].building != null &&
                            //        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AgriculturalArea)
                            //    {
                            //        foodIncome += 5;
                            //    }

                            //    if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                            //    {
                            //        foodIncome += 3;
                            //    }

                            //    else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
                            //    {
                            //        foodIncome += 2;
                            //    }

                            //    else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
                            //    {
                            //        foodIncome += 1;
                            //    }
                            //    break;

                            //case BuildingType.ResearchCenter:
                            //    if (cellsInRange[i].building != null &&
                            //        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.ResearchCenter)
                            //    {
                            //        sciencePointIncome += 5;
                            //    }
                            //    break;

                            case BuildingType.IndustrialArea:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.IndustrialArea)
                                {
                                    productivityIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Forest || cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                                {
                                    productivityIncome += 3;
                                }
                                break;

                            case BuildingType.CommercialCenter:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                                {
                                    moneyIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Desert || cellsInRange[i].mapType == (int)MapTypeName.Plain)
                                {
                                    moneyIncome += 3;
                                }
                                break;

                            case BuildingType.Harbor:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                                {
                                    moneyIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                                {
                                    moneyIncome += 3;
                                }
                                break;

                            case BuildingType.Airport:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                                {
                                    moneyIncome += 20;
                                }
                                break;

                            case BuildingType.MilitaryBase:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                                {
                                    productivityIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                                {
                                    productivityIncome += 3;
                                }
                                break;

                            case BuildingType.NavalBase:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                                {
                                    productivityIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                                {
                                    productivityIncome += 3;
                                }
                                break;

                            case BuildingType.AirForceBase:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                                {
                                    productivityIncome += 20;
                                }
                                break;

                            case BuildingType.MilitaryFactory:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                                {
                                    developmentPointIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Desert || cellsInRange[i].mapType == (int)MapTypeName.Snow)
                                {
                                    developmentPointIncome += 3;
                                }
                                break;

                            case BuildingType.NavalShipyard:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                                {
                                    developmentPointIncome += 10;
                                }

                                if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                                {
                                    developmentPointIncome += 3;
                                }
                                break;

                            case BuildingType.FlightTestCenter:
                                if (cellsInRange[i].building != null && cellsInRange[i].building.player == player &&
                                (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                                cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                                {
                                    developmentPointIncome += 20;
                                }
                                break;
                        }
                    }
                }
            }
        }

        slider.maxValue = maxHP;
    }

    public void CancelProject()
    {
        if(developmentProject != null)
        {
            developmentProject.RemoveDevelopmentCenter(this);
            developmentProject = null;
        }
    }

    public void CaptureCity(Player newPlayer)
    {
        
        {
            //Old player
            player.cityList.Remove(this);
            player.playerStartFunction -= CityTurn;

            //New player
            player = newPlayer;
            newPlayer.cityList.Add(this);
            iconBackground.color = player.playerColor;
            CancelProduce();
            CancelProject();

            foreach (Area area in areaList)
            {
                area.player = newPlayer;
                area.iconBackground.color = newPlayer.playerColor;

                if (area.belongCell.unit != null && area.belongCell.unit.player != player)
                {
                    area.currentHp = 0;
                    area.slider.value = 0;
                    area.isDestroy = true;
                    area.slider.gameObject.SetActive(true);
                }
            }

            WorldController.instance.CheckVictory();
        }

        CalculateIncome();
        currentHp += Mathf.Abs(maxHP / 2);
        slider.value = currentHp;
    }
}
