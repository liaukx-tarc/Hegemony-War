using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Player
    public Player player;

    //Component
    public Renderer rendererCpn;
    public Collider colliderCpn;

    //Unit Propety Scriptable Object
    public UnitProperty property;
    public TagController.UnitFunction runtimeFunction;

    //Unit state
    public int currentHp;
    public int damage;
    public float remainMove;

    public bool isAction = false;
    public MapCell currentPos;

    //Moving
    public bool isMoving = false;
    public bool isAutoMove = false;
    public bool startMove = false;
    public MapCell targetPos;
    public int moveFrame;
    int moveCount = 0;
    MapCell currentPath;
    Vector3 moveDistance;

    //Building
    public GameObject buildingObj;
    public bool isBuilding = false;

    public void InitializeUnit()
    {
        currentHp = property.maxHp;
        damage = property.damage;
        remainMove = property.speed;
    }

    private void Update()
    {        
        if(startMove)
        {
            if (path.Count > 0)
            {
                if (remainMove >= path[0].cost)
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
                    WorldController.autoUnitArrive = false;
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
                currentPos.unitsList.Remove(this);
                currentPos = currentPath;
                currentPos.unitsList.Add(this);
                path.Remove(currentPath);
                showPath = false;

                if (currentPos == targetPos)
                {
                    if (isBuilding)
                    {
                        GameObject building = Instantiate(buildingObj,
                            new Vector3(targetPos.transform.position.x, targetPos.transform.position.y + 1.0f, targetPos.transform.position.z),
                            Quaternion.identity);
                        building.transform.parent = targetPos.transform;
                        building.name = "Building" + " " + ++player.buildNum;
                        targetPos.building = building.GetComponent<Building>();

                        isBuilding = false;
                    }

                    if(isAutoMove)
                    {
                        isAction = false;
                        isAutoMove = false;
                        WorldController.autoUnitArrive = true;
                        WorldController.activeUnitList.Add(this);
                    }

                    else
                    {
                        isAction = true;
                        WorldController.activeUnitList.Remove(this);
                    }

                    isMoving = false;
                }

                else if (path.Count > 0)
                {
                    if (remainMove >= path[0].cost)
                    {
                        currentPath = path[0];
                        remainMove -= currentPath.cost;
                        moveDistance = currentPath.transform.position - currentPos.transform.position;
                        this.transform.LookAt(new Vector3(currentPath.transform.position.x, this.transform.position.y, currentPath.transform.position.z));

                        moveCount = 0;
                    }

                    else
                    {
                        isAutoMove = true;
                        isAction = true;
                        WorldController.autoUnitArrive = false;
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
    public bool endSearch;
    public bool showPath;
    public int turnSeachCount;
    public int maxSeachCount;
    public List<MapCell> path = new List<MapCell>();

    public void CheckPath(MapCell targetPos)
    {
        this.targetPos = targetPos;
        StopAllCoroutines();
        path.Clear();
        StartCoroutine(AStar_PathFinding(targetPos));
    }

    IEnumerator AStar_PathFinding(MapCell targetPos)
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

        if(startNode.mapCell.connectGroupCoast == targetPos.connectGroupCoast)
        {
            do
            {
                if (seachcount >= maxSeachCount)
                {
                    isBuilding = false;
                    startMove = false;
                    endSearch = true;
                    yield break;
                }

                else if (seachcount % turnSeachCount == 0)
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
                        float cost = temp.gCost + currentNode.mapCell.cost; //add the traget mapcell's cost

                        while (temp.parent != null)
                        {
                            path.Add(temp.mapCell);
                            temp = temp.parent;
                        }

                        cost -= temp.mapCell.cost; //minus the start mapcell's cost

                        path.Add(temp.mapCell);
                        path.Reverse();

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
            Debug.Log("Can't Move");
            startMove = false;
            endSearch = true;
            yield break;
        }
    }
}
