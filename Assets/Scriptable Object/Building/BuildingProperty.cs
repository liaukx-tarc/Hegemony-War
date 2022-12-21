using UnityEngine;

public enum BuildingType
{
    City,
    AgriculturalArea,
    //ResearchCenter,
    IndustrialArea,
    CommercialCenter,
    Harbor,
    Airport,
    MilitaryBase,
    NavalBase,
    AirForceBase,
    MilitaryFactory,
    NavalShipyard,
    FlightTestCenter
}

[CreateAssetMenu(fileName = "BuildingProperty", menuName = "ScriptableObject/BuildingProperty")]
public class BuildingProperty : ScriptableObject
{
    [Header("Info")]
    public string buildingName;
    public Sprite icon;
    public GameObject model;
    public Color backgroundColor;
    public Color iconColor;
    public BuildingType buildingType;

    [Header("Property")]
    public int buildingRange;
    public int produceCost;
    public int maintenanceCost;
    public int healthPoint;
    public int defense;
    public int damage;

    [Header("Resource")]
    public int money;
    public int food;
    public int productivity;
    public int sciencePoint;
    public int developmentPoint;
}
