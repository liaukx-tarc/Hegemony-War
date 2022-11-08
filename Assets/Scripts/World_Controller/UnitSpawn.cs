using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unit;
    public Camera cam;
    GameObject temp, temp2, temp3;
    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        temp = Instantiate(unit, new Vector3(0, 1.5f, 0), Quaternion.identity);
        temp.name = "Unit 1";
    }

    void Update()
    {

    }

    public void GenerateUnit()
    {
        if (WorldController.playerList[0].GetComponent<PlayerController>())
        {
            Vector2 tempPos;
            do
            {
                tempPos = new Vector2(Random.Range(0, WorldController.map.GetLength(0)),
                Random.Range(0, WorldController.map.GetLength(1)));
            } while (WorldController.map[(int)tempPos.x, (int)tempPos.y].cost == 0 || WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Coast ||
            WorldController.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Ocean);

            WorldController.playerList[0].GetComponent<PlayerController>().unitList.Add(temp.GetComponent<Unit>());
            temp.GetComponent<Unit>().currentPos = WorldController.map[(int)tempPos.x, (int)tempPos.y];
            temp.transform.position = WorldController.map[(int)tempPos.x, (int)tempPos.y].transform.position + new Vector3(0, 1.5f, 0);
            WorldController.map[(int)tempPos.x, (int)tempPos.y].units.Add(temp.GetComponent<Unit>());

            cam.GetComponent<CameraControl>().StartCamera(temp.GetComponent<Unit>());

        }
    }
}
