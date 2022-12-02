using System;
using System.Reflection;
using UnityEngine;

public enum TransportType
{
    Infantry,
    Vechicle,
    Aircarft,
    Ship
}

[CreateAssetMenu(fileName = "TransportProperty", menuName = "ScriptableObject/TransportProperty")]
public class TransportProperty : ScriptableObject
{
    [Header("Info")]
    public string transportName;
    public GameObject model;

    public TransportType transportType;

    [Header("Accessory Slots Number"), Range(0, 3)] public int heavyWeaponNum;
    [Range(0, 3)] public int mediumWeaponNum;
    [Range(0, 3)] public int lightWeaponNum;
    [Range(0, 3)] public int defenceEquipmentNum;
    [Range(0, 4)] public int auxiliaryEquipmentNum;

    [Header("Property")]
    public int maxHp = 0;
    public int armor = 0;
    public int speed = 0;
    public int load = 0;

    [Header("Cost")]
    public int budgetCost = 0;
    public int developCost = 0;
    public int maintanceCost = 0;
    public int produceCost = 0;
}

