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
    public GameObject greenSelectCell;
    public List<GameObject> blueSelectCellList;

    public GameObject blueSelectCellPrefab;

    Ray ray;
    RaycastHit hitInfo;
    MapCell selectingCell, previousSelectedCell;
    public static Unit selectedUnit;
    public static Unit selectedBuilding;

    public static Building Building;
    public static bool isBuilding, isMoving;
    public bool isRightClicking;

    private void Update()
    {
        RaycastMapCell();

        if (selectedUnit != null && !selectedUnit.showPath && selectedUnit.endSearch)
        {
            if (selectedUnit.path.Count == 0)
            {
                ActiveSelectCell(redSelectCell, selectedUnit.targetPos.transform);
            }

            foreach (GameObject blueCell in blueSelectCellList)
            {
                DisableSelectCell(blueCell);
            }

            ActiveSelectCell(yellowSelectCell, selectedUnit.currentPos.transform);

            for (int i = 0; i < selectedUnit.path.Count; i++)
            {
                if (selectedUnit.path[i] == selectedUnit.targetPos)
                {
                    ActiveSelectCell(greenSelectCell, selectedUnit.path[i].transform);
                }

                else if (i < blueSelectCellList.Count)
                {
                    ActiveSelectCell(blueSelectCellList[i], selectedUnit.path[i].transform);
                }

                else
                {
                    GameObject tempBlueCell = Instantiate(blueSelectCellPrefab);
                    blueSelectCellList.Add(tempBlueCell);
                    ActiveSelectCell(tempBlueCell, selectedUnit.path[i].transform);
                }
            }

            selectedUnit.showPath = true;
        }

        if(selectingCell != null)
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
                if (isBuilding || isMoving) //Cancel
                {
                    isBuilding = isMoving = false;
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
                DisableSelectCell(greenSelectCell);
                DisableSelectCell(redSelectCell);
                ActiveSelectCell(yellowSelectCell, selectingCell.transform);

                if (isMoving && isRightClicking)
                {
                    isMoving = false;
                    isRightClicking = false;
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

                else if (isBuilding)
                {

                }

                else
                {
                    if (NextUnit(selectingCell.unitsList))
                        WorldController.UI.OpenUnitUI(selectedUnit);
                    else
                        WorldController.UI.CloseUnitUI();
                }
            }
        }

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

        //is Building Construction
        if(isBuilding)
        {

        }

        else
        {
            ActiveSelectCell(whiteSelectCell, hitInfo.transform);
        }
    }

    void CheckMovetable()
    {
        if (selectingCell != selectedUnit.currentPos)
            selectedUnit.CheckPath(selectingCell);
    }

    void ActiveSelectCell(GameObject selectCell, Transform targetTransform)
    {
        selectCell.SetActive(true);
        selectCell.transform.position = targetTransform.position;
        selectCell.transform.parent = targetTransform;
    }

    void DisableSelectCell(GameObject selectCell)
    {
        selectCell.SetActive(false);
        selectCell.transform.parent = selectionList.transform;
    }

    public bool NextUnit(List<Unit> unitsList)
    {
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
}