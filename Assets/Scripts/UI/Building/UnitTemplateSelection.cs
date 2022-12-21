using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTemplateSelection : MonoBehaviour
{
    public Image unitIcon;
    public Image templateBackground;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI produceVariable;

    public UnitTemplate unitTemplate;

    public void onClick()
    {
        WorldController.instance.uiController.ClickSound();
        City selectingCity = (City)WorldController.instance.playerController.selectedBuilding;

        if (selectingCity.producingArea == null && selectingCity.producingUnit == null)
        {
            MapCell produceCell = null;

            switch (unitTemplate.property.transportProperty.transportType)
            {
                case TransportType.Vechicle:
                    foreach (Area area in selectingCity.areaList)
                    {
                        if (area.buildingProperty.buildingType == BuildingType.MilitaryBase)
                            produceCell = area.belongCell;
                    }
                    break;

                case TransportType.Aircarft:
                    foreach (Area area in selectingCity.areaList)
                    {
                        if (area.buildingProperty.buildingType == BuildingType.AirForceBase)
                            produceCell = area.belongCell;
                    }
                    break;

                case TransportType.Ship:
                    foreach (Area area in selectingCity.areaList)
                    {
                        if (area.buildingProperty.buildingType == BuildingType.NavalBase)
                            produceCell = area.belongCell;
                    }
                    break;
            }

            selectingCity.Produce(unitTemplate, produceCell);
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.AddProduceFail());
        }       
    }
}
