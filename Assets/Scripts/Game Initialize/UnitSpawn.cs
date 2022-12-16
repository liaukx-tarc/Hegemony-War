using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitObj;
    public int unitNumber;

    public Material mtrAllianceUnit;
    public Material mtrEnemeyUnit;

    public UnitProperty settlerProperty;

    Vector2 tempPos;

    public void GenerateUnit()
    {
        foreach (Player player in WorldController.playerList)
        {
            for (int i = 0; i < unitNumber; i++)
            {
                do
                {
                    //Random spawn unit at a position
                    tempPos = new Vector2(Random.Range(0, WorldController.map.GetLength(0)),
                    Random.Range(0, WorldController.map.GetLength(1)));

                } while (WorldController.map[(int)tempPos.x, (int)tempPos.y].cost == 0 || WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Coast ||
                             WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Ocean);

                WorldController.unitController.GenerateUnit(player, settlerProperty, WorldController.map[(int)tempPos.x, (int)tempPos.y]);
            }
        }
    }
}
