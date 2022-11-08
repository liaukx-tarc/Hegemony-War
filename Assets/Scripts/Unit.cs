using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isAction = false;
    public MapCell currentPos;

    //pathFinding
    public int movement = 6;
    public float remainMove;
    public int turnSeachCount;
    public int maxSeachCount;
    List<MapCell> path = new List<MapCell>();

    //Moving
    public bool isMoving = false;
    public bool startMove = false;
    public MapCell targetPos;
    public int moveFrame;
    int moveCount = 0;
    MapCell currentPath;
    Vector3 moveDistance;

    //Building
    public GameObject buildingObj;
    public bool isBuilding = false;

    private void Start()
    {
        remainMove = movement;
    }

    private void FixedUpdate()
    {
        
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
                    currentPath.GetComponent<Renderer>().material.color = Color.red;

                    moveDistance = currentPath.transform.position - currentPos.transform.position;
                    moveCount = 0;
                    isMoving = true;
                    isAction = false;

                    startMove = false;
                }

                else
                    isAction = true;
            }      
        }

        if (isMoving)
        {
            if (moveCount == moveFrame)
            {
                if (!isAction)
                {
                    currentPos.GetComponent<Renderer>().material.color = currentPos.color;
                    currentPos.units.Remove(this);
                    currentPos = currentPath;
                    currentPos.GetComponent<Renderer>().material.color = Color.red;
                    currentPos.units.Add(this);
                    path.Remove(currentPath);

                    if (currentPos == targetPos)
                    {
                        if (isBuilding)
                        {
                            GameObject building = Instantiate(buildingObj,
                                new Vector3(targetPos.transform.position.x, targetPos.transform.position.y + 1.0f, targetPos.transform.position.z),
                                Quaternion.identity);
                            building.transform.parent = targetPos.transform;
                            building.name = "Building" + " " + ++PlayerController.buildNum;
                            targetPos.building = building;

                            isBuilding = false;
                        }

                        isMoving = false;
                        currentPos.GetComponent<Renderer>().material.color = currentPos.color;
                    }

                    else if (path.Count > 0)
                    {
                        if (remainMove >= path[0].cost)
                        {
                            currentPath.GetComponent<Renderer>().material.color = currentPath.color;
                            currentPath = path[0];
                            remainMove -= currentPath.cost;
                            moveDistance = currentPath.transform.position - currentPos.transform.position;
                            this.transform.LookAt(new Vector3(currentPath.transform.position.x, this.transform.position.y, currentPath.transform.position.z));

                            moveCount = 0;
                        }

                        else
                        {
                            foreach (MapCell cell in path)
                            {
                                cell.GetComponent<Renderer>().material.color = cell.color;
                            }

                            isAction = true;
                        }
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

    public void CheckPath(MapCell targetPos)
    {
        StopAllCoroutines();
        path.Clear();
        StartCoroutine(AStar_PathFinding(targetPos));
    }

    IEnumerator AStar_PathFinding(MapCell targetPos)
    {
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

        //Test
        List<MapCell> clearList = new List<MapCell>();
        Debug.Log(startNode.mapCell.connectGroupCoast + " + " + targetPos.connectGroupCoast);
        if(startNode.mapCell.connectGroupCoast == targetPos.connectGroupCoast)
        {
            do
            {
                if (seachcount >= maxSeachCount)
                {
                    Debug.Log("Too Far Can't Reach");
                    isBuilding = false;
                    startMove = false;
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
                            foreach (MapCell cell in clearList)
                            {
                                cell.GetComponent<Renderer>().material.color = cell.color;
                            }
                            currentNode.mapCell.GetComponent<Renderer>().material.color = new Color(1, 1, 0);
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

                        foreach (MapCell cell in path)
                        {
                            cell.GetComponent<Renderer>().material.color = Color.grey;
                        }

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
                                                neighbourNode.mapCell.GetComponent<Renderer>().material.color = Color.blue;
                                                clearList.Add(neighbourNode.mapCell);
                                            }
                                            haveSame = true;
                                            break;
                                        }
                                    }

                                    if (!haveSame)
                                    {
                                        openList.Add(neighbourNode);
                                        neighbourNode.mapCell.GetComponent<Renderer>().material.color = Color.blue;
                                        clearList.Add(neighbourNode.mapCell);
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
        }
    }


}
