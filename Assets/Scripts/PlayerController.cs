using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public List<Unit> unitList = new List<Unit>();
    const string MapTag = "Map";

    //UI
    public GameObject unitUI;
    public TextMeshProUGUI unitName;

    public GameObject buildingUI;
    public TextMeshProUGUI buildingName;

    public Unit selectedUnit;
    public GameObject selectedBuilding;
    MapCell rightSelectedCell;
    MapCell leftSelectedCell;
    static public MapCell previousSelectedCell;

    List<MapCell> buildingBlock = new List<MapCell>();
    bool isBuildingSelect = false;
    static public int buildNum = 0;
    int buildingRange = 0;

    bool isMovingSelect = false;

    // Start is called before the first frame update
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
            if (previousSelectedCell != null)
                previousSelectedCell.GetComponent<Renderer>().material.color = previousSelectedCell.color;

            foreach (MapCell cell in buildingBlock)
            {
                cell.GetComponent<Renderer>().material.color = cell.color;
            }
            buildingBlock.Clear();

            if(isBuildingSelect || isMovingSelect)
            {
                MapCell select = hitInfo.collider.GetComponent<MapCell>();
                
                if (select.cost != 0 && select.building == null)
                {
                    select.GetComponent<Renderer>().material.color = Color.yellow;
                    
                    if(isBuildingSelect)
                    {
                        buildingBlock = select.CheckCellInRange(buildingRange);

                        foreach (MapCell cell in buildingBlock)
                        {
                            cell.GetComponent<Renderer>().material.color = Color.gray;
                        }
                    }

                    previousSelectedCell = select;
                }

                else
                {
                    hitInfo.collider.GetComponent<Renderer>().material.color = Color.red;
                    previousSelectedCell = select;
                }
            }          

            //Left click
            if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
            {
                if (leftSelectedCell != null)
                    leftSelectedCell.GetComponent<Renderer>().material.color = leftSelectedCell.color;
                hitInfo.collider.GetComponent<Renderer>().material.color = Color.red;
                leftSelectedCell = hitInfo.collider.gameObject.GetComponent<MapCell>();

                //Build
                if (isBuildingSelect && leftSelectedCell.cost != 0 && !selectedUnit.isMoving)
                {
                    if (leftSelectedCell.building == null)
                    {
                        selectedUnit.CheckPath(leftSelectedCell);
                        selectedUnit.startMove = true;
                        selectedUnit.isBuilding = true;
                        selectedUnit.targetPos = leftSelectedCell;
                    }

                    foreach (MapCell cell in buildingBlock)
                    {
                        cell.GetComponent<Renderer>().material.color = cell.color;
                    }

                    isBuildingSelect = false;
                    previousSelectedCell.GetComponent<Renderer>().material.color = previousSelectedCell.color;
                    previousSelectedCell = null;
                }

                //Move
                else if(isMovingSelect && leftSelectedCell.cost != 0 && !selectedUnit.isMoving)
                {
                    selectedUnit.CheckPath(leftSelectedCell);
                    selectedUnit.startMove = true;
                    selectedUnit.targetPos = leftSelectedCell;

                    isMovingSelect = false;
                    previousSelectedCell.GetComponent<Renderer>().material.color = previousSelectedCell.color;
                    previousSelectedCell = null;
                }

                //Select Unit
                else if (hitInfo.collider.GetComponent<MapCell>().units.Count != 0) //Check has unit or not
                {
                    MapCell mapCell = hitInfo.collider.GetComponent<MapCell>();
                    
                    if(selectedUnit == null || selectedUnit != mapCell.units[0])
                    {
                        SelectUnit(mapCell.units[0]);
                    }
                    
                    else
                    {
                        if(mapCell.units.Count > mapCell.units.IndexOf(selectedUnit) + 1)
                        {
                            SelectUnit(mapCell.units[mapCell.units.IndexOf(selectedUnit) + 1]);
                        }

                        else if(mapCell.building != null)
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

                    unitUI.SetActive(false);
                    buildingUI.SetActive(false);
                }
            }
        }

        //Right Click
        if(selectedUnit != null && (!selectedUnit.isMoving || selectedUnit.isAction))//is selecting unit
        {
            if(!isBuildingSelect)
            {
                if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
                {
                    if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask(MapTag)))
                    {
                        if (hitInfo.collider.gameObject != selectedUnit.currentPos && //check is the unit's currentPos or not
                            hitInfo.collider.GetComponent<MapCell>().cost != 0) //the mapcell not can't move
                        {
                            if (rightSelectedCell != null)
                            {
                                if (rightSelectedCell.gameObject != hitInfo.collider.gameObject)
                                {
                                    rightSelectedCell.GetComponent<Renderer>().material.color = rightSelectedCell.color;
                                    rightSelectedCell = hitInfo.collider.gameObject.GetComponent<MapCell>();
                                    rightSelectedCell.GetComponent<Renderer>().material.color = Color.green;

                                    foreach (MapCell cell in WorldController.map)
                                    {
                                        cell.gameObject.GetComponent<Renderer>().material.color = cell.GetComponent<MapCell>().color;
                                    }
                                    rightSelectedCell.GetComponent<Renderer>().material.color = Color.green;
                                    selectedUnit.CheckPath(rightSelectedCell);
                                }
                            }

                            else
                            {
                                rightSelectedCell = hitInfo.collider.gameObject.GetComponent<MapCell>();
                                rightSelectedCell.GetComponent<Renderer>().material.color = Color.green;

                                selectedUnit.CheckPath(rightSelectedCell);
                            }
                        }
                    }
                }

                else if (Input.GetMouseButtonUp(1))
                {
                    if (rightSelectedCell != null)
                    {
                        rightSelectedCell.GetComponent<Renderer>().material.color = rightSelectedCell.color;
                        selectedUnit.startMove = true;
                        selectedUnit.targetPos = rightSelectedCell;
                    }
                }
            }

            else if(Input.GetMouseButtonUp(1))
            {
                isBuildingSelect = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach (MapCell cell in WorldController.map)
            {
                cell.gameObject.GetComponent<Renderer>().material.color = cell.GetComponent<MapCell>().color;
            }

            selectedUnit = null;
            rightSelectedCell = null;
            unitUI.SetActive(false);
        }
    }

    

    public void SelectUnit(Unit unit)
    {
        selectedBuilding = null;
        buildingUI.SetActive(false);

        selectedUnit = unit;
        unitUI.SetActive(true);
        unitName.text = selectedUnit.name;
    }

    public void SelectBuilding(GameObject building)
    {
        selectedUnit = null;
        unitUI.SetActive(false);

        selectedBuilding = building;
        buildingUI.SetActive(true);
        buildingName.text = selectedBuilding.name;
    }

    public void Skip()
    {
        if(selectedUnit != null)
            selectedUnit.isAction = true;
    } //Unit Skip Button

    public void Move()
    {
        isMovingSelect = true;
    } //Unit Move Button

    public void DestroyUnit()
    {
        unitList.Remove(selectedUnit);
        selectedUnit.currentPos.units.Remove(selectedUnit);
        GameObject.Destroy(selectedUnit.gameObject);
        selectedUnit = null;
        unitUI.SetActive(false);
    } //Unit Destroy Button

    public void Building()
    {
        isBuildingSelect = true;
        buildingRange = 2;
    } //Unit Build city Button
}
