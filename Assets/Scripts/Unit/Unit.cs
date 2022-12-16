using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject
{
    //Player
    public Player player;

    //Unit Propety Scriptable Object
    public UnitProperty property;
    public TagController.UnitFunction runtimeFunction;

    //Unit state
    [Header("State")]
    public int currentHp;
    public int damage;
    public float remainMove;

    public bool isAction = false;
    public bool isSleep = false;
    public MapCell currentPos;

    //Moving
    [Header("Moving")]
    public bool isMoving = false;
    public bool isAutoMove = false;
    public bool startMove = false;
    public bool isNoCost = false;
    public MapCell targetPos;
    public int moveFrame;
    int moveCount = 0;
    MapCell currentPath;
    Vector3 moveDistance;

    //Building
    [Header("Building")]
    public bool isBuilding = false;
    public GameObject cityModel;

    public void InitializeUnit()
    {
        currentHp = property.maxHp;
        damage = property.damage;
        remainMove = property.speed;

        switch (property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                AStar_PathFinding = Ground_AStar_PathFinding;
                break;

            case TransportType.Aircarft:
                AStar_PathFinding = NotCost_AStar_PathFinding;
                isNoCost = true;
                break;

            case TransportType.Ship:
                AStar_PathFinding = Sea_AStar_PathFinding;
                break;
        }
    }

    private void Update()
    {        
        if(startMove)
        {
            if (path.Count > 0)
            {
                float cost;
                
                if (isNoCost)
                    cost = 1;
                else
                    cost = path[0].cost;

                if (remainMove >= cost)
                {
                    currentPath = path[0];
                    moveDistance = currentPath.transform.position - currentPos.transform.position;
                    moveCount = 0;
                    isMoving = true;
                    isAction = false;
                    startMove = false;
                }

                else
                {
                    isAction = true;
                    isAutoMove = true;
                    WorldController.activeUnitList.Remove(this);
                }

            }

            else
            {
                isAction = true;
            }
        }

        if (isMoving)
        {
            if (moveCount == moveFrame)
            {
                switch (property.transportProperty.transportType)
                {
                    case TransportType.Vechicle:
                        currentPos.groundUnit = null;
                        currentPath.groundUnit = this;
                        break;

                    case TransportType.Aircarft:
                        currentPos.airForceUnit = null;
                        currentPath.airForceUnit = this;
                        break;

                    case TransportType.Ship:
                        currentPos.navalUnit = null;
                        currentPath.navalUnit = this;
                        break;
                }

                currentPos.mapObjectList.Remove(this);
                currentPos = currentPath;
                currentPos.mapObjectList.Add(this);

                path.Remove(currentPath);
                showPath = false;

                if (currentPos == targetPos)
                {
                    if(isAutoMove)
                    {
                        isAction = false;
                        isAutoMove = false;
                        WorldController.activeUnitList.Add(this);
                        WorldController.movingUnitList.Remove(this);
                        WorldController.arriveUnit = this;

                        if (!isBuilding)
                            WorldController.autoUnitArrive = true;
                    }

                    else
                    {
                        isAction = true;
                        WorldController.activeUnitList.Remove(this);
                    }

                    if (isBuilding)
                    {
                        WorldController.buildingController.BuildCity(player, cityModel, currentPos);
                        cityModel = null;
                        UnitDestroy();
                        isBuilding = false;
                    }

                    isMoving = false;
                }

                else if (path.Count > 0)
                {
                    float cost;

                    if (isNoCost)
                        cost = 1;
                    else
                        cost = path[0].cost;

                    if (remainMove >= cost)
                    {
                        currentPath = path[0];

                        if (isNoCost)
                            remainMove -= 1;
                        else
                            remainMove -= currentPath.cost;

                        moveDistance = currentPath.transform.position - currentPos.transform.position;
                        this.transform.LookAt(new Vector3(currentPath.transform.position.x, this.transform.position.y, currentPath.transform.position.z));

                        moveCount = 0;
                    }

                    else
                    {
                        isAutoMove = true;
                        isAction = true;
                        WorldController.activeUnitList.Remove(this);
                        isMoving = false;
                    }
                }
            }

            else
            {
                this.transform.position += moveDistance / moveFrame;
                moveCount++;
            }
        }
    }

    //pathFinding
    [Header("Path Finding")]
    public bool endSearch;
    public bool showPath;
    public int turnSeachCount;
    public List<MapCell> path = new List<MapCell>();
    public List<MapCell> eachTurnStartCell = new List<MapCell>();
    public delegate IEnumerator AStar(MapCell targetCell);
    public AStar AStar_PathFinding;

    public void CheckPath(MapCell targetPos)
    {
        this.targetPos = targetPos;
        StopAllCoroutines();
        path.Clear();
        StartCoroutine(AStar_PathFinding(targetPos));
    }

    IEnumerator Ground_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;
        showPath = false;

        Node startNode = new Node(currentPos);
        startNode.gCost = 0;
        startNode.hCost = startNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
        startNode.fCost = startNode.gCost + startNode.hCost;

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        openList.Add(startNode);

        Node currentNode;
        Node neighbourNode;
        bool haveSame;
        bool inClose;
        int seachcount = 0;

        if (startNode.mapCell.groundConnect == targetPos.groundConnect)
        {
            do
            {
                if (seachcount % turnSeachCount == 0)
                {
                    yield return null;
                }

                else
                {
                    currentNode = openList[0];
                    for (int i = 0; i < openList.Count; i++)
                    {
                        if (openList[i].fCost < currentNode.fCost ||
                            (openList[i].fCost == currentNode.fCost && openList[i].gCost > currentNode.gCost))
                        {
                            currentNode = openList[i];
                        }

                    }

                    openList.Remove(currentNode);
                    closeList.Add(currentNode);

                    if (currentNode.mapCell == targetPos)
                    {
                        Node temp = currentNode;

                        while (temp.parent != null)
                        {
                            path.Add(temp.mapCell);
                            temp = temp.parent;
                        }

                        path.Add(temp.mapCell);
                        path.Reverse();

                        //Calculate End Cell of each turn
                        eachTurnStartCell.Clear();

                        float tempRemainMove = remainMove;

                        foreach (MapCell cell in path)
                        {
                            if (cell == currentPos)
                                continue;

                            tempRemainMove -= cell.cost;
                            if (tempRemainMove < 0)
                            {
                                eachTurnStartCell.Add(cell);
                                tempRemainMove = property.speed;
                            }
                        }

                        endSearch = true;
                        yield break;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        haveSame = false;
                        inClose = false;

                        if (currentNode.mapCell.neighborCell[i] != null)
                        {
                            neighbourNode = new Node(currentNode.mapCell.neighborCell[i]);

                            if (neighbourNode.mapCell.mapType != (int)MapTypeName.Ocean)
                            {
                                foreach (Node node in closeList)
                                {
                                    if (node.mapCell == neighbourNode.mapCell)
                                    {
                                        inClose = true;
                                        break;
                                    }
                                }

                                if (!inClose)
                                {
                                    neighbourNode.gCost = currentNode.gCost + currentNode.mapCell.cost * 2;
                                    neighbourNode.hCost = neighbourNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
                                    neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;
                                    neighbourNode.parent = currentNode;

                                    foreach (Node node in openList)
                                    {
                                        if (node.mapCell == neighbourNode.mapCell)
                                        {
                                            if (node.fCost > neighbourNode.fCost)
                                            {
                                                openList.Remove(node);
                                                openList.Add(neighbourNode);
                                            }
                                            haveSame = true;
                                            break;
                                        }
                                    }

                                    if (!haveSame)
                                    {
                                        openList.Add(neighbourNode);
                                    }
                                }
                            }
                        }
                    }
                }
                seachcount++;
            } while (openList.Count > 0);
        }

        else
        {
            isBuilding = false;
            startMove = false;
            endSearch = true;
            yield break;
        }
    }

    IEnumerator Sea_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;
        showPath = false;

        Node startNode = new Node(currentPos);
        startNode.gCost = 0;
        startNode.hCost = startNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
        startNode.fCost = startNode.gCost + startNode.hCost;

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        openList.Add(startNode);

        Node currentNode;
        Node neighbourNode;
        bool haveSame;
        bool inClose;
        int seachcount = 0;

        if (startNode.mapCell.seaConnect == targetPos.seaConnect)
        {
            do
            {
                if (seachcount % turnSeachCount == 0)
                {
                    yield return null;
                }

                else
                {
                    currentNode = openList[0];
                    for (int i = 0; i < openList.Count; i++)
                    {
                        if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].gCost > currentNode.gCost))
                        {
                            currentNode = openList[i];
                        }

                    }

                    openList.Remove(currentNode);
                    closeList.Add(currentNode);

                    if (currentNode.mapCell == targetPos)
                    {
                        Node temp = currentNode;

                        while (temp.parent != null)
                        {
                            path.Add(temp.mapCell);
                            temp = temp.parent;
                        }

                        path.Add(temp.mapCell);
                        path.Reverse();

                        //Calculate End Cell of each turn
                        eachTurnStartCell.Clear();

                        float tempRemainMove = remainMove;

                        foreach (MapCell cell in path)
                        {
                            if (cell == currentPos)
                                continue;

                            tempRemainMove -= cell.cost;
                            if (tempRemainMove < 0)
                            {
                                eachTurnStartCell.Add(cell);
                                tempRemainMove = property.speed;
                            }
                        }

                        endSearch = true;
                        yield break;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        haveSame = false;
                        inClose = false;

                        if (currentNode.mapCell.neighborCell[i] != null)
                        {
                            neighbourNode = new Node(currentNode.mapCell.neighborCell[i]);
                            if (neighbourNode.mapCell.mapType == (int)MapTypeName.Ocean || neighbourNode.mapCell.mapType == (int)MapTypeName.Coast)
                            {
                                foreach (Node node in closeList)
                                {
                                    if (node.mapCell == neighbourNode.mapCell)
                                    {
                                        inClose = true;
                                        break;
                                    }
                                }

                                if (!inClose)
                                {
                                    neighbourNode.gCost = currentNode.gCost + currentNode.mapCell.cost * 2;
                                    neighbourNode.hCost = neighbourNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
                                    neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;
                                    neighbourNode.parent = currentNode;

                                    foreach (Node node in openList)
                                    {
                                        if (node.mapCell == neighbourNode.mapCell)
                                        {
                                            if (node.fCost > neighbourNode.fCost)
                                            {
                                                openList.Remove(node);
                                                openList.Add(neighbourNode);
                                            }
                                            haveSame = true;
                                            break;
                                        }
                                    }

                                    if (!haveSame)
                                    {
                                        openList.Add(neighbourNode);
                                    }
                                }
                            }
                        }
                    }
                }
                seachcount++;
            } while (openList.Count > 0);
        }

        else
        {
            isBuilding = false;
            startMove = false;
            endSearch = true;
            yield break;
        }
    }

    IEnumerator NotCost_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;
        showPath = false;

        Node startNode = new Node(currentPos);
        startNode.gCost = 0;
        startNode.hCost = startNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
        startNode.fCost = startNode.gCost + startNode.hCost;

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        openList.Add(startNode);

        Node currentNode;
        Node neighbourNode;
        bool haveSame;
        bool inClose;
        int seachcount = 0;

        do
        {
            if (seachcount % turnSeachCount == 0)
            {
                yield return null;
            }

            else
            {
                currentNode = openList[0];
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost || 
                        (openList[i].fCost == currentNode.fCost && openList[i].gCost > currentNode.gCost))
                    {
                        currentNode = openList[i];
                    }

                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);

                if (currentNode.mapCell == targetPos)
                {
                    Node temp = currentNode;

                    while (temp.parent != null)
                    {
                        path.Add(temp.mapCell);
                        temp = temp.parent;
                    }

                    path.Add(temp.mapCell);
                    path.Reverse();

                    //Calculate End Cell of each turn
                    eachTurnStartCell.Clear();

                    float tempRemainMove = remainMove;

                    foreach (MapCell cell in path)
                    {
                        if (cell == currentPos)
                            continue;

                        tempRemainMove -= 1;//each move is 1 cost
                        if (tempRemainMove < 0)
                        {
                            eachTurnStartCell.Add(cell);
                            tempRemainMove = property.speed;
                        }
                    }

                    endSearch = true;
                    yield break;
                }

                for (int i = 0; i < 6; i++)
                {
                    haveSame = false;
                    inClose = false;

                    if (currentNode.mapCell.neighborCell[i] != null)
                    {
                        neighbourNode = new Node(currentNode.mapCell.neighborCell[i]);

                        if (neighbourNode.mapCell.cost != 0)//cant move
                        {
                            foreach (Node node in closeList)
                            {
                                if (node.mapCell == neighbourNode.mapCell)
                                {
                                    inClose = true;
                                    break;
                                }
                            }

                            if (!inClose)
                            {
                                neighbourNode.gCost = currentNode.gCost + 2;
                                neighbourNode.hCost = neighbourNode.mapCell.cubePosition.CheckDistance(targetPos.cubePosition);
                                neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;
                                neighbourNode.parent = currentNode;

                                foreach (Node node in openList)
                                {
                                    if (node.mapCell == neighbourNode.mapCell)
                                    {
                                        if (node.fCost > neighbourNode.fCost)
                                        {
                                            openList.Remove(node);
                                            openList.Add(neighbourNode);
                                        }
                                        haveSame = true;
                                        break;
                                    }
                                }

                                if (!haveSame)
                                {
                                    openList.Add(neighbourNode);
                                }
                            }
                        }
                    }
                }
            }
            seachcount++;
        } while (openList.Count > 0);

    }

    public void UnitDestroy()
    {
        player.unitList.Remove(this);
        WorldController.activeUnitList.Remove(this);
        WorldController.movingUnitList.Remove(this);

        switch (property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                currentPos.groundUnit = null;
                break;

            case TransportType.Aircarft:
                currentPos.airForceUnit = null;
                break;

            case TransportType.Ship:
                currentPos.navalUnit = null;
                break;
        }

        currentPos.mapObjectList.Remove(this);

        if(PlayerController.selectedUnit == this)
        {
            WorldController.playerController.CancelUnitSelect();
            WorldController.UI.CloseUnitUI();
        }

        if(cityModel != null)
            Destroy(cityModel);

        Destroy(this.gameObject);
    }
}
