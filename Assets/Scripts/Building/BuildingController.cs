using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public GameObject cityPrefab;
    public GameObject areaPrefab;
    int inRangeCellNum;

    const string PlusOne = "+1";
    const string PlusTwo = "+2";
    const string PlusThree = "+3";

    [Header("Icon")]
    public Sprite moneyIcon;
    public Sprite foodIcon;
    public Sprite productivityIcon;
    public Sprite sciencePointIcon;
    public Sprite developmentPointIcon;

    [Header("Color")]
    public Color moneyColor;
    public Color foodColor;
    public Color productivityColor;
    public Color sciencePointColor;
    public Color developmentPointColor;

    [Header("Resource")]
    int moneyIncome;
    int foodIncome;
    int productivityIncome;
    int sciencePointIncome;
    int developmentPointIncome;
    int maintenanceCost;

    GameObject modelObject;
    BuildingProperty selectingBuilding;

    public void StartBuilding(BuildingProperty building)
    {
        modelObject = Instantiate(building.model);
        selectingBuilding = building;
    }

    public void CancelBuilding()
    {
        if(modelObject!= null)
        {
            Destroy(modelObject);
            modelObject = null;
        }

        DisableBlueCells();
    }

    public void BuildCity(MapCell belongCell)
    {
        GameObject tempObj = Instantiate(cityPrefab, belongCell.transform.position, Quaternion.identity, belongCell.transform);
        modelObject.transform.parent = tempObj.transform;

        City city = tempObj.GetComponent<City>();
        belongCell.belongCity = city;
        belongCell.building = city;
        belongCell.mapObjectList.Add(city);
        city.CreateBuilding(modelObject.GetComponent<Renderer>(), modelObject.GetComponent<Collider>(), selectingBuilding, belongCell);

        city.player = WorldController.currentPlayer;
        foreach(MapCell cell in cellsInRange)
        {
            cell.belongCity = city;
            city.controlledCellList.Add(cell);
        }

        city.moneyIncome += moneyIncome;
        city.foodIncome += foodIncome;
        city.productivityIncome += productivityIncome;
        city.sciencePointIncome += sciencePointIncome;
        city.developmentPointIncome += developmentPointIncome;

        WorldController.currentPlayer.cityList.Add(city);
        WorldController.currentPlayer.UpdateResource();

        DisableBlueCells();
        modelObject = null;
    }

    public void BuildArea(City belongCity, MapCell belongCell)
    {
        belongCity.moneyIncome += moneyIncome -= maintenanceCost;
        belongCity.foodIncome += foodIncome;
        belongCity.productivityIncome += productivityIncome;
        belongCity.sciencePointIncome += sciencePointIncome;
        belongCity.developmentPointIncome += developmentPointIncome;

        switch (selectingBuilding.buildingType)
        {
            case BuildingType.MilitaryBase:
                belongCity.isProduceGround = true;
                break;

            case BuildingType.NavalBase:
                belongCity.isProduceNaval = true;
                break;

            case BuildingType.AirForceBase:
                belongCity.isProduceAirForce = true;
                break;

            case BuildingType.MilitaryFactory:
                belongCity.isDevelopmentGround = true;
                break;

            case BuildingType.NavalShipyard:
                belongCity.isDevelopmentNaval = true;
                break;

            case BuildingType.FlightTestCenter:
                belongCity.isDevelopmentAirForce = true;
                break;
        }

        GameObject tempObj = Instantiate(areaPrefab, belongCell.transform.position, Quaternion.identity, belongCell.transform);
        modelObject.transform.parent = tempObj.transform;
        
        Area area = tempObj.GetComponent<Area>();
        belongCell.building = area;
        belongCell.mapObjectList.Add(area);
        belongCell.building.CreateBuilding(modelObject.GetComponent<Renderer>(), modelObject.GetComponent<Collider>(), selectingBuilding, belongCell, belongCity);

        belongCity.areaList.Add(area);
        WorldController.currentPlayer.UpdateResource();
        UI_Controller.buildingUIController.UpdateBuildingList();

        DisableBlueCells();
        modelObject = null;
    }

    public List<MapCell> cellsInRange;

    public bool CityBuildCheck(MapCell belongCell)
    {
        cellsInRange = belongCell.CheckCellInRange(selectingBuilding.buildingRange);

        moneyIncome = 0;
        foodIncome = 0;
        productivityIncome = 0;
        sciencePointIncome = 0;
        developmentPointIncome = 0;
        maintenanceCost = selectingBuilding.maintenanceCost;

        inRangeCellNum = cellsInRange.Count;

        foreach (MapCell cell in cellsInRange)
        {
            if (cell.belongCity != null)
                return false;
        }

        for (int i = 0; i < cellsInRange.Count; i++)
        {
            if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
            {
                foodIncome += 3;
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusThree, foodIcon, foodColor);
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
            {
                foodIncome += 2;
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, foodIcon, foodColor);
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
            {
                foodIncome += 1;
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusOne, foodIcon, foodColor);
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
            {
                moneyIncome += 2;
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, moneyIcon, moneyColor);
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Forest)
            {
                productivityIncome += 2;
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, productivityIcon, productivityColor);
            }

            else
            {
                WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
            }

            WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
        }

        //Move Model
        modelObject.SetActive(true);
        modelObject.transform.position = belongCell.transform.position + new Vector3(0, 0.75f, 0);
        return true;
    }

    public bool AreaBuildCheck(City belongCity, MapCell belongCell)
    {
        cellsInRange = belongCell.CheckCellInRange(selectingBuilding.buildingRange);

        moneyIncome = 0;
        foodIncome = 0;
        productivityIncome = 0;
        sciencePointIncome = 0;
        developmentPointIncome = 0;
        maintenanceCost = selectingBuilding.maintenanceCost;

        inRangeCellNum = cellsInRange.Count;

        if (belongCell.belongCity != belongCity)
            return false;

        for (int i = 0; i < cellsInRange.Count; i++)
        {
            switch (selectingBuilding.buildingType)
            {
                case BuildingType.AgriculturalArea:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    foodIncome += selectingBuilding.food;

                    if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                    {
                        foodIncome += 3;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusThree, foodIcon, foodColor);
                    }

                    else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
                    {
                        foodIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, foodIcon, foodColor);
                    }

                    else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
                    {
                        foodIncome += 1;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusOne, foodIcon, foodColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.ResearchCenter:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    sciencePointIncome += selectingBuilding.sciencePoint;

                    if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
                    {
                        sciencePointIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, sciencePointIcon, sciencePointColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.IndustrialArea:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    productivityIncome += selectingBuilding.productivity;

                    if (cellsInRange[i].mapType == (int)MapTypeName.Forest)
                    {
                        productivityIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, productivityIcon, productivityColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.CommercialCenter:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    moneyIncome += selectingBuilding.money;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        moneyIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, moneyIcon, moneyColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.Harbor:
                    //Map Cell Checking
                    if (belongCell.mapType != (int)MapTypeName.Ocean && belongCell.mapType != (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    moneyIncome += selectingBuilding.money;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        moneyIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, moneyIcon, moneyColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.Airport:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    moneyIncome += selectingBuilding.money;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        moneyIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, moneyIcon, moneyColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.MilitaryBase:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    productivityIncome += selectingBuilding.productivity;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        productivityIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, productivityIcon, productivityColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.NavalBase:
                    //Map Cell Checking
                    if (belongCell.mapType != (int)MapTypeName.Ocean && belongCell.mapType != (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    productivityIncome += selectingBuilding.productivity;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        productivityIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, productivityIcon, productivityColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.AirForceBase:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    productivityIncome += selectingBuilding.productivity;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        productivityIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, productivityIcon, productivityColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.MilitaryFactory:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    developmentPointIncome += selectingBuilding.developmentPoint;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        developmentPointIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, developmentPointIcon, developmentPointColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.NavalShipyard:
                    //Map Cell Checking
                    if (belongCell.mapType != (int)MapTypeName.Ocean && belongCell.mapType != (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    developmentPointIncome += selectingBuilding.developmentPoint;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        developmentPointIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, developmentPointIcon, developmentPointColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;

                case BuildingType.FlightTestCenter:
                    //Map Cell Checking
                    if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                        return false;

                    //Calculate Resource Income
                    developmentPointIncome += selectingBuilding.developmentPoint;

                    if (cellsInRange[i].building != null &&
                        (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        developmentPointIncome += 2;
                        ActiveResourceText(WorldController.playerController.blueSelectCellList[i], PlusTwo, developmentPointIcon, developmentPointColor);
                    }

                    else
                    {
                        WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
                    }

                    WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
                    break;
            }
        }

        //Move Model
        modelObject.SetActive(true);
        modelObject.transform.position = belongCell.transform.position + new Vector3(0, 0.75f, 0);
        return true;
    }

    void ActiveResourceText(SelectionCell selectionCell, string resourceAmount, Sprite resourceIcon, Color color)
    {
        selectionCell.resourceText.text = resourceAmount;
        selectionCell.resourceIcon.sprite = resourceIcon;
        selectionCell.resourceIcon.color = color;
        selectionCell.resourceText.transform.parent.gameObject.SetActive(true);
    }

    public void DisableBlueCells()
    {
        for (int i = 0; i < inRangeCellNum; i++)
        {
            WorldController.playerController.blueSelectCellList[i].gameObject.SetActive(false);
            WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void DisableModel()
    {
        modelObject.SetActive(false);
    }
}
