using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public GameObject cityPrefab;
    public GameObject areaPrefab;
    int inRangeCellNum;

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

    public GameObject modelObject;
    public BuildingProperty selectingBuilding;
    public BuildingProperty cityProperty;

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

    public void BuildCity(Player player, GameObject cityModel, MapCell belongCell)
    {
        GameObject tempObj = Instantiate(cityPrefab, belongCell.transform.position, Quaternion.identity, belongCell.transform);
        cityModel.transform.parent = tempObj.transform;
        cityModel.transform.localPosition = new Vector3(0, cityModel.transform.localPosition.y, 0);
        cityModel.SetActive(true);

        City city = tempObj.GetComponent<City>();
        belongCell.belongCity = city;
        belongCell.building = city;
        belongCell.mapObjectList.Add(city);

        city.player = player;

        cellsInRange = belongCell.CheckCellInRange(cityProperty.buildingRange);

        foreach (MapCell cell in cellsInRange)
        {
            cell.belongCity = city;
            city.controlledCellList.Add(cell);
        }

        WorldController.currentPlayer.cityList.Add(city);
        city.CreateBuilding(cityModel.GetComponent<Renderer>(), cityModel.GetComponent<Collider>(), cityProperty, belongCell);
    }

    public void BuildArea(BuildingProperty areaProperty, GameObject areaModel, City belongCity, MapCell belongCell)
    {
        switch (areaProperty.buildingType)
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
        areaModel.transform.parent = tempObj.transform;
        areaModel.transform.localPosition = new Vector3(0, areaModel.transform.localPosition.y, 0);
        areaModel.SetActive(true);

        Area area = tempObj.GetComponent<Area>();
        belongCell.building = area;
        belongCell.mapObjectList.Add(area);
        belongCell.building.CreateBuilding(areaModel.GetComponent<Renderer>(), areaModel.GetComponent<Collider>(), areaProperty, belongCell, belongCity);

        belongCity.areaList.Add(area);
    }

    public List<MapCell> cellsInRange;
    int cellResourceAmount;
    Sprite resourceIcon;
    Color resourceColor;

    public bool CityBuildCheck(MapCell belongCell)
    {
        cellsInRange = belongCell.CheckCellInRange(selectingBuilding.buildingRange);

        inRangeCellNum = cellsInRange.Count;

        foreach (MapCell cell in cellsInRange)
        {
            if (cell.belongCity != null)
                return false;
        }

        for (int i = 0; i < cellsInRange.Count; i++)
        {
            cellResourceAmount = 0;

            if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
            {
                cellResourceAmount += 3;
                resourceIcon = foodIcon;
                resourceColor = foodColor;
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
            {
                cellResourceAmount += 2;
                resourceIcon = foodIcon;
                resourceColor = foodColor;
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
            {
                cellResourceAmount += 1;
                resourceIcon = foodIcon;
                resourceColor = foodColor;
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
            {
                cellResourceAmount += 2;
                resourceIcon = moneyIcon;
                resourceColor = moneyColor;
            }

            else if (cellsInRange[i].mapType == (int)MapTypeName.Forest)
            { 
                cellResourceAmount += 2;
                resourceIcon = productivityIcon;
                resourceColor = productivityColor;
            }


            if(cellResourceAmount > 0)
            {
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], cellResourceAmount.ToString(), resourceIcon, resourceColor);
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

        inRangeCellNum = cellsInRange.Count;

        if (belongCell.belongCity != belongCity)
            return false;

        //Map Cell Checking
        switch (selectingBuilding.buildingType)
        {
            case BuildingType.Harbor:

            case BuildingType.NavalBase:

            case BuildingType.NavalShipyard:
                if (belongCell.mapType != (int)MapTypeName.Ocean && belongCell.mapType != (int)MapTypeName.Coast)
                    return false;
                break;

            default:
                if (belongCell.mapType == (int)MapTypeName.Ocean || belongCell.mapType == (int)MapTypeName.Coast)
                    return false;
                break;
        }

        for (int i = 0; i < cellsInRange.Count; i++)
        {
            cellResourceAmount = 0;

            switch (selectingBuilding.buildingType)
            {
                case BuildingType.AgriculturalArea:
                    if (cellsInRange[i].building != null && 
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AgriculturalArea)
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Marsh)
                    {
                        cellResourceAmount += 3;
                    }

                    else if (cellsInRange[i].mapType == (int)MapTypeName.Plain || cellsInRange[i].mapType == (int)MapTypeName.Coast)
                    {
                        cellResourceAmount += 2;
                    }

                    else if (cellsInRange[i].mapType == (int)MapTypeName.Snow)
                    {
                        cellResourceAmount += 1;
                    }

                    resourceIcon = foodIcon;
                    resourceColor = foodColor;
                    break;

                case BuildingType.ResearchCenter:
                    if (cellsInRange[i].building != null && 
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.ResearchCenter)
                    { 
                        cellResourceAmount += 5;
                    }

                    resourceIcon = sciencePointIcon;
                    resourceColor = sciencePointColor;
                    break;

                case BuildingType.IndustrialArea:
                    if (cellsInRange[i].building != null && 
                        cellsInRange[i].building.buildingProperty.buildingType == BuildingType.IndustrialArea)
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Forest)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = productivityIcon;
                    resourceColor = productivityColor;
                    break;

                case BuildingType.CommercialCenter:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = moneyIcon;
                    resourceColor = moneyColor;
                    break;

                case BuildingType.Harbor:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = moneyIcon;
                    resourceColor = moneyColor;
                    break;

                case BuildingType.Airport:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.CommercialCenter ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Harbor ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.Airport))
                    {
                        cellResourceAmount += 10;
                    }

                    resourceIcon = moneyIcon;
                    resourceColor = moneyColor;
                    break;

                case BuildingType.MilitaryBase:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Plain)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = productivityIcon;
                    resourceColor = productivityColor;
                    break;

                case BuildingType.NavalBase:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                    {
                        cellResourceAmount += 2;
                    }


                    resourceIcon = productivityIcon;
                    resourceColor = productivityColor;
                    break;

                case BuildingType.AirForceBase:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalBase ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.AirForceBase))
                    {
                        cellResourceAmount += 10;
                    }

                    resourceIcon = productivityIcon;
                    resourceColor = productivityColor;
                    break;

                case BuildingType.MilitaryFactory:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Desert)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = developmentPointIcon;
                    resourceColor = developmentPointColor;
                    break;

                case BuildingType.NavalShipyard:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        cellResourceAmount += 5;
                    }

                    if (cellsInRange[i].mapType == (int)MapTypeName.Coast || cellsInRange[i].mapType == (int)MapTypeName.Ocean)
                    {
                        cellResourceAmount += 2;
                    }

                    resourceIcon = developmentPointIcon;
                    resourceColor = developmentPointColor;
                    break;

                case BuildingType.FlightTestCenter:
                    if (cellsInRange[i].building != null &&
                    (cellsInRange[i].building.buildingProperty.buildingType == BuildingType.MilitaryFactory ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.NavalShipyard ||
                    cellsInRange[i].building.buildingProperty.buildingType == BuildingType.FlightTestCenter))
                    {
                        cellResourceAmount += 10;
                    }

                    resourceIcon = developmentPointIcon;
                    resourceColor = developmentPointColor;
                    break;
            }

            if (cellResourceAmount > 0)
                ActiveResourceText(WorldController.playerController.blueSelectCellList[i], cellResourceAmount.ToString(), resourceIcon, resourceColor);

            else
                WorldController.playerController.blueSelectCellList[i].resourceText.transform.parent.gameObject.SetActive(false);

            WorldController.playerController.ActiveSelectCell(WorldController.playerController.blueSelectCellList[i].gameObject, cellsInRange[i].transform);
        }

        //Move Model
        modelObject.SetActive(true);
        modelObject.transform.position = belongCell.transform.position + new Vector3(0, 0.75f, 0);
        return true;
    }

    void ActiveResourceText(SelectionCell selectionCell, string resourceAmount, Sprite resourceIcon, Color color)
    {
        selectionCell.turnUI.SetActive(false);

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
