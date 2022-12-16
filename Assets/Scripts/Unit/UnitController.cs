using UnityEngine;

public class UnitController : MonoBehaviour
{
    public GameObject unitPrefab;

    public void Start_UnitController()
    {

    }

    public void GenerateUnit(Player player, UnitProperty unitProperty,MapCell generateCell)
    {
        GameObject unitObj = Instantiate(unitPrefab, generateCell.transform.position + Vector3.up, Quaternion.identity, player.unitListObj.transform);
        GameObject unitModel = Instantiate(unitProperty.transportProperty.model, unitObj.transform.position, Quaternion.identity, unitObj.transform);
        Unit unit = unitObj.GetComponent<Unit>();

        unitObj.name = unitProperty.unitName;
        unit.currentPos = generateCell;
        unit.player = player;

        unit.property = unitProperty;
        unit.rendererCpn = unitModel.GetComponent<Renderer>();
        unit.rendererCpn.material.color = player.playerColor;
        unit.InitializeUnit();

        player.unitList.Add(unit);

        switch (unit.property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                generateCell.groundUnit = unit;
                break;

            case TransportType.Aircarft:
                generateCell.airForceUnit = unit;
                break;

            case TransportType.Ship:
                generateCell.navalUnit = unit;
                break;
        }

        generateCell.mapObjectList.Add(unit);
    }
}
