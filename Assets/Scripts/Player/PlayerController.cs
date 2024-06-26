using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    const string MapTag = "Map";

    [Header("Select Cell")]
    public GameObject selectionList;
    public GameObject whiteSelectCell;
    public GameObject yellowSelectCell;
    public GameObject redSelectCell;
    public SelectionCell greenSelectCell;
    public List<SelectionCell> blueSelectCellList;

    public GameObject blueSelectCellPrefab;

    Ray ray;
    RaycastHit hitInfo;
    public MapCell selectingCell, previousSelectedCell;
    public Unit selectedUnit;
    public Building selectedBuilding;

    public BuildingProperty cityBuilding;
    public bool isBuildingArea, isBuildingCity, isMoving;
    public bool isRightClicking, canBuild;

    private void Update()
    {
        RaycastMapCell();

        ShowPath();

        Controller();

        previousSelectedCell = selectingCell;
    }

    void RaycastMapCell()
    {
        hitInfo = new RaycastHit();
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask(MapTag)) || EventSystem.current.IsPointerOverGameObject())//no raycast or block by UI
        {
            selectingCell = null;
            return;
        }

        selectingCell = hitInfo.collider.GetComponent<MapCell>();

        if (selectingCell == previousSelectedCell)
            return;

        DisableSelectCell(redSelectCell);
        DisableSelectCell(whiteSelectCell.gameObject);

        //is Moving unit
        if (isMoving)
        {
            CheckMovetable();
        }

        //is Building Area
        else if(isBuildingArea)
        {
            if (selectingCell.building == null && selectingCell.unit == null && 
                WorldController.instance.buildingController.AreaBuildCheck((City)selectedBuilding, selectingCell))
            {
                ActiveSelectCell(greenSelectCell.gameObject, selectingCell.transform);
                canBuild = true;
            }

            else
            {
                DisableSelectCell(greenSelectCell.gameObject);
                ActiveSelectCell(redSelectCell, selectingCell.transform);

                WorldController.instance.buildingController.DisableBlueCells();
                WorldController.instance.buildingController.DisableModel();
                canBuild = false;
            }
        }

        //is Building City
        else if (isBuildingCity)
        {
            if (selectingCell.building == null && 
                (selectingCell.unit == null || selectingCell.unit.player == WorldController.instance.currentPlayer) &&
                selectingCell.mapType != (int)MapTypeName.Ocean && 
                selectingCell.mapType != (int)MapTypeName.Coast &&
                WorldController.instance.buildingController.CityBuildCheck(selectingCell))
            {
                ActiveSelectCell(greenSelectCell.gameObject, selectingCell.transform);
                canBuild = true;
            }
                
            else
            {
                DisableSelectCell(greenSelectCell.gameObject);
                ActiveSelectCell(redSelectCell, selectingCell.transform);
                
                WorldController.instance.buildingController.DisableBlueCells();
                WorldController.instance.buildingController.DisableModel();
                canBuild = false;
            }
        }

        else
        {
            ActiveSelectCell(whiteSelectCell, hitInfo.transform);
        }
    }

    void ShowPath()
    {
        //Showing the the pathfinding result
        if (selectedUnit != null && !selectedUnit.showPath && selectedUnit.endSearch)
        {
            if (selectedUnit.path.Count == 0)
            {
                ActiveSelectCell(redSelectCell, selectedUnit.selectingTarget.transform);
            }

            foreach (SelectionCell blueCell in blueSelectCellList)
            {
                DisableSelectCell(blueCell.gameObject);
                blueCell.turnUI.SetActive(false);
            }

            greenSelectCell.turnUI.SetActive(false);

            ActiveSelectCell(yellowSelectCell, selectedUnit.currentPos.transform);

            int turnCount = 0;

            for (int i = 0; i < selectedUnit.path.Count; i++)
            {
                if (selectedUnit.path[i] == selectedUnit.targetPos)
                {
                    ActiveSelectCell(greenSelectCell.gameObject, selectedUnit.path[i].transform);

                    if (turnCount < selectedUnit.eachTurnStartCell.Count && selectedUnit.path[i] == selectedUnit.eachTurnStartCell[turnCount])
                    {
                        greenSelectCell.turnText.text = (turnCount + 2).ToString();
                        greenSelectCell.turnUI.SetActive(true);
                        turnCount++;
                    }
                }

                else if (i < blueSelectCellList.Count)
                {
                    if (turnCount < selectedUnit.eachTurnStartCell.Count && selectedUnit.path[i] == selectedUnit.eachTurnStartCell[turnCount])
                    {
                        blueSelectCellList[i].turnText.text = (turnCount + 2).ToString();
                        blueSelectCellList[i].turnUI.SetActive(true);
                        turnCount++;
                    }

                    blueSelectCellList[i].resourceIcon.transform.parent.gameObject.SetActive(false);
                    ActiveSelectCell(blueSelectCellList[i].gameObject, selectedUnit.path[i].transform);
                }

                else
                {
                    GameObject tempBlueCell = Instantiate(blueSelectCellPrefab);
                    blueSelectCellList.Add(tempBlueCell.GetComponent<SelectionCell>());
                    ActiveSelectCell(tempBlueCell, selectedUnit.path[i].transform);

                    if (turnCount < selectedUnit.eachTurnStartCell.Count && selectedUnit.path[i] == selectedUnit.eachTurnStartCell[turnCount])
                    {
                        blueSelectCellList[i].turnText.text = (turnCount + 2).ToString();
                        blueSelectCellList[i].turnUI.SetActive(true);
                        turnCount++;
                    }
                }
            }

            selectedUnit.showPath = true;
        }
    }

    void Controller()
    {
        if (selectingCell != null)
        {
            if (Input.GetMouseButtonUp(1)) //Right Click Up
            {
                if (isMoving) //Moving
                {
                    if (selectedUnit != null && selectedUnit.targetPos != selectedUnit.currentPos && !selectedUnit.completeAttack)
                    {
                        selectedUnit.isSleep = false;
                        selectedUnit.startMove = true;
                    }
                    isMoving = false;
                }

                isRightClicking = false;
            }

            else if (Input.GetMouseButtonDown(1)) //Right Click Down
            {
                if (isBuildingArea || isBuildingCity || isMoving) //Cancel
                {
                    isBuildingArea = isBuildingCity = isMoving = false;
                    WorldController.instance.buildingController.CancelBuilding();
                    DisablePathShow();
                }

                else if (selectedUnit != null && selectedUnit.player == WorldController.instance.currentPlayer && !selectedUnit.completeAttack)
                {
                    isMoving = true;
                    DisableSelectCell(whiteSelectCell);
                    CheckMovetable();
                }

                isRightClicking = true;
            }

            else if (Input.GetMouseButtonDown(0)) //Left Click Down
            {
                DisableSelectCell(greenSelectCell.gameObject);

                if (isMoving && isRightClicking) //Cancel Move
                {
                    isMoving = false;
                    selectedUnit.isAttack = false;
                    selectedUnit.attackTarget = null;
                    isRightClicking = false;
                    selectedUnit.path.Clear();
                    DisableSelectCell(redSelectCell);
                    DisablePathShow();
                }

                else if (isMoving && selectingCell != selectedUnit.currentPos && !selectedUnit.completeAttack)
                {
                    DisableSelectCell(redSelectCell);
                    CheckMovetable();
                    if (selectingCell != selectedUnit.currentPos)
                    {
                        selectedUnit.isSleep = false;
                        selectedUnit.startMove = true;
                    }
                    isMoving = false;
                }

                else if (isBuildingArea && canBuild)
                {
                    City selectingCity = (City)selectedBuilding;
                    selectingCity.Produce(WorldController.instance.buildingController.modelObject, WorldController.instance.buildingController.selectingBuilding, selectingCell);
                    isBuildingArea = false;
                }

                else if (isBuildingCity && canBuild && !selectedUnit.completeAttack)
                {
                    if (selectingCell != selectedUnit.currentPos)
                    {
                        DisableSelectCell(redSelectCell);
                        CheckMovetable();
                        selectedUnit.cityModel = WorldController.instance.buildingController.modelObject;
                        WorldController.instance.buildingController.modelObject.SetActive(false);
                        selectedUnit.isBuilding = true;
                        selectedUnit.isSleep = false;
                        selectedUnit.startMove = true;
                    }
                    else
                    {
                        WorldController.instance.buildingController.BuildCity(WorldController.instance.currentPlayer, WorldController.instance.buildingController.modelObject, selectingCell);
                        selectedUnit.UnitDestroy();
                    }

                    isBuildingCity = false;
                }

                else if(!isBuildingCity && !isBuildingArea) //Not Building
                {
                    DisableSelectCell(redSelectCell);
                    ActiveSelectCell(yellowSelectCell, selectingCell.transform);
                    NextMapObject(selectingCell.mapObjectList);
                }
            }
        }
    }

    void CheckMovetable()
    {
        selectedUnit.showPath = false;
        selectedUnit.path.Clear();
        selectedUnit.eachTurnStartCell.Clear();
        selectedUnit.selectingTarget = selectingCell;
        selectedUnit.isBuilding = false;
        selectedUnit.isMoving = false;
        selectedUnit.isAttack = false;
        selectedUnit.isAction = false;
        selectedUnit.isAutoMove = false;
        WorldController.instance.isAutoUnitArrive = false;

        //Attack Check
        if (selectingCell.unit != null)
        {
            if (selectingCell.unit.player != WorldController.instance.currentPlayer)
            {
                selectedUnit.CheckAttackUnit(selectingCell.unit);//Attack Ground Unit
            }

            else
            {
                selectedUnit.startMove = false;
            }
        }

        else if (selectingCell.building != null && 
                selectingCell.building.player != WorldController.instance.currentPlayer && 
                !selectingCell.building.isDestroy)
        {
            Debug.Log("Check Build Attack");
            selectedUnit.CheckAttackBuilding(selectingCell.building);
        }

        else
        {
            selectedUnit.CheckPath(selectingCell);
        }
    }

    public void DisablePathShow()
    {
        foreach (SelectionCell blueCell in blueSelectCellList)
        {
            DisableSelectCell(blueCell.gameObject);
        }

        greenSelectCell.turnUI.SetActive(false);
        DisableSelectCell(greenSelectCell.gameObject);
    }

    public void ActiveSelectCell(GameObject selectCell, Transform targetTransform)
    {
        selectCell.SetActive(true);
        selectCell.transform.position = targetTransform.position;
        selectCell.transform.parent = targetTransform;
    }

    public void DisableSelectCell(GameObject selectCell)
    {
        selectCell.SetActive(false);
        selectCell.transform.parent = selectionList.transform;
    }

    public bool NextUnit(List<Unit> unitsList)
    {
        selectedBuilding = null;
        WorldController.instance.uiController.buildingUIController.CloseBuildingUI();

        if(unitsList.Count > 0)
        {
            Unit tempUnit = unitsList[0]; //Move to back
            unitsList.Remove(tempUnit);
            unitsList.Add(tempUnit);

            selectedUnit = tempUnit;
            selectedUnit.showPath = false;
            WorldController.instance.uiController.OpenUnitUI(selectedUnit);
            return true;
        }

        return false;
    }    

    public bool NextMapObject(List<MapObject> mapObjectList)
    {
        selectedBuilding = null;
        CancelUnitSelect();
        WorldController.instance.uiController.CloseAllUI();

        if(mapObjectList.Count > 0)
        {
            MapObject tempObj = mapObjectList[0]; //Move to back
            mapObjectList.Remove(tempObj);
            mapObjectList.Add(tempObj);

            if (tempObj.GetType() == typeof(Unit))
            {
                selectedUnit = (Unit)tempObj;

                if (selectedUnit.player == WorldController.instance.currentPlayer)
                    selectedUnit.showPath = false;

                WorldController.instance.uiController.OpenUnitUI(selectedUnit);
            }
            
            else if(tempObj.GetType() == typeof(City))
            {
                selectedBuilding = (Building)tempObj;
                WorldController.instance.uiController.buildingUIController.OpenBuildingUI(selectedBuilding);
            }

            return true;
        }

        return false;
    }

    public void CancelUnitSelect()
    {
        selectedUnit = null;

        DisablePathShow();
    }

    public void BuildArea(BuildingProperty building)
    {
        if (isBuildingArea)
            WorldController.instance.buildingController.CancelBuilding();

        WorldController.instance.buildingController.StartBuilding(building);
        isBuildingArea = true;
    }

    public void BuildCity()
    {
        if(!isBuildingCity)
        {
            WorldController.instance.buildingController.StartBuilding(cityBuilding);
            isBuildingCity = true;
        }
    }
}