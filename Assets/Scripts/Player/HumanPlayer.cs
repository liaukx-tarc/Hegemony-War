using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HumanPlayer : Player
{
    const string MapTag = "Map";

    //Unit select
    public Unit selectedUnit;
    public Building selectedBuilding;
    public GameObject selectionList;
    public GameObject whiteSelectCell;
    public GameObject yellowSelectCell;
    public GameObject redSelectCell;
    public GameObject greenSelectCell;
    public List<GameObject> blueSelectCellList;
    MapCell rightSelectedCell;
    MapCell leftSelectedCell;
    MapCell previousSelectedCell;

    List<MapCell> buildingBlock = new List<MapCell>();
    List<GameObject> previousSelectedList = new List<GameObject>();
    bool isBuildingSelect = false;

    int buildingRange = 0;

    bool isMovingSelect = false;

    // Update is called once per frame
    void Update()
    {
        SelectMapCell();
    }

    void SelectMapCell()
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask(MapTag)) &&//Raycast
        !EventSystem.current.IsPointerOverGameObject())//Check has block by UI or not
        {
            if (previousSelectedCell == null || previousSelectedCell.gameObject != hitInfo.collider.gameObject)
            {
                if (isBuildingSelect || isMovingSelect)
                {
                    MapCell select = hitInfo.collider.GetComponent<MapCell>();

                    if (select.cost != 0 && select.building == null)
                    {
                        redSelectCell.transform.parent = selectionList.transform;
                        redSelectCell.SetActive(false);

                        yellowSelectCell.SetActive(true);
                        yellowSelectCell.transform.position = select.transform.position;
                        yellowSelectCell.transform.parent = select.transform;


                        if (isBuildingSelect)
                        {
                            buildingBlock = select.CheckCellInRange(buildingRange);

                            for (int i = 0; i < buildingBlock.Count; i++)
                            {
                                blueSelectCellList[i].SetActive(true);
                                blueSelectCellList[i].transform.position = buildingBlock[i].transform.position;
                                blueSelectCellList[i].transform.parent = buildingBlock[i].transform;
                                previousSelectedList.Add(blueSelectCellList[i]);
                            }
                        }
                    }

                    else
                    {
                        yellowSelectCell.transform.parent = selectionList.transform;
                        yellowSelectCell.SetActive(false);

                        redSelectCell.SetActive(true);
                        redSelectCell.transform.position = select.transform.position;
                        redSelectCell.transform.parent = select.transform;
                    }

                    previousSelectedCell = select;
                }
            }

            //Left click
            if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
            {
                yellowSelectCell.transform.parent = selectionList.transform;
                yellowSelectCell.SetActive(false);
                redSelectCell.transform.parent = selectionList.transform;
                redSelectCell.SetActive(false);

                whiteSelectCell.SetActive(true);
                whiteSelectCell.transform.position = hitInfo.transform.position;
                whiteSelectCell.transform.parent = hitInfo.transform;

                leftSelectedCell = hitInfo.collider.gameObject.GetComponent<MapCell>();

                //Build
                if (isBuildingSelect && leftSelectedCell.cost != 0 && !selectedUnit.isMoving)
                {
                    foreach (GameObject cell in previousSelectedList)
                    {
                        cell.transform.parent = selectionList.transform;
                        cell.SetActive(false);
                    }

                    previousSelectedList.Clear();

                    yellowSelectCell.transform.parent = selectionList.transform;
                    yellowSelectCell.SetActive(false);

                    if (leftSelectedCell.building == null)
                    {
                        selectedUnit.CheckPath(leftSelectedCell);
                        selectedUnit.startMove = true;
                        selectedUnit.isBuilding = true;
                        selectedUnit.targetPos = leftSelectedCell;
                    }

                    isBuildingSelect = false;
                }

                //Move
                else if (isMovingSelect && leftSelectedCell.cost != 0 && !selectedUnit.isMoving)
                {
                    selectedUnit.CheckPath(leftSelectedCell);
                    selectedUnit.startMove = true;
                    selectedUnit.targetPos = leftSelectedCell;

                    isMovingSelect = false;
                }

                //Select Unit
                else if (hitInfo.collider.GetComponent<MapCell>().unitsList.Count != 0) //Check has unit or not
                {
                    MapCell mapCell = hitInfo.collider.GetComponent<MapCell>();

                    if (selectedUnit == null || selectedUnit != mapCell.unitsList[0])
                    {
                        SelectUnit(mapCell.unitsList[0]);
                    }

                    else
                    {
                        if (mapCell.unitsList.Count > mapCell.unitsList.IndexOf(selectedUnit) + 1)
                        {
                            SelectUnit(mapCell.unitsList[mapCell.unitsList.IndexOf(selectedUnit) + 1]);
                        }

                        else if (mapCell.building != null)
                        {
                            SelectBuilding(mapCell.building);
                        }
                    }
                }

                //Select Building
                else if (hitInfo.collider.GetComponent<MapCell>().building != null)
                {
                    SelectBuilding(hitInfo.collider.GetComponent<MapCell>().building);
                }

                //select a not unit and building cell
                else if (!isBuildingSelect)
                {
                    selectedUnit = null;
                    selectedBuilding = null;

                    WorldController.UI.Disable(WorldController.UI.unitUI);
                    WorldController.UI.Disable(WorldController.UI.buildingUI);
                }
            }
        }

        //Right Click
        if (selectedUnit != null && (!selectedUnit.isMoving || selectedUnit.isAction))//is selecting unit
        {
            if (!isBuildingSelect)
            {
                if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
                {
                    if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask(MapTag)))
                    {
                        if (hitInfo.collider.gameObject != selectedUnit.currentPos)//check is the unit's currentPos or not 
                        {
                            if (rightSelectedCell == null || rightSelectedCell.gameObject != hitInfo.collider.gameObject)
                            {
                                if (hitInfo.collider.GetComponent<MapCell>().cost != 0)//the mapcell not can't move
                                {
                                    redSelectCell.transform.parent = selectionList.transform;
                                    redSelectCell.SetActive(false);

                                    greenSelectCell.SetActive(true);
                                    greenSelectCell.transform.position = hitInfo.transform.position;
                                    greenSelectCell.transform.parent = hitInfo.transform;
                                    rightSelectedCell = hitInfo.collider.gameObject.GetComponent<MapCell>();

                                    selectedUnit.CheckPath(rightSelectedCell);
                                }

                                else
                                {
                                    greenSelectCell.transform.parent = selectionList.transform;
                                    greenSelectCell.SetActive(false);

                                    redSelectCell.SetActive(true);
                                    redSelectCell.transform.position = hitInfo.transform.position;
                                    redSelectCell.transform.parent = hitInfo.transform;
                                }
                            }



                        }
                    }
                }

                else if (Input.GetMouseButtonUp(1))
                {
                    if (rightSelectedCell != null)
                    {
                        greenSelectCell.transform.parent = selectionList.transform;
                        greenSelectCell.SetActive(false);
                        redSelectCell.transform.parent = selectionList.transform;
                        redSelectCell.SetActive(false);

                        selectedUnit.startMove = true;
                        selectedUnit.targetPos = rightSelectedCell;
                    }
                }
            }

            else if (Input.GetMouseButtonUp(1))
            {
                foreach (GameObject cell in previousSelectedList)
                {
                    cell.transform.parent = selectionList.transform;
                    cell.SetActive(false);
                }

                previousSelectedList.Clear();

                yellowSelectCell.transform.parent = selectionList.transform;
                yellowSelectCell.SetActive(false);

                isBuildingSelect = false;
            }
        }

        //Testing Function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            whiteSelectCell.transform.parent = selectionList.transform;
            whiteSelectCell.SetActive(false);
            selectedUnit = null;
            rightSelectedCell = null;
            WorldController.UI.Disable(WorldController.UI.unitUI);
        }
    }

    public void SelectUnit(Unit unit)
    {
        selectedBuilding = null;
        WorldController.UI.Disable(WorldController.UI.buildingUI);

        selectedUnit = unit;
        WorldController.UI.Enable(WorldController.UI.unitUI);
        WorldController.UI.showUnit(selectedUnit.name, selectedUnit.template.property.maxHp, selectedUnit.currentHp, selectedUnit.template.property.armor, selectedUnit.damage, selectedUnit.remainMove);
    }

    public void SelectBuilding(Building building)
    {
        selectedUnit = null;
        WorldController.UI.Disable(WorldController.UI.unitUI);

        selectedBuilding = building;
        WorldController.UI.Enable(WorldController.UI.buildingUI);
        WorldController.UI.buildingName.text = selectedBuilding.name;
    }

    public void Skip()
    {
        if (selectedUnit != null)
        {
            selectedUnit.isAction = true;
            WorldController.activeUnitList.Remove(selectedUnit);
            worldController.NextUnit();
        }
            
    } //Unit Skip Button

    public void Move()
    {
        isMovingSelect = true;
    } //Unit Move Button

    public void DestroyUnit()
    {
        unitList.Remove(selectedUnit);
        selectedUnit.currentPos.unitsList.Remove(selectedUnit);
        GameObject.Destroy(selectedUnit.gameObject);
        selectedUnit = null;
        WorldController.UI.Disable(WorldController.UI.unitUI);
    } //Unit Destroy Button

    public void Building()
    {
        isBuildingSelect = true;
        buildingRange = 2;
    } //Unit Build city Button
}
