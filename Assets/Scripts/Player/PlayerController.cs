using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.CanvasScaler;

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
    public static Unit selectedUnit;
    public static Building selectedBuilding;

    public BuildingProperty cityBuilding;
    public static bool isBuildingArea, isBuildingCity, isMoving;
    public bool isRightClicking;

    bool isBuildingShow = false;

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

        //is Moving unit
        if (isMoving)
        {
            DisableSelectCell(whiteSelectCell);
            CheckMovetable();
        }

        //is Building Area
        else if(isBuildingArea)
        {
            WorldController.buildingController.BuildCheck(selectingCell);
        }

        //is Building City
        else if (isBuildingCity)
        {
            WorldController.buildingController.BuildCheck(selectingCell);
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
                ActiveSelectCell(redSelectCell, selectedUnit.targetPos.transform);
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
                    if (selectedUnit.targetPos != selectedUnit.currentPos)
                    {
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
                    DisablePathShow();
                }

                else if (selectedUnit != null)
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
                DisableSelectCell(redSelectCell);
                ActiveSelectCell(yellowSelectCell, selectingCell.transform);

                if (isMoving && isRightClicking) //Cancel Move
                {
                    isMoving = false;
                    isRightClicking = false;
                    DisablePathShow();
                }

                else if (isMoving && selectingCell != selectedUnit.currentPos)
                {
                    CheckMovetable();
                    if (selectingCell != selectedUnit.currentPos)
                    {
                        selectedUnit.startMove = true;
                    }
                    isMoving = false;
                }

                else if (isBuildingArea)
                {
                    WorldController.buildingController.BuildArea((City)selectedBuilding, selectingCell);
                    isBuildingArea = false;
                }

                else if (isBuildingCity)
                {
                    WorldController.buildingController.BuildCity(selectingCell);
                    isBuildingCity = false;
                }

                else
                {
                    CancelUnitSelect();
                    selectedBuilding = null;

                    if (selectingCell != previousSelectedCell)
                    {
                        isBuildingShow = false;
                    }

                    if (!isBuildingShow && selectingCell.building != null)
                    {
                        selectedBuilding = selectingCell.building;
                        UI_Controller.buildingUIController.OpenBuildingUI(selectedBuilding);
                        isBuildingShow = true;
                    }

                    else
                    {
                        if (NextUnit(selectingCell.unitsList))
                            WorldController.UI.OpenUnitUI(selectedUnit);
                        else
                        {
                            WorldController.UI.CloseUnitUI();
                            isBuildingShow = false;
                        }
                           
                    }
                }
            }
        }
    }

    void CheckMovetable()
    {
        if (selectingCell != selectedUnit.currentPos)
            selectedUnit.CheckPath(selectingCell);
    }

    public void DisablePathShow()
    {
        foreach (SelectionCell blueCell in blueSelectCellList)
        {
            DisableSelectCell(blueCell.gameObject);
        }

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
        UI_Controller.buildingUIController.CloseBuildingUI();

        for (int i = 0; i < unitsList.Count; i++)
        {
            if (unitsList[i].player == WorldController.currentPlayer)
            {
                Unit tempUnit = unitsList[i]; //Move to back
                unitsList.Remove(tempUnit);
                unitsList.Add(tempUnit);

                selectedUnit = tempUnit;
                selectedUnit.showPath = false;
                WorldController.UI.OpenUnitUI(selectedUnit);
                return true;
            } 
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
        WorldController.buildingController.StartBuilding(building);
        isBuildingArea = true;
    }

    public void BuildCity()
    {
        WorldController.buildingController.StartBuilding(cityBuilding);
        isBuildingCity = true;
    }
}