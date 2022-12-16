using UnityEngine;

public class Area : Building
{
    public City belongCity;

    override public void CreateBuilding(Renderer rendererCpn, Collider colliderCpn, BuildingProperty buildingProperty, MapCell belongCell, City city)
    {
        base.CreateBuilding(rendererCpn, colliderCpn, buildingProperty, belongCell);

        belongCity = city;
    }
}
