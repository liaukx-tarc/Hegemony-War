using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unit;
    public Camera cam;
    public int unitNumber;
    List<GameObject> tempUnitList = new List<GameObject>();
    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < unitNumber; i++)
        {
            tempUnitList.Add(Instantiate(unit, new Vector3(0, 1.5f, 0), Quaternion.identity));
            tempUnitList[i].name = "Unit " + (i + 1);
        }
    }

    public void GenerateUnit()
    {
        if (WorldController.playerList[0].GetComponent<PlayerController>())
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

                WorldController.playerList[0].GetComponent<PlayerController>().unitList.Add(temp.GetComponent<Unit>());
                temp.GetComponent<Unit>().currentPos = WorldController.map[(int)tempPos.x, (int)tempPos.y];
                temp.transform.position = WorldController.map[(int)tempPos.x, (int)tempPos.y].transform.position + new Vector3(0, 1.5f, 0);
                WorldController.map[(int)tempPos.x, (int)tempPos.y].units.Add(temp.GetComponent<Unit>());
            }

            cam.GetComponent<CameraControl>().StartCamera(tempUnitList[0].GetComponent<Unit>());

        }

        GetComponent<WorldController>().TurnStart();//inti complete start turn
    }
}
