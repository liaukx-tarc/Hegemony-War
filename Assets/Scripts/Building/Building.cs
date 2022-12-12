using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //Component
    public Renderer rendererCpn;
    public Collider colliderCpn;

    //Data
    public BuildingProperty buildingProperty;
    public MapCell belongCell;
    
    public int healthPoint;
    public int defense;
    public int damage;

    virtual public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell)
    {
        this.rendererCpn = rendererCpn;
        this.colliderCpn = colliderCpn;
        this.buildingProperty = buildingProperty;
        this.belongCell = belongCell;
        
        healthPoint = buildingProperty.healthPoint;
        defense = buildingProperty.defense;
        damage = buildingProperty.damage;
    }

    virtual public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell, City city)
    {
        this.rendererCpn = rendererCpn;
        this.colliderCpn = colliderCpn;
        this.buildingProperty = buildingProperty;
        this.belongCell = belongCell;

        healthPoint = buildingProperty.healthPoint;
        defense = buildingProperty.defense;
        damage = buildingProperty.damage;
    }
}
