using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject
{
    [Header("Unit Data")]
    //Player
    public Player player;

    //Unit Propety Scriptable Object
    public UnitProperty property;
    public TagController.UnitFunction runtimeFunction;

    //Unit state
    [Header("State")]
    public int currentHp;
    public int hpRestore;
    public int damage;
    public int range;
    public float remainMove;

    public bool isAction = false;
    public bool isSleep = false;
    public bool isAttack = false;
    public bool completeAttack = false;
    public MapCell currentPos;

    //Moving
    [Header("Moving")]
    public bool isMoving = false;
    public bool isAutoMove = false;
    public bool startMove = false;
    public bool isNoCost = false;
    public MapCell targetPos;
    public MapCell selectingTarget;
    public int moveFrame;
    int moveCount = 0;
    MapCell currentPath;
    Vector3 moveDistance;

    //Building
    [Header("Building")]
    public bool isBuilding = false;
    public GameObject cityModel;

    //Attack
    [Header("Attack")]
    public MapObject attackTarget;

    public void InitializeUnit()
    {
        icon.sprite = property.unitIcon;
        switch (property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                iconBackground.color = WorldController.instance.uiController.vechicleColor;
                break;

            case TransportType.Aircarft:
                iconBackground.color = WorldController.instance.uiController.aircarftColor;
                break;

            case TransportType.Ship:
                iconBackground.color = WorldController.instance.uiController.shipColor;
                break;
        }

        slider.value = slider.maxValue = property.maxHp;
        currentHp = property.maxHp;
        hpRestore = WorldController.instance.unitController.hpRestore;
        damage = property.damage;
        remainMove = property.speed;
        range = property.range;

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

    public void TurnStart()
    {
        remainMove = property.speed;
        isAction = false;
        completeAttack = false;

        if(currentHp < property.maxHp)
        {
            currentHp += hpRestore;
            currentHp = Mathf.Min(currentHp, property.maxHp);
            slider.value = currentHp;

            if (currentHp == property.maxHp)
            {
                slider.gameObject.SetActive(false);
            }
        }

        if(isAttack)
        {
            if (attackTarget == null)
            {
                isAttack = false;
                isAutoMove = false;
                path.Clear();
            }
        }

        else if(isBuilding)
        {
            if(targetPos.unit != null || targetPos.building != null)
            {
                isAutoMove = false;
                path.Clear();

                Destroy(cityModel);
                isBuilding = false;
            }
           
            else
            {
                List<MapCell> cellsInBuildingRange = targetPos.CheckCellInRange(3);

                foreach (MapCell cell in cellsInBuildingRange)
                {
                    if (cell.belongCity != null)
                    {
                        isAutoMove = false;
                        path.Clear();

                        Destroy(cityModel);
                        isBuilding = false;
                        break;
                    }
                }
            }
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
                    startMove = false;
                    WorldController.instance.activeUnitList.Remove(this);
                }

            }
        }

        if (isMoving)
        {
            if (moveCount == moveFrame)
            {
                currentPos.mapObjectList.Remove(this);
                currentPos.unit = null;

                currentPos = currentPath;

                currentPos.mapObjectList.Add(this);
                currentPos.unit = this;

                path.Remove(currentPath);
                showPath = false;

                if (currentPos == targetPos)
                {
                    if(isAutoMove)
                    {
                        isAutoMove = false;
                        
                        if (!isAttack)
                        {
                            isAction = false;
                            WorldController.instance.activeUnitList.Add(this);
                            WorldController.instance.isAutoUnitArrive = true;
                        }

                        else
                        {
                            isAction = true;
                        }
                           

                        WorldController.instance.movingUnitList.Remove(this);
                        WorldController.instance.isAutoUnitMoving = false;
                    }

                    else
                    {
                        isAction = true;
                        WorldController.instance.activeUnitList.Remove(this);
                    }

                    //Capture city
                    if (currentPos.building != null && currentPos.building.GetType() == typeof(City) &&
                        currentPos.building.isDestroy)
                    {
                        City city = (City)currentPos.building;

                        city.isDestroy = false;

                        if (city.player != player)
                            city.CaptureCity(player);
                    }

                    if (isBuilding)
                    {
                        bool canBuild = true;

                        List<MapCell> cellsInBuildingRange = targetPos.CheckCellInRange(3);

                        foreach (MapCell cell in cellsInBuildingRange)
                        {
                            if (cell.belongCity != null)
                            {
                                Destroy(cityModel);
                                canBuild = false;
                                break;
                            }
                        }

                        if(canBuild)
                        {
                            WorldController.instance.buildingController.BuildCity(player, cityModel, currentPos);
                            cityModel = null;
                            UnitDestroy();
                        }
                        
                        isBuilding = false;
                    }

                    if (isAttack)
                    {
                        if (attackTarget.GetType() == typeof(Unit))
                        {
                            Unit unit = (Unit)attackTarget;
                            if (currentPos.cubePosition.CheckDistance(unit.currentPos.cubePosition) <= range)
                            {
                                AttackUnit(unit);
                            }
                        }

                        else if (attackTarget.GetType() == typeof(City) || attackTarget.GetType() == typeof(Area))
                        {
                            Building building = (Building)attackTarget;
                            if (currentPos.cubePosition.CheckDistance(building.belongCell.cubePosition) <= range)
                            {
                                AttackBuilding(building);
                            }
                        }

                        completeAttack = true;

                    }

                    isAttack = false;
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
                        rendererCpn.gameObject.transform.LookAt(new Vector3(currentPath.transform.position.x, this.transform.position.y, currentPath.transform.position.z));

                        moveCount = 0;
                    }

                    else
                    {
                        isAutoMove = true;
                        isAction = true;
                        WorldController.instance.activeUnitList.Remove(this);
                        WorldController.instance.movingUnitList.Remove(this);
                        WorldController.instance.isAutoUnitMoving = false;
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
        StartCoroutine(AStar_PathFinding(targetPos));
    }

    IEnumerator Ground_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;

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

        if (isAttack || startNode.mapCell.groundConnect == targetPos.groundConnect)
        {
            List<MapCell> listOfTarget = targetPos.CheckCellInRange(range);

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

                    if(isAttack)
                    {
                        if(listOfTarget.Contains(currentNode.mapCell))
                        {
                            this.targetPos = targetPos = currentNode.mapCell;

                        }
                    }

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

                            if (neighbourNode.mapCell == targetPos || 
                                (neighbourNode.mapCell.mapType != (int)MapTypeName.Ocean && neighbourNode.mapCell.unit == null &&
                                (neighbourNode.mapCell.building == null || neighbourNode.mapCell.building.player == player || neighbourNode.mapCell.building.isDestroy)))
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

            isBuilding = false;
            isAttack = false;
            startMove = false;
            endSearch = true;
            attackTarget = null;
        }

        else
        {
            isBuilding = false;
            isAttack = false;
            startMove = false;
            endSearch = true;
            attackTarget = null;
            yield break;
        }
    }

    IEnumerator Sea_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;

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

        if (isAttack || startNode.mapCell.seaConnect == targetPos.seaConnect)
        {
            List<MapCell> listOfTarget = targetPos.CheckCellInRange(range);

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

                    if (isAttack)
                    {
                        if (listOfTarget.Contains(currentNode.mapCell))
                        {
                            this.targetPos = targetPos = currentNode.mapCell;

                        }
                    }

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

                            if (neighbourNode.mapCell == targetPos || 
                                ((neighbourNode.mapCell.mapType == (int)MapTypeName.Ocean || neighbourNode.mapCell.mapType == (int)MapTypeName.Coast) && 
                                neighbourNode.mapCell.unit == null && (neighbourNode.mapCell.building == null 
                                || neighbourNode.mapCell.building.player == player || neighbourNode.mapCell.building.isDestroy)))
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

            isBuilding = false;
            isAttack = false;
            startMove = false;
            endSearch = true;
            attackTarget = null;
        }

        else
        {
            isBuilding = false;
            isAttack = false;
            startMove = false;
            endSearch = true;
            attackTarget = null;
            yield break;
        }
    }

    IEnumerator NotCost_AStar_PathFinding(MapCell targetPos)
    {
        endSearch = false;

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

        List<MapCell> listOfTarget = targetPos.CheckCellInRange(range);

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

                if (isAttack)
                {
                    if (listOfTarget.Contains(currentNode.mapCell))
                    {
                        this.targetPos = targetPos = currentNode.mapCell;

                    }
                }

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

                        if (neighbourNode.mapCell == targetPos || (neighbourNode.mapCell.unit == null && 
                            (neighbourNode.mapCell.building == null || neighbourNode.mapCell.building.player == player || neighbourNode.mapCell.building.isDestroy)))
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

        isBuilding = false;
        isAttack = false;
        startMove = false;
        endSearch = true;
        attackTarget = null;
    }

    public bool CheckAttackUnit(Unit unit)
    {
        if(range <= 0)
        {
            startMove = false;
            endSearch = true;
            return false;
        }

        if(property.transportProperty.transportType == TransportType.Vechicle)
        {
            switch (unit.property.transportProperty.transportType)
            {
                case TransportType.Aircarft:
                    startMove = false;
                    endSearch = true;
                    return false;
            }
        }

        if(currentPos.cubePosition.CheckDistance(unit.currentPos.cubePosition) <= range)
        {
            AttackUnit(unit);
            
            WorldController.instance.activeUnitList.Remove(this);
            completeAttack = true;
            isAction = true;
            return true;
        }

        else
        {
            isAttack = true;
            CheckPath(unit.currentPos);
            attackTarget = unit;
            return true;
        }  
    }

    public void AttackUnit(Unit unit)
    {
        unit.currentHp -= Mathf.Max(damage - unit.property.armor, 0);
        unit.slider.value = unit.currentHp;
        unit.slider.gameObject.SetActive(true);

        if (unit.currentHp <= 0)
        {
            unit.UnitDestroy();
        }

        if (unit.damage != 0 && unit.currentPos.cubePosition.CheckDistance(currentPos.cubePosition) <= unit.range)
        {
            currentHp -= Mathf.Max(unit.damage - property.armor, 0);
            slider.value = currentHp;
            slider.gameObject.SetActive(true);

            if (currentHp <= 0)
            {
                UnitDestroy();
            }
        }

        if (isAutoMove)
            WorldController.instance.movingUnitList.Remove(this);
    }

    public bool CheckAttackBuilding(Building building)
    {
        if (range <= 0)
        {
            startMove = false;
            endSearch = true;
            return false;
        }

        if (currentPos.cubePosition.CheckDistance(building.belongCell.cubePosition) <= range)
        {
            AttackBuilding(building);

            WorldController.instance.activeUnitList.Remove(this);
            completeAttack = true;
            isAction = true;
            return true;
        }
        else
        {
            isAttack = true;
            CheckPath(building.belongCell);
            attackTarget = building;
            return true;
        }
    }

    public void AttackBuilding(Building building)
    {
        building.currentHp -= Mathf.Max(damage - building.defense, 0);
        building.currentHp = Mathf.Max(building.currentHp, 0);
        building.slider.value = building.currentHp;
        building.slider.gameObject.SetActive(true);

        if (building.currentHp <= 0)
        {
            building.isDestroy = true;

            if (building.GetType() == typeof(Area))
            {
                Area area = (Area)building;
                area.belongCity.CalculateIncome();
            }

            else if (building.GetType() == typeof(City))
            {
                City city = (City)building;
                city.CalculateIncome();
            }
        }

        if (building.buildingProperty.buildingType == BuildingType.City ||
            building.buildingProperty.buildingType == BuildingType.MilitaryBase ||
            building.buildingProperty.buildingType == BuildingType.AirForceBase ||
            building.buildingProperty.buildingType == BuildingType.NavalBase)
        {
            if (building.belongCell.cubePosition.CheckDistance(currentPos.cubePosition) <= 3)
            {
                currentHp -= Mathf.Max(building.damage - property.armor, 0);
                slider.value = currentHp;
                slider.gameObject.SetActive(true);

                if (currentHp <= 0)
                {
                    UnitDestroy();
                }
            }
        }

        if(isAutoMove)
            WorldController.instance.movingUnitList.Remove(this);
    }

    public void UnitDestroy()
    {
        player.unitList.Remove(this);
        WorldController.instance.activeUnitList.Remove(this);
        WorldController.instance.movingUnitList.Remove(this);

        currentPos.unit = null;

        currentPos.mapObjectList.Remove(this);

        if(WorldController.instance.playerController.selectedUnit == this)
        {
            WorldController.instance.playerController.selectedUnit = null;
            WorldController.instance.playerController.CancelUnitSelect();
            WorldController.instance.uiController.CloseUnitUI();
        }

        if(cityModel != null)
            Destroy(cityModel);

        WorldController.instance.CheckVictory();

        Destroy(this.gameObject);
    }
}
