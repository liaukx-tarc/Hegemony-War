using UnityEngine;

[CreateAssetMenu(fileName = "UnitProperty", menuName = "ScriptableObject/UnitProperty")]
public class UnitProperty : ScriptableObject
{
    [Header("Info")]
    public string unitName;
    public Sprite unitIcon;
    public bool isSettler;

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

    public TransportProperty transportProperty;
    public AccessoryProperty[] accessoryProperty;

    public void Create(string name, Sprite icon, int maxHp, int armor, int damage, int range, int speed, int weight, int budgetCost, int developCost, int maintanceCost, int produceCost, TransportProperty transportProperty, AccessoryProperty[] accessoryProperty, int accessorySlotsNum)
    {
        unitName = name;
        unitIcon = icon;
        this.maxHp = maxHp;
        this.armor = armor;
        this.damage = damage;
        this.range = range;
        this.speed = speed;
        this.weight = weight;
        this.budgetCost = budgetCost;
        this.developCost = developCost;
        this.maintanceCost = maintanceCost;
        this.produceCost = produceCost;
        this.transportProperty = transportProperty;

        this.accessoryProperty = new AccessoryProperty[accessorySlotsNum];
        
        for (int i = 0; i < accessoryProperty.Length; i++)
        {
            this.accessoryProperty[i] = accessoryProperty[i];
        }
    }

    public void Create(Sprite icon)
    {
        unitIcon = icon;
    }
}
