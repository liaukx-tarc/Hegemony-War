using UnityEngine;

public class UnitController : MonoBehaviour
{
    public GameObject unitPrefab;

    [Header("Unit HP Restore")]
    public int hpRestore;

    public void Start_UnitController()
    {
        
    }

    public void GenerateUnit(Player player, UnitProperty unitProperty,MapCell generateCell)
    {
        GameObject unitObj = Instantiate(unitPrefab, generateCell.transform.position + Vector3.up, Quaternion.identity, player.unitListObj.transform);
        GameObject unitModel = Instantiate(unitProperty.transportProperty.model, unitObj.transform.position, Quaternion.identity, unitObj.transform);
        Unit unit = unitObj.GetComponent<Unit>();

        unitObj.name = player + unitProperty.unitName;
        unit.currentPos = generateCell;
        unit.player = player;

        unit.property = unitProperty;
        unit.rendererCpn = unitModel.GetComponent<Renderer>();
        unit.rendererCpn.material.color = player.playerColor;
        unit.InitializeUnit();

        player.unitList.Add(unit);
        WorldController.instance.activeUnitList.Add(unit);
        generateCell.unit = unit;

        generateCell.mapObjectList.Add(unit);
    }
}
