using System.Collections.Generic;
using UnityEngine;

public class MapCreate : MonoBehaviour
{
    const string MapCellName = "MapCell";
    const string Multiply = "x";
    public List<GameObject> mapCellPrefabs;
    public int width, height;

    //Terrain Cost
    public float plainCost = 0;
    public float desertCost = 0;
    public float snowCost = 0;
    public float coastCost = 0;
    public float oceanCost = 0;
    public float forestCost = 0;
    public float marshCost = 0;

    public void GenerateWorld()
    {
        WorldController.map = new MapCell[width, height];
        WorldController.mapSize.x = width;
        WorldController.mapSize.y = height;
        int[,] mapTypeList = CellularAutomata_MapGenerate();

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                GameObject temp = Instantiate(mapCellPrefabs[mapTypeList[w, h]], new Vector3((w * 2f) - (h % 2f), 0.0f, h * -1.75f),
                    Quaternion.Euler(0, 0, 0), this.transform.parent.gameObject.transform);
                temp.name = MapCellName + w.ToString() + Multiply + h.ToString();

                MapCell tempMapCell = temp.GetComponent<MapCell>();
                tempMapCell.setPosition(new Vector2(w, h));
                tempMapCell.mapType = mapTypeList[w, h];

                //set cost
                switch (tempMapCell.mapType)
                {
                    case (int)MapTypeName.Plain:
                        tempMapCell.cost = plainCost;
                        break;
                    case (int)MapTypeName.Desert:
                        tempMapCell.cost = desertCost;
                        break;
                    case (int)MapTypeName.Snow:
                        tempMapCell.cost = snowCost;
                        break;
                    case (int)MapTypeName.Coast:
                        tempMapCell.cost = coastCost;
                        break;
                    case (int)MapTypeName.Ocean:
                        tempMapCell.cost = oceanCost;
                        break;
                    case (int)MapTypeName.Forest:
                        tempMapCell.cost = forestCost;
                        break;
                    case (int)MapTypeName.Marsh:
                        tempMapCell.cost = marshCost;
                        break;
                    default:
                        tempMapCell.cost = 0;
                        break;
                }

                WorldController.map[w, h] = temp.GetComponent<MapCell>();
            }
        }

        LinkNeighborCell();
        FloodFillCheck();
    }

    public int landChance;
    public int coastChance;
    public int snowChance;
    public int forestChance;
    public int desertChance;
    public int marshChance;

    public int repeatTimes;
    public int deathLimit;
    public int birthLimit;

    int[,] CellularAutomata_MapGenerate()
    {
        int count;
        float rate;
        Vector2[] neighborCell;
        int[,] map = new int[width, height];
        int[,] newMap = new int[width, height];

        //Generate Land
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (Random.Range(1, 100) < landChance)
                    map[w, h] = 1;

        //Land CellularAutomata
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 1)
                            count++;
                    }

                    if (map[w, h] == 1)
                    {
                        if (count < deathLimit)
                            newMap[w, h] = 0;
                        else
                            newMap[w, h] = 1;
                    }

                    else
                    {
                        if (count > birthLimit)
                            newMap[w, h] = 1;
                        else
                            newMap[w, h] = 0;
                    }
                }
            }

            map = newMap;
        }

        //-------------------------------------------------------------------------------------------------
        //Generate Coast
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (map[w, h] == 0)
                {
                    rate = coastChance;
                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 1)
                        {
                            rate *= 2;
                            break;
                        }
                    }

                    if (Random.Range(1, 100) < rate)
                        map[w, h] = 2;
                }

        //Coast CellularAutomata
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    rate = coastChance;
                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 1)
                        {
                            rate *= 2;
                            break;
                        }
                    }

                    if (Random.Range(1, 100) < rate)
                    {
                        foreach (Vector2 neighbour in neighborCell)
                        {
                            if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 2)
                                count++;
                        }

                        if (map[w, h] == 2)
                        {
                            if (count < deathLimit)
                                newMap[w, h] = 0;
                            else
                                newMap[w, h] = 2;
                        }

                        else if (map[w, h] == 0)
                        {
                            if (count > birthLimit)
                                newMap[w, h] = 2;
                            else
                                newMap[w, h] = 0;
                        }
                    }
                }
            }

            map = newMap;
        }

        //--------------------------------------------------------------------------------------------------
        //Generate Snow
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (map[w, h] == 1)
                {
                    rate = snowChance;

                    if (h < height / 10)
                        if (h != 0)
                            rate = rate / (h / rate);
                        else
                            rate *= 5;

                    else if (h > height * 0.9f)
                        if (h != height - 1)
                            rate = rate / ((height - h) / rate);
                        else
                            rate *= 5;

                    if (Random.Range(1, 100) < rate)
                        map[w, h] = 3;
                }

        //Snow CellularAutomata
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 3)
                            count++;
                    }

                    if (map[w, h] == 3)
                    {
                        if (count < deathLimit)
                            newMap[w, h] = 1;
                        else
                            newMap[w, h] = 3;
                    }

                    else if (map[w, h] == 1)
                    {
                        if (count > birthLimit)
                            newMap[w, h] = 3;
                        else
                            newMap[w, h] = 1;
                    }
                }
            }

            map = newMap;
        }

        //--------------------------------------------------------------------------------------------------
        //Generate Forest
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (map[w, h] == 1)
                    if (Random.Range(1, 100) < forestChance)
                        map[w, h] = 4;

        //Forest CellularAutomata
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 4)
                            count++;
                    }

                    if (map[w, h] == 4)
                    {
                        if (count < deathLimit)
                            newMap[w, h] = 1;
                        else
                            newMap[w, h] = 4;
                    }

                    else if(map[w,h] == 1)
                    {
                        if (count > birthLimit)
                            newMap[w, h] = 4;
                        else
                            newMap[w, h] = 1;
                    }
                }
            }

            map = newMap;
        }

        //--------------------------------------------------------------------------------------------------
        //Generate Desert
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (map[w, h] == 1)
                    if (Random.Range(1, 100) < desertChance)
                        map[w, h] = 5;

        //Desert CellularAutomata
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 5)
                            count++;
                    }

                    if (map[w, h] == 5)
                    {
                        if (count < deathLimit)
                            newMap[w, h] = 1;
                        else
                            newMap[w, h] = 5;
                    }

                    else if (map[w, h] == 1)
                    {
                        if (count > birthLimit)
                            newMap[w, h] = 5;
                        else
                            newMap[w, h] = 1;
                    }
                }
            }

            map = newMap;
        }

        //--------------------------------------------------------------------------------------------------
        //Generate Marsh
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (map[w, h] == 1)
                    if (Random.Range(1, 100) < marshChance)
                        map[w, h] = 6;

        //Generate Marsh
        for (int r = 0; r < repeatTimes; r++)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    count = 0;

                    neighborCell = GetNeighborCell(new Vector2(w, h));

                    foreach (Vector2 neighbour in neighborCell)
                    {
                        if (neighbour.x > -1 && map[(int)neighbour.x, (int)neighbour.y] == 6)
                            count++;
                    }

                    if (map[w, h] == 6)
                    {
                        if (count < deathLimit)
                            newMap[w, h] = 1;
                        else
                            newMap[w, h] = 6;
                    }

                    else if (map[w, h] == 1)
                    {
                        if (count > birthLimit)
                            newMap[w, h] = 6;
                        else
                            newMap[w, h] = 1;
                    }
                }
            }

            map = newMap;
        }

        //--------------------------------------------------------------------------------------------------

        return map;
    }

    public void LinkNeighborCell()
    {
        foreach (MapCell mapcell in WorldController.map)
        {
            mapcell.neighborCell = new MapCell[6];

            for (int n = 0; n < 6; n++)
            {
                Vector2 neighbourPos = CubeNeighbor(new CubePosition(mapcell.position), n).CubeToOffset();

                if (neighbourPos.y >= 0 && neighbourPos.y < WorldController.map.GetLength(1))
                {
                    //connect left and right map
                    if (neighbourPos.x == -1)
                    {
                        mapcell.neighborCell[n] = WorldController.map[WorldController.map.GetLength(0) - 1, (int)neighbourPos.y];
                    }

                    else if (neighbourPos.x == WorldController.map.GetLength(0))
                    {
                        mapcell.neighborCell[n] = WorldController.map[0, (int)neighbourPos.y];
                    }

                    else
                    {
                        mapcell.neighborCell[n] = WorldController.map[(int)neighbourPos.x, (int)neighbourPos.y];
                    }
                }
            }
        }
    }

    //Use Flood Fill algorthm to check the connect of the map
    void FloodFillCheck()
    {
        //check without sea connection
        int connectionNum = 0;
        bool[,] checkedMapList = new bool[width, height];

        //Ignore the cell can't pass
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (WorldController.map[w, h].cost == 0 || WorldController.map[w,h].mapType == (int)MapTypeName.Ocean || WorldController.map[w, h].mapType == (int)MapTypeName.Coast)
                    checkedMapList[w, h] = true;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if (!checkedMapList[w, h])
                {
                    connectionNum++;
                    Queue<Vector2> queue = new Queue<Vector2>();

                    //add the cell's postion to the queue
                    queue.Enqueue(new Vector2(w, h));
                    WorldController.map[w, h].connectGroup = connectionNum;

                    do//do when the queue is not empty
                    {
                        Vector2 currentCell = queue.Dequeue();
                        int curW = (int)currentCell.x;
                        int curH = (int)currentCell.y;

                        foreach (MapCell cell in WorldController.map[curW, curH].neighborCell)
                        {
                            if (cell != null && !checkedMapList[(int)cell.position.x, (int)cell.position.y] &&
                                cell.cost != 0 && cell.mapType != (int)MapTypeName.Ocean && cell.mapType != (int)MapTypeName.Coast)
                            {
                                cell.connectGroup = connectionNum;
                                checkedMapList[(int)cell.position.x, (int)cell.position.y] = true;
                                queue.Enqueue(new Vector2((int)cell.position.x, (int)cell.position.y));
                            }
                        }
                    } while (queue.Count > 0);
                }
            }
        }

        //check with coast
        int coastConnectionNum = 0;
        bool[,] coastCheckedMapList = new bool[width, height];

        //Ignore the cell can't pass
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (WorldController.map[w, h].cost == 0 || WorldController.map[w,h].mapType == (int)MapTypeName.Ocean)
                    coastCheckedMapList[w, h] = true;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if (!coastCheckedMapList[w, h])
                {
                    coastConnectionNum++;
                    Queue<Vector2> queue = new Queue<Vector2>();

                    //add the cell's postion to the queue
                    queue.Enqueue(new Vector2(w, h));
                    WorldController.map[w, h].connectGroup = coastConnectionNum;

                    do//do when the queue is not empty
                    {
                        Vector2 currentCell = queue.Dequeue();
                        int curW = (int)currentCell.x;
                        int curH = (int)currentCell.y;

                        foreach (MapCell cell in WorldController.map[curW, curH].neighborCell)
                        {
                            if (cell != null && !coastCheckedMapList[(int)cell.position.x, (int)cell.position.y] &&
                                cell.cost != 0 && cell.mapType != (int)MapTypeName.Ocean)
                            {
                                cell.connectGroupCoast = coastConnectionNum;
                                coastCheckedMapList[(int)cell.position.x, (int)cell.position.y] = true;
                                queue.Enqueue(new Vector2((int)cell.position.x, (int)cell.position.y));
                            }
                        }

                    } while (queue.Count > 0);
                }
            }
        }

        //check with sea
        int seaConnectionNum = 0;
        bool[,] seaCheckedMapList = new bool[width, height];

        //Ignore the cell can't pass
        for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
                if (WorldController.map[w, h].cost == 0)
                    seaCheckedMapList[w, h] = true;

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                if (!seaCheckedMapList[w, h])
                {
                    seaConnectionNum++;
                    Queue<Vector2> queue = new Queue<Vector2>();

                    //add the cell's postion to the queue
                    queue.Enqueue(new Vector2(w, h));
                    WorldController.map[w, h].connectGroup = seaConnectionNum;

                    do//do when the queue is not empty
                    {
                        Vector2 currentCell = queue.Dequeue();
                        int curW = (int)currentCell.x;
                        int curH = (int)currentCell.y;

                        foreach (MapCell cell in WorldController.map[curW, curH].neighborCell)
                        {
                            if (cell != null && !seaCheckedMapList[(int)cell.position.x, (int)cell.position.y] &&
                                cell.cost != 0 && cell.mapType != (int)MapTypeName.Ocean)
                            {
                                cell.connectGroupSea = seaConnectionNum;
                                seaCheckedMapList[(int)cell.position.x, (int)cell.position.y] = true;
                                queue.Enqueue(new Vector2((int)cell.position.x, (int)cell.position.y));
                            }
                        }

                    } while (queue.Count > 0);
                }
            }
        }
    }

    public Vector2[] GetNeighborCell(Vector2 cellPos)
    {
        Vector2[] neighborCell = new Vector2[6];

        for (int n = 0; n < 6; n++)
        {
            Vector2 neighbourPos = CubeNeighbor(new CubePosition(cellPos), n).CubeToOffset();

            if (neighbourPos.y >= 0 && neighbourPos.y < height)
            {
                //connect left and right map
                if (neighbourPos.x == -1)
                {
                    neighborCell[n] = new Vector2(width - 1, neighbourPos.y);
                }

                else if (neighbourPos.x == width)
                {
                    neighborCell[n] = new Vector2(0, neighbourPos.y);
                }

                else
                {
                    neighborCell[n] = new Vector2(neighbourPos.x, neighbourPos.y);
                }
            }

            else
                neighborCell[n] = new Vector2(-1, -1);
        }

        return neighborCell;
    }

    CubePosition[] cubeDirectionVectors =
        {
            new CubePosition(0, -1, 1), new CubePosition(1, -1, 0),
            new CubePosition(1, 0, -1), new CubePosition(0, 1, -1),
            new CubePosition(-1, 1, 0), new CubePosition(-1, 0, 1)
        };

    public CubePosition CubeNeighbor(CubePosition cubePosition, int direction)
    {
        return new CubePosition
            (
                cubePosition.q + cubeDirectionVectors[direction].q,
                cubePosition.r + cubeDirectionVectors[direction].r,
                cubePosition.s + cubeDirectionVectors[direction].s
            );
    }
}


