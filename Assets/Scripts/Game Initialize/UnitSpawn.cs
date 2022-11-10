using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unit;
    public int unitNumber;
    List<GameObject> tempUnitList = new List<GameObject>(); 
    public void GenerateUnit()
    {
        for (int i = 0; i < unitNumber; i++)
        {
            tempUnitList.Add(Instantiate(unit, new Vector3(0, 1.2f, 0), Quaternion.identity));
            tempUnitList[i].name = "Unit " + (i + 1);
        }

        Player player = WorldController.playerList[0].GetComponent<Player>();

        if (player != null)
        {
            foreach (GameObject temp in tempUnitList)
            {
                Vector2 tempPos;
                do
                {
                    //Random spawn unit at a position
                    tempPos = new Vector2(Random.Range(0, WorldController.map.GetLength(0)),
                    Random.Range(0, WorldController.map.GetLength(1)));

                } while (WorldController.map[(int)tempPos.x, (int)tempPos.y].cost == 0 || WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Coast ||
                             WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Ocean);

                Unit unit = temp.GetComponent<Unit>();

                player.unitList.Add(unit);
                unit.currentPos = WorldController.map[(int)tempPos.x, (int)tempPos.y];
                temp.transform.position = WorldController.map[(int)tempPos.x, (int)tempPos.y].transform.position + new Vector3(0, 1.2f, 0);
                WorldController.map[(int)tempPos.x, (int)tempPos.y].units.Add(unit);
                unit.player = player;
            }

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>().StartCamera(tempUnitList[0].GetComponent<Unit>());

        }

        GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>().TurnStart();//inti complete start turn

        Destroy(this.gameObject);
    }
}
