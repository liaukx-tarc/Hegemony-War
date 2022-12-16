using UnityEngine;

public enum AccessoriesTypes
{
    HeavyWeapon,
    MediumWeapon,
    LightWeapon,
    DefenceEquipment,
    AuxiliaryEquipment,
    FireControlSystem,
    engine
}

[CreateAssetMenu(fileName = "AccessoryProperty", menuName = "ScriptableObject/AccessoryProperty")]
public class AccessoryProperty : ScriptableObject
{
    [Header("Info")]
    public string accessoryName;
    public Sprite icon;
    public GameObject model;

    [Space]
    public TransportType[] transportType;

    [Space]
    public AccessoriesTypes[] accessoryTypes;

    [Space]
    public UnitTag accessoryTag;
    
    [Header("Property")]
    public int maxHp = 0;
    public int armor = 0;
    public int damage = 0;
    public int range = 0;
    public int speed = 0;
    public int weight = 0;

    [Header("Cost")]
    public int budgetCost = 0;
    public int developCost = 0;
    public int maintanceCost = 0;
    public int produceCost = 0;
}