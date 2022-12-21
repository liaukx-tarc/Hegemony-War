using UnityEngine;

public class Building : MapObject
{
    //Data
    [Header("Building Data")]
    public Player player;
    public BuildingProperty buildingProperty;
    public MapCell belongCell;

    [Header("Building Info")]
    public int maxHP;
    public int currentHp;
    public int defense;
    public int damage;

    [Space]
    public bool isDestroy;

    virtual public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell)
    {
        this.rendererCpn = rendererCpn;
        this.buildingProperty = buildingProperty;
        this.belongCell = belongCell;

        maxHP = currentHp = buildingProperty.healthPoint;
        defense = buildingProperty.defense;
        damage = buildingProperty.damage;

        slider.value = slider.maxValue = maxHP;
        icon.sprite = buildingProperty.icon;
        iconBackground.color = player.playerColor;
    }
}
