using UnityEngine;

public class Node
{
    public Node parent;
    public float gCost; //distance between start node
    public float hCost; //distance between target node
    public float fCost; //gCost + hCost
    public MapCell mapCell;

    public Node(MapCell mapcell)
    {
        this.mapCell = mapcell;
    }
}
