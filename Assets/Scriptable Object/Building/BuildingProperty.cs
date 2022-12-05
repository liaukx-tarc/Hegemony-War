using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    City,
    AgriculturalArea,
    ResearchCenter,
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


public class BuildingProperty : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public GameObject model;
    public BuildingType buildingType;

    [Header("Produce")]
    public int money;
    public int food;
    public int productivity;
    public int developmentPoint;
    public int sciencePoint;
}
