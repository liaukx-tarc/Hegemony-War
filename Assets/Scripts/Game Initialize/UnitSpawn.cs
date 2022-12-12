using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitObj;
    public int unitNumber;

    public Material mtrAllianceUnit;
    public Material mtrEnemeyUnit;

    public UnitProperty unitProperty;

    GameObject tempUnit;
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

                tempUnit = Instantiate(unitObj, WorldController.map[(int)tempPos.x, (int)tempPos.y].transform.position + new Vector3(0, 1.2f, 0), 
                    Quaternion.identity, player.unitListObj.transform);
                tempUnit.name = "Unit " + (i + 1);
               
                Unit unit = tempUnit.GetComponent<Unit>();
                unit.property = unitProperty;
                unit.InitializeUnit();

                if (player is HumanPlayer)
                {
                    tempUnit.GetComponent<Renderer>().material = mtrAllianceUnit;
                }

                else if (player is AI_Player)
                {
                    tempUnit.GetComponent<Renderer>().material = mtrEnemeyUnit;
                }

                player.unitList.Add(unit);
                unit.currentPos = WorldController.map[(int)tempPos.x, (int)tempPos.y];
                unit.player = player;
                WorldController.map[(int)tempPos.x, (int)tempPos.y].unitsList.Add(unit);
            }
        }
    }
}
