using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public int cameraSpeed;
    public float zoomSpeed;
    public int maxZoomIn, maxZoomOut, halfDisplayX, halfDisplayY;

    public Vector2 maxDisplayMaxSize;
    MapCell[,] displayMap;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    const string MapTag = "Map";
    const string CameraUp = "CameraUp";
    const string CameraDown = "CameraDown";
    const string CameraLeft = "CameraLeft";
    const string CameraRight = "CameraRight";

    RaycastHit hitInfo;
    MapCell perviousMiddleCell;
    MapCell currentMiddleCell;
    MapCell tempCell;
    Vector2 curMovedDis;
    Vector3 heighFix = Vector3.up;
    bool isCulled;
    int culledCount = 0;
    

    private void Update()
    {
        hitInfo = new RaycastHit();
        if(!WorldController.instance.uiController.isUIOpen)
        {
            //Check Middle Cell
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, 100, LayerMask.GetMask(MapTag)))
            {
                currentMiddleCell = hitInfo.collider.GetComponent<MapCell>();
                if (perviousMiddleCell != null && currentMiddleCell != perviousMiddleCell)
                {
                    //Moving Up
                    if (curMovedDis.y <= -1.75f)
                    {
                        MapCulling(Direction.Up);
                        curMovedDis.y += 1.75f;
                    }


                    //Moving Down
                    else if (curMovedDis.y >= 1.75f)
                    {
                        MapCulling(Direction.Down);
                        curMovedDis.y -= 1.75f;
                    }


                    //Moving Left
                    if (curMovedDis.x <= -2.0f)
                    {
                        MapCulling(Direction.Left);
                        curMovedDis.x += 2.0f;
                    }

                    //Moving Right
                    else if (curMovedDis.x >= 2.0f)
                    {
                        MapCulling(Direction.Right);
                        curMovedDis.x -= 2.0f;
                    }
                }

                perviousMiddleCell = currentMiddleCell;
            }

            //Camera Move
            if (Input.GetButton(CameraUp) && currentMiddleCell.position.y != 0)
            {
                curMovedDis.y -= cameraSpeed * Time.deltaTime;
                this.transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
            }

            if (Input.GetButton(CameraDown) && currentMiddleCell.position.y != WorldController.instance.mapSize.y - 1)
            {
                curMovedDis.y += cameraSpeed * Time.deltaTime;
                this.transform.position += Vector3.back * cameraSpeed * Time.deltaTime;
            }

            if (Input.GetButton(CameraLeft))
            {
                curMovedDis.x -= cameraSpeed * Time.deltaTime;
                this.transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
            }

            if (Input.GetButton(CameraRight))
            {
                curMovedDis.x += cameraSpeed * Time.deltaTime;
                this.transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
            }

            //Camera Zoom
            if (Input.mouseScrollDelta.y > 0)
            {
                if (this.transform.position.y > maxZoomIn)
                {
                    this.transform.position += new Vector3(0, -1, 1) * zoomSpeed;
                    if (this.transform.position.y < maxZoomIn)
                    {
                        float fix = maxZoomIn - this.transform.position.y;
                        this.transform.position -= new Vector3(0, -fix, fix) * zoomSpeed;
                    }
                }
            }

            else if (Input.mouseScrollDelta.y < 0)
            {
                if (this.transform.position.y < maxZoomOut)
                {
                    this.transform.position += new Vector3(0, 1, -1) * zoomSpeed;
                    if (this.transform.position.y < maxZoomIn)
                    {
                        float fix = maxZoomIn - this.transform.position.y;
                        this.transform.position -= new Vector3(0, fix, -fix) * zoomSpeed;
                    }
                }
            }
        }
    }

    public void ChangeMaxZoom(int newZoomOut)
    {
        maxZoomOut = newZoomOut;
        StartCamera(currentMiddleCell);
    }

    public void StartCamera(MapCell startCell)
    {
        displayMap = new MapCell[(int)maxDisplayMaxSize.x, (int)maxDisplayMaxSize.y];

        halfDisplayX = (int)maxDisplayMaxSize.x / 2;
        halfDisplayY = (int)maxDisplayMaxSize.y / 2;

        MapCell cell;

        for (int x = 0; x < WorldController.instance.mapSize.x; x++)
        {
            for (int y = 0; y < WorldController.instance.mapSize.y; y++)
            {
                cell = WorldController.instance.map[x, y];
                cell.gameObject.SetActive(false);

                foreach (MapObject mapObject in cell.mapObjectList)
                {
                    mapObject.rendererCpn.enabled = false;
                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        MoveCamera(startCell);
    }

    public void MoveCamera(MapCell targetCell)
    {
        foreach (MapCell tempCell in displayMap)
        {
            if (tempCell != null)
            {
                foreach (MapObject mapObject in tempCell.mapObjectList)
                {
                    mapObject.rendererCpn.enabled = false;
                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                }

                tempCell.gameObject.SetActive(false);
            }
        }

        MapCell cell;

        for (int w = -halfDisplayX; w < halfDisplayX; w++)
        {
            isCulled = false;
            for (int h = -halfDisplayY; h < halfDisplayY; h++)
            {
                int x = (int)targetCell.position.x + w;
                int y = (int)targetCell.position.y + h;

                //let the map connect together horizontally
                if (x < 0)
                    x += (int)WorldController.instance.mapSize.x;
                else if (x >= WorldController.instance.mapSize.x)
                    x -= (int)WorldController.instance.mapSize.x;


                if (!isCulled)
                {
                    for (int i = 0; i < WorldController.instance.mapSize.y; i++)
                    {
                        cell = WorldController.instance.map[x, i];
                        cell.transform.position = new Vector3((w * 2f) - (Mathf.Abs(i) % 2f), 0.0f, i * -1.75f);

                        foreach (MapObject mapObject in cell.mapObjectList)
                        {
                            mapObject.transform.position = cell.transform.position;

                            if (mapObject.GetType() == typeof(Unit))
                                mapObject.transform.position += heighFix;
                        }
                    }
                    isCulled = true;
                }

                if (y >= 0 && y < WorldController.instance.mapSize.y)
                {
                    cell = WorldController.instance.map[x, y];
                    cell.gameObject.SetActive(true);

                    foreach (MapObject mapObject in cell.mapObjectList)
                    {
                        mapObject.rendererCpn.enabled = true;
                        mapObject.slider.transform.parent.gameObject.SetActive(true);
                    }

                    displayMap[w + halfDisplayX, h + halfDisplayY] = cell;
                }

                else
                    displayMap[w + halfDisplayX, h + halfDisplayY] = null;
            }
        }

        this.transform.position = new Vector3(targetCell.transform.position.x, transform.position.y,
            targetCell.transform.position.z - (transform.position.y / 2 + 1) * 1.75f);

        culledCount = 0;
        curMovedDis.x = curMovedDis.y = 0;
    }

    void MapCulling(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                for (int x = 0; x < maxDisplayMaxSize.x; x++)
                {
                    for (int y = (int)maxDisplayMaxSize.y - 1; y >= 0 ; y--)
                    {
                        if (y == (int)maxDisplayMaxSize.y - 1)
                        {
                            if (displayMap[x, y] != null)
                            {
                                displayMap[x, y].gameObject.SetActive(false);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = false;
                                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                                }
                            }

                            displayMap[x, y] = displayMap[x, y - 1];
                        }


                        else if (y == 0)
                        {
                            if (displayMap[x, y] != null)
                            {
                                if (displayMap[x, y].position.y - 1 >= 0)
                                {
                                    displayMap[x, y] = WorldController.instance.map[(int)displayMap[x, y].position.x, (int)displayMap[x, y].position.y - 1];
                                    displayMap[x, y].gameObject.SetActive(true);

                                    foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                    {
                                        mapObject.rendererCpn.enabled = true;
                                        mapObject.slider.transform.parent.gameObject.SetActive(true);
                                    }
                                }

                                else
                                {
                                    displayMap[x, y] = null;
                                }
                            }
                        }

                        else
                        {
                            displayMap[x, y] = displayMap[x, y - 1];
                        }
                    }
                }
                break;

            case Direction.Down:
                for (int x = 0; x < maxDisplayMaxSize.x; x++)
                {
                    for (int y  = 0; y < maxDisplayMaxSize.y; y++)
                    {
                        if(y == 0)
                        {
                            if (displayMap[x, y] != null)
                            {
                                displayMap[x, y].gameObject.SetActive(false);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = false;
                                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                                }
                            }

                            displayMap[x, y] = displayMap[x, y + 1];
                        }

                        else if(y == (int)maxDisplayMaxSize.y - 1)
                        {
                            if (displayMap[x, y] != null)
                            {
                                if (displayMap[x, y].position.y + 1 < WorldController.instance.mapSize.y)
                                {
                                    displayMap[x, y] = WorldController.instance.map[(int)displayMap[x, y].position.x, (int)displayMap[x, y].position.y + 1];
                                    displayMap[x, y].gameObject.SetActive(true);

                                    foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                    {
                                        mapObject.rendererCpn.enabled = true;
                                        mapObject.slider.transform.parent.gameObject.SetActive(true);
                                    }
                                }

                                else
                                {
                                    displayMap[x, y] = null;
                                }
                            }
                        }

                        else
                        {
                            displayMap[x, y] = displayMap[x, y + 1];
                        }
                    }
                }
                break;

            case Direction.Left:
                isCulled = false;

                for (int x = (int)maxDisplayMaxSize.x - 1; x >= 0 ; x--)
                {
                    for (int y = 0; y < maxDisplayMaxSize.y; y++)
                    {
                        //Last Row
                        if(x == (int)maxDisplayMaxSize.x - 1)
                        {
                            if (displayMap[x, y] != null)
                            {
                                displayMap[x, y].gameObject.SetActive(false);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = false;
                                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                                }
                            }

                            displayMap[x, y] = displayMap[x - 1, y];
                        }

                        //First Row
                        else if(x == 0)
                        {
                            if (displayMap[x,y] != null)
                            {
                                if (displayMap[x, y].position.x - 1 >= 0)
                                {
                                    displayMap[x, y] = WorldController.instance.map[(int)displayMap[x, y].position.x - 1, (int)displayMap[x, y].position.y];
                                }

                                else
                                {
                                    displayMap[x, y] = WorldController.instance.map[(int)WorldController.instance.mapSize.x - 1, (int)displayMap[x, y].position.y];
                                    if (y == halfDisplayY)
                                        culledCount--;
                                }

                                if (!isCulled)
                                {
                                    int mapX = (int)displayMap[x, y].position.x + 1;
                                    if (mapX == WorldController.instance.mapSize.x)
                                        mapX = 0;

                                    for (int i = 0; i < WorldController.instance.mapSize.y; i++)
                                    {
                                        tempCell = WorldController.instance.map[(int)displayMap[x, y].position.x, i];
                                        tempCell.transform.position = WorldController.instance.map[mapX, i].transform.position + Vector3.left * 2;

                                        foreach (MapObject mapObject in tempCell.mapObjectList)
                                        {
                                            mapObject.transform.position = tempCell.transform.position;

                                            if (mapObject.GetType() == typeof(Unit))
                                                mapObject.transform.position += heighFix;
                                        }
                                    }
                                    isCulled = true;
                                }

                                displayMap[x, y].gameObject.SetActive(true);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = true;
                                    mapObject.slider.transform.parent.gameObject.SetActive(true);
                                }
                            }

                            else
                            {
                                displayMap[x, y] = null;
                            }
                        }

                        else
                        {
                            displayMap[x, y] = displayMap[x - 1, y];
                        }
                    }
                }

                break;

            case Direction.Right:
                isCulled = false;
                for (int x = 0; x < maxDisplayMaxSize.x; x++)
                {
                    for (int y = 0; y < maxDisplayMaxSize.y; y++)
                    {
                        //First Row
                        if (x == 0)
                        {
                            if (displayMap[x, y] != null)
                            {
                                displayMap[x, y].gameObject.SetActive(false);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = false;
                                    mapObject.slider.transform.parent.gameObject.SetActive(false);
                                }
                            }

                            displayMap[x, y] = displayMap[x + 1, y];
                        }

                        //Last Row
                        else if (x == (int)maxDisplayMaxSize.x - 1)
                        {
                            if (displayMap[x, y] != null)
                            {
                                if (displayMap[x, y].position.x + 1 < WorldController.instance.mapSize.x)
                                {
                                    displayMap[x, y] = WorldController.instance.map[(int)displayMap[x, y].position.x + 1, (int)displayMap[x, y].position.y];
                                }

                                else
                                {
                                    displayMap[x, y] = WorldController.instance.map[0, (int)displayMap[x, y].position.y];
                                    if (y == halfDisplayY)
                                        culledCount++;
                                }

                                if (!isCulled)
                                {
                                    int mapX = (int)displayMap[x, y].position.x - 1;
                                    if (mapX == -1)
                                        mapX = (int)WorldController.instance.mapSize.x - 1;

                                    for (int i = 0; i < WorldController.instance.mapSize.y; i++)
                                    {
                                        tempCell = WorldController.instance.map[(int)displayMap[x, y].position.x, i];
                                        tempCell.transform.position = WorldController.instance.map[mapX, i].transform.position + Vector3.right * 2;

                                        foreach (MapObject mapObject in tempCell.mapObjectList)
                                        {
                                            mapObject.transform.position = tempCell.transform.position;

                                            if (mapObject.GetType() == typeof(Unit))
                                                mapObject.transform.position += heighFix;
                                        }
                                    }
                                    isCulled = true;
                                }

                                displayMap[x, y].gameObject.SetActive(true);

                                foreach (MapObject mapObject in displayMap[x, y].mapObjectList)
                                {
                                    mapObject.rendererCpn.enabled = true;
                                    mapObject.slider.transform.parent.gameObject.SetActive(true);
                                }
                            }

                            else
                            {
                                displayMap[x, y] = null;
                            }
                        }

                        else
                        {
                            displayMap[x, y] = displayMap[x + 1, y];
                        }
                    }
                }
                break;
        }

        if (culledCount <= -10 || culledCount >= 10)
        {
            MoveCamera(currentMiddleCell);
        }
    }
}
