using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelection : MonoBehaviour
{
    public Image buildingIcon;
    public Image templateBackground;
    public TextMeshProUGUI buildingName;
    public BuildingProperty building;

    public void Start()
    {
        buildingIcon.sprite = building.icon;
        buildingIcon.color = building.iconColor;
        buildingName.text = building.buildingName;
        templateBackground.color = building.backgroundColor;
    }

    public void onClick()
    {
        WorldController.playerController.BuildArea(building);
    }
}
