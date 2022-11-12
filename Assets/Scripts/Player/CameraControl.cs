using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    const string CameraUp = "CameraUp";
    const string CameraDown = "CameraDown";
    const string CameraLeft = "CameraLeft";
    const string CameraRight = "CameraRight";

    public int cameraSpeed;
    public int maxZoomOut, maxZoomIn;

    MapCell[,] displayMap = new MapCell[50, 32];

    int moveCount, maxCount;
    public float maxUp, maxDown, camPosY, moveRange;
    float upCount = 0;
    float downCount = 0;
    float leftCount = 0;
    float rightCount = 0;
    bool isMoving = false;
    bool mapHaveMove = false;
    Vector2 mapMove;
    

    void Update()
    {
        if(isMoving)
        {
            if(mapMove.x > 0)
            {
                leftCount += mapMove.x;
                if (leftCount > 2)
                {
                    leftCount -= 2;
                    MapCulling((int)Direction.Left);
                }
            }

            else if(mapMove.x < 0)
            {
                rightCount -= mapMove.x;
                if (rightCount > 2)
                {
                    rightCount -= 2;
                    MapCulling((int)Direction.Right);
                }
            }

            if(mapMove.y < 0)
            {
                camPosY += mapMove.y;
                upCount -= mapMove.y;

                if (upCount > 2)
                {
                    upCount -= 2;
                    MapCulling((int)Direction.Up);
                }
            }

            else if(mapMove.y > 0)
            {
                camPosY += mapMove.y;
                downCount += mapMove.y;
                
                if (downCount > 2)
                {
                    downCount -= 2;
                    MapCulling((int)Direction.Down);
                }
            }

            mapHaveMove = true;
            moveCount++;

            if (moveCount == maxCount)
            {
                isMoving = false;
            }
                
        }

        else
        {
            mapMove = new Vector2(0, 0);

            //Camera Move
            if (Input.GetButton(CameraUp))
            {
                moveRange = -cameraSpeed * Time.deltaTime;
                if (camPosY + moveRange < maxUp)
                    moveRange = maxUp - camPosY;

                camPosY += moveRange;
                upCount -= moveRange;

                if (upCount > 2)
                {
                    upCount -= 2;
                    MapCulling((int)Direction.Up);
                }

                mapMove.y += moveRange;
                mapHaveMove = true;
            }

            if (Input.GetButton(CameraDown))
            {
                moveRange = cameraSpeed * Time.deltaTime;
                if (camPosY + moveRange > maxDown)
                    moveRange = maxDown - camPosY;

                camPosY += moveRange;
                downCount += moveRange;
                if (downCount > 2)
                {
                    downCount -= 2;
                    MapCulling((int)Direction.Down);
                }

                mapMove.y += moveRange;
                mapHaveMove = true;
            }

            if (Input.GetButton(CameraLeft))
            {
                leftCount += cameraSpeed * Time.deltaTime;

                if (leftCount > 2)
                {
                    leftCount -= 2;
                    MapCulling((int)Direction.Left);
                }

                mapMove.x += cameraSpeed * Time.deltaTime;
                mapHaveMove = true;
            }

            if (Input.GetButton(CameraRight))
            {
                rightCount += cameraSpeed * Time.deltaTime;

                if (rightCount > 2)
                {
                    rightCount -= 2;
                    MapCulling((int)Direction.Right);
                }

                mapMove.x += -cameraSpeed * Time.deltaTime;
                mapHaveMove = true;
            }

            //Camera Zoom
            if (Input.mouseScrollDelta.y > 0)
            {
                if (this.transform.position.y > maxZoomIn)
                {
                    camPosY += transform.forward.y * Input.mouseScrollDelta.y;
                    this.transform.position += transform.forward * Input.mouseScrollDelta.y;

                    this.transform.position = new Vector3(this.transform.position.x,
                        Mathf.Max(maxZoomIn, this.transform.position.y), this.transform.position.z);

                    maxUp = 0 + this.transform.position.y;
                    maxDown = WorldController.map.GetLength(1) * 1.75f + this.transform.position.y;
                }
            }

            else if (Input.mouseScrollDelta.y < 0)
            {
                if (this.transform.position.y < maxZoomOut)
                {
                    camPosY += transform.forward.y * Input.mouseScrollDelta.y;
                    this.transform.position += transform.forward * Input.mouseScrollDelta.y;

                    this.transform.position = new Vector3(this.transform.position.x,
                       Mathf.Min(maxZoomOut, this.transform.position.y), this.transform.position.z);

                    maxUp = 0 + this.transform.position.y;
                    maxDown = WorldController.map.GetLength(1) * 1.75f + this.transform.position.y;
                }
            }
        }

        if(mapHaveMove)
        {
            foreach (MapCell cell in displayMap)
            {
                if (cell != null)
                {
                    cell.transform.position += new Vector3(mapMove.x, 0, mapMove.y);

                    foreach (Unit unit in cell.unitsList)
                        unit.transform.position = cell.transform.position + new Vector3(0, 1.2f, 0);

                    if (cell.building != null)
                        cell.building.transform.position = cell.transform.position + new Vector3(0, 1.2f, 0);
                }
            }

            mapHaveMove = false;
        }   
    }

    public void StartCamera(Unit startedUnit)
    {
        foreach (MapCell cell in WorldController.map)
        {
            cell.GetComponent<Renderer>().enabled = false;
            cell.GetComponent<Collider>().enabled = false;

            for (int i = 0; i < cell.transform.childCount; i++)
            {
                cell.transform.GetChild(i).gameObject.SetActive(false);
            }

            foreach (Unit unit in cell.unitsList)
            {
                unit.GetComponent<Renderer>().enabled = false;
                unit.GetComponent<Collider>().enabled = false;
            }

            if (cell.building != null)
            {
                cell.building.GetComponent<Renderer>().enabled = false;
                cell.building.GetComponent<Collider>().enabled = false;
            }
        }

        MapCell startedCell = startedUnit.currentPos;

        for (int w = -25; w < 25; w++)
        {
            for (int h = -16; h < 16; h++)
            {
                int x = (int)startedCell.position.x + w;
                int y = (int)startedCell.position.y + h - 6;

                if (x < 0)
                    x += WorldController.map.GetLength(0);
                else if (x >= WorldController.map.GetLength(0))
                    x -= WorldController.map.GetLength(0);

                if (y >= 0 && y < WorldController.map.GetLength(1))
                {
                    MapCell cell = WorldController.map[x, y];

                    cell.transform.position = new Vector3((w * 2f) - (y % 2), 0.0f, h * -1.75f);                  
                    cell.GetComponent<Renderer>().enabled = true;
                    cell.GetComponent<Collider>().enabled = true;

                    for (int i = 0; i < cell.transform.childCount; i++)
                    {
                        cell.transform.GetChild(i).gameObject.SetActive(true);
                    }

                    foreach (Unit unit in cell.unitsList)
                    {
                        unit.transform.position = cell.transform.position + new Vector3(0, 1.2f, 0);
                        unit.GetComponent<Renderer>().enabled = true;
                        unit.GetComponent<Collider>().enabled = true;
                    }

                    if (cell.building != null)
                    {
                        cell.building.transform.position = cell.transform.position + new Vector3(0, 1.2f, 0);
                        cell.building.GetComponent<Renderer>().enabled = true;
                        cell.building.GetComponent<Collider>().enabled = true;
                    }

                    displayMap[w + 25, h + 16] = cell;
                }
            }
        }

        maxUp = 0 + this.transform.position.y;
        maxDown = WorldController.map.GetLength(1) * 1.75f + this.transform.position.y;
        camPosY = startedUnit.currentPos.position.y * 1.75f - this.transform.position.z ;
    }

    public void MoveCamera(MapCell targetCell)
    {
        moveCount = 0;
        maxCount = (int)Mathf.Max(Mathf.Abs(displayMap[25, 16].position.x - targetCell.position.x), Mathf.Abs(displayMap[25, 16].position.y - targetCell.position.y));
        if (maxCount > 0)
            isMoving = true;
        else
            return;

        Vector2 targetPos = new Vector2(displayMap[25, 16].transform.position.x - (displayMap[25, 16].position.x - targetCell.position.x) * 2,
            displayMap[25, 16].transform.position.z - (displayMap[25, 16].position.y - targetCell.position.y) * -1.75f);


        if (this.transform.position.x - targetPos.x != 0)
            mapMove.x = (this.transform.position.x - targetPos.x) / maxCount;

        if (this.transform.position.z + 20 - targetPos.y != 0)
            mapMove.y = (this.transform.position.z + 10 - targetPos.y) / maxCount;

        this.transform.position = new Vector3(this.transform.position.x, 11, -20);
        maxUp = 0 + this.transform.position.y;
        maxDown = WorldController.map.GetLength(1) * 1.75f + this.transform.position.y;
        camPosY = targetCell.position.y * 1.75f - this.transform.position.z;
    }

    void MapCulling(int direction)
    {
        switch (direction)
        {
            case (int)Direction.Up:
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 31; y >= 0; y--)
                    {
                        //disable the first row cell
                        if (y == 31 && displayMap[x, y] != null)
                        {
                            displayMap[x, y].GetComponent<Renderer>().enabled = false;
                            displayMap[x, y].GetComponent<Collider>().enabled = false;

                            for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                            {
                                displayMap[x, y].transform.GetChild(i).gameObject.SetActive(false);
                            }

                            foreach (Unit unit in displayMap[x, y].unitsList)
                            {
                                unit.GetComponent<Renderer>().enabled = false;
                                unit.GetComponent<Collider>().enabled = false;
                            }

                            if (displayMap[x, y].building != null)
                            {
                                displayMap[x, y].building.GetComponent<Renderer>().enabled = false;
                                displayMap[x, y].building.GetComponent<Collider>().enabled = false;
                            }
                        }

                        //swap array
                        if (y > 0)
                            displayMap[x, y] = displayMap[x, y - 1];
                        else
                        {
                            if (displayMap[x, y] != null && displayMap[x, y].position.y - 1 >= 0)
                            {
                                displayMap[x, y] = WorldController.map[(int)displayMap[x, y].position.x, (int)displayMap[x, y].position.y - 1];
                                //enable the cell
                                if (displayMap[x, y].position.y % 2 == 0)
                                    displayMap[x, y].transform.position = displayMap[x, y + 1].transform.position + new Vector3(1, 0, 1.75f);
                                else
                                    displayMap[x, y].transform.position = displayMap[x, y + 1].transform.position + new Vector3(-1, 0, 1.75f);

                                displayMap[x, y].GetComponent<Renderer>().enabled = true;
                                displayMap[x, y].GetComponent<Collider>().enabled = true;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(true);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = true;
                                    unit.GetComponent<Collider>().enabled = true;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = true;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = true;
                                }
                            }

                            else
                                displayMap[x, y] = null;
                        }
                    }
                }
                break;

            case (int)Direction.Down:
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        //disable the first row cell
                        if (y == 0 && displayMap[x, y] != null)
                        {
                            displayMap[x, y].GetComponent<Renderer>().enabled = false;
                            displayMap[x, y].GetComponent<Collider>().enabled = false;

                            for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                            {
                                displayMap[x, y].transform.GetChild(i).gameObject.SetActive(false);
                            }

                            foreach (Unit unit in displayMap[x, y].unitsList)
                            {
                                unit.GetComponent<Renderer>().enabled = false;
                                unit.GetComponent<Collider>().enabled = false;
                            }

                            if (displayMap[x, y].building != null)
                            {
                                displayMap[x, y].building.GetComponent<Renderer>().enabled = false;
                                displayMap[x, y].building.GetComponent<Collider>().enabled = false;
                            }
                        }

                        //swap array
                        if (y < 31)
                            displayMap[x, y] = displayMap[x, y + 1];
                        else
                        {
                            if (displayMap[x, y] != null && displayMap[x, y].position.y + 1 < WorldController.map.GetLength(1))
                            {
                                displayMap[x, y] = WorldController.map[(int)displayMap[x, y].position.x, (int)displayMap[x, y].position.y + 1];
                                //enable the cell
                                if (displayMap[x, y].position.y % 2 == 0)
                                    displayMap[x, y].transform.position = displayMap[x, y - 1].transform.position + new Vector3(1, 0, -1.75f);
                                else
                                    displayMap[x, y].transform.position = displayMap[x, y - 1].transform.position + new Vector3(-1, 0, -1.75f);

                                displayMap[x, y].GetComponent<Renderer>().enabled = true;
                                displayMap[x, y].GetComponent<Collider>().enabled = true;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(true);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = true;
                                    unit.GetComponent<Collider>().enabled = true;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = true;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = true;
                                }
                            }

                            else
                                displayMap[x, y] = null;
                        }
                    }
                }
                break;

            case (int)Direction.Left:
                for (int x = 49; x >= 0; x--)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (displayMap[x, y] != null)
                        {
                            //disable the last row cell
                            if (x == 49)
                            {
                                displayMap[x, y].GetComponent<Renderer>().enabled = false;
                                displayMap[x, y].GetComponent<Collider>().enabled = false;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(false);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = false;
                                    unit.GetComponent<Collider>().enabled = false;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = false;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = false;
                                }
                            }

                            //swap array
                            if (x > 0)
                                displayMap[x, y] = displayMap[x - 1, y];
                            else
                            {
                                //replace last row with the next row in map
                                if (displayMap[x, y].position.x - 1 >= 0)
                                    displayMap[x, y] = WorldController.map[(int)displayMap[x, y].position.x - 1, (int)displayMap[x, y].position.y];

                                //replace last row with the first row in map
                                else
                                    displayMap[x, y] = WorldController.map[WorldController.map.GetLength(0) - 1, (int)displayMap[x, y].position.y];

                                //enable the cell
                                displayMap[x, y].transform.position = displayMap[x + 1, y].transform.position - new Vector3(2, 0, 0);
                                displayMap[x, y].GetComponent<Renderer>().enabled = true;
                                displayMap[x, y].GetComponent<Collider>().enabled = true;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(true);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = true;
                                    unit.GetComponent<Collider>().enabled = true;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = true;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = true;
                                }
                            }
                        }
                    }
                }
                break;

            case (int)Direction.Right:
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (displayMap[x, y] != null)
                        {
                            //disable the first row cell
                            if (x == 0)
                            {
                                displayMap[x, y].GetComponent<Renderer>().enabled = false;
                                displayMap[x, y].GetComponent<Collider>().enabled = false;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(false);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = false;
                                    unit.GetComponent<Collider>().enabled = false;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = false;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = false;
                                }
                            }

                            //swap array
                            if (x < 49)
                                displayMap[x, y] = displayMap[x + 1, y];
                            else
                            {
                                //replace last row with the next row in map
                                if (displayMap[x, y].position.x + 1 < WorldController.map.GetLength(0))
                                    displayMap[x, y] = WorldController.map[(int)displayMap[x, y].position.x + 1, (int)displayMap[x, y].position.y];

                                //replace last row with the first row in map
                                else
                                    displayMap[x, y] = WorldController.map[0, (int)displayMap[x, y].position.y];

                                //enable the cell
                                displayMap[x, y].transform.position = displayMap[x - 1, y].transform.position + new Vector3(2, 0, 0);
                                displayMap[x, y].GetComponent<Renderer>().enabled = true;
                                displayMap[x, y].GetComponent<Collider>().enabled = true;

                                for (int i = 0; i < displayMap[x, y].transform.childCount; i++)
                                {
                                    displayMap[x, y].transform.GetChild(i).gameObject.SetActive(true);
                                }

                                foreach (Unit unit in displayMap[x, y].unitsList)
                                {
                                    unit.GetComponent<Renderer>().enabled = true;
                                    unit.GetComponent<Collider>().enabled = true;
                                }

                                if (displayMap[x, y].building != null)
                                {
                                    displayMap[x, y].building.GetComponent<Renderer>().enabled = true;
                                    displayMap[x, y].building.GetComponent<Collider>().enabled = true;
                                }
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

}
