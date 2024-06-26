using System.Collections.Generic;
using UnityEngine;

public enum MapTypeName
{
    Ocean,
    Plain,
    Coast,
    Snow,
    Forest,
    Desert,
    Marsh
}

public class MapCell : MonoBehaviour
{
    //Map Information
    public Vector2 position; //the position of the map cell in the whole map.
    public CubePosition cubePosition;
    public MapCell[] neighborCell;
    public int mapType;
    public float cost;

    public City belongCity;

    public int groundConnect;
    public int seaConnect;

    //Unit
    public Unit unit;
    public Building building;
    public List<MapObject>mapObjectList = new List<MapObject>();

    public int q, r, s;

    public void setPosition(Vector2 position)
    {
        this.position = position;
        cubePosition = new CubePosition(position);

        q = cubePosition.q;
        r = cubePosition.r;
        s = cubePosition.s;
    }

    public List<MapCell> CheckCellInRange(int range)
    {
        List<MapCell> cellInRange = new List<MapCell>();

        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                int s = -q - r;
                CubePosition cellCubePos = cubePosition;
                cellCubePos = new CubePosition(cellCubePos.q + q, cellCubePos.r + r, cellCubePos.s + s);
                Vector2 cellPos = cellCubePos.CubeToOffset();

                if (cellPos.x < 0)
                {
                    cellPos.x = WorldController.instance.map.GetLength(0) + cellPos.x;
                }

                else if (cellPos.x >= WorldController.instance.map.GetLength(0))
                    cellPos.x = cellPos.x - WorldController.instance.map.GetLength(0);

                if (cellPos.y >= 0 && cellPos.y < WorldController.instance.map.GetLength(1) && cellPos != position)
                {
                    cellInRange.Add(WorldController.instance.map[(int)cellPos.x, (int)cellPos.y]);
                }
                    
            }
        }

        return cellInRange;
    }

    public List<MapCell> CheckCellAlongLine(MapCell targetCell)
    {
        List<MapCell> cellAlongLine = new List<MapCell>();

        CubePosition targetPos = targetCell.cubePosition;
        int distance = cubePosition.CheckDistance(targetPos);


        for (int d = 0; d < distance; d++)
        {
            Vector2 cellPos = new CubePosition(
                cubePosition.q + (targetPos.q - cubePosition.q) * (1.0f / distance * d),
                 cubePosition.r + (targetPos.r - cubePosition.r) * (1.0f / distance * d),
                 cubePosition.s + (targetPos.s - cubePosition.s) * (1.0f / distance * d)).CubeToOffset();

            cellAlongLine.Add(WorldController.instance.map[(int)cellPos.x, (int)cellPos.y]);
        }

        return cellAlongLine;
    }
}