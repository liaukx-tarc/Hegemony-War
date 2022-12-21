using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelection : MonoBehaviour
{
    public Image buildingIcon;
    public Image templateBackground;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI produceVariable;
    
    public BuildingProperty building;

    public void Start()
    {
        buildingIcon.sprite = building.icon;
        buildingIcon.color = building.iconColor;
        buildingName.text = building.buildingName;
        produceVariable.text = building.produceCost.ToString();
        templateBackground.color = building.backgroundColor;
    }

    public void onClick()
    {
        WorldController.instance.uiController.ClickSound();
        City city = (City)WorldController.instance.playerController.selectedBuilding;

        if (city.producingArea == null && city.producingUnit == null)
        {
            WorldController.instance.playerController.BuildArea(building);
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.AddProduceFail());
        }

    }
}
