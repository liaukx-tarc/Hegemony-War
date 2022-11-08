using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    static public int turn;
    static public List<PlayerController> playerList = new List<PlayerController>();
    static public MapCell[,] map;

    public TextMeshProUGUI turnTxt;
    public Button turnBtn;

    bool haveActiveUnit;
    string activeTxt = "NEXT\nUNIT";
    Color activeColor = new Color(0.08f, 0.3f, 1);
    Color activeSelectColor = new Color(0.08f, 0.6f, 1);

    string nextTxt = "END\nTURN";
    Color nextColor = new Color(0.95f, 0.75f, 0.08f);
    Color nextSelectColor = new Color(0.98f, 0.85f, 0.45f);

    public GameObject unitUI;
    public TextMeshProUGUI unitName;

    public GameObject buildingUI;
    public TextMeshProUGUI buildingName;

    public CameraControl cameraScirpt;

    public GameObject worldInit;

    // Start is called before the first frame update
    void Start()
    {
        turn = 1;
        playerList.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>());
        Instantiate(worldInit, gameObject.transform);
    }

    void Update()
    {
        haveActiveUnit = false;
        
        foreach (Unit unit in playerList[0].unitList)
        {
            if (!unit.isAction)
            {
                haveActiveUnit = true;
                break;
            }
        }

        if(haveActiveUnit)
        {
            ColorBlock turnBtnColors = turnBtn.colors;
            turnBtnColors.normalColor = turnBtnColors.pressedColor = turnBtnColors.selectedColor = activeColor;
            turnBtnColors.highlightedColor = activeSelectColor;
            turnBtnColors.disabledColor = Color.gray;
            turnBtn.colors = turnBtnColors;
            turnBtn.GetComponentInChildren<TextMeshProUGUI>().text = activeTxt;
            turnBtn.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        else
        {
            ColorBlock turnBtnColors = turnBtn.colors;
            turnBtnColors.normalColor = turnBtnColors.pressedColor = turnBtnColors.selectedColor = nextColor;
            turnBtnColors.highlightedColor = nextSelectColor;
            turnBtnColors.disabledColor = Color.gray;
            turnBtn.colors = turnBtnColors;
            turnBtn.GetComponentInChildren<TextMeshProUGUI>().text = nextTxt;
            turnBtn.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        }
    }

    public void NextTurn()
    {
        if (haveActiveUnit)
        {
            NextUnit();
        }

        else
        {
            turn++;
            turnTxt.text = "TURN " + turn;
            foreach (Unit unit in playerList[0].unitList)
            {
                unit.isAction = false;
                unit.remainMove = unit.movement;
                if (unit.isMoving)
                    unit.startMove = true;
            }
        }
    }

    public void NextUnit()
    {
        if (haveActiveUnit)
        {
            //close the ui of previous unit
            if (playerList[0].selectedUnit != null && playerList[0].selectedUnit.isAction)
            {
                playerList[0].selectedUnit = null;
                unitUI.SetActive(false);
            }
                

            List<Unit> activeUnit = new List<Unit>();

            foreach (Unit unit in playerList[0].unitList)
            {
                if (!unit.isAction)
                {
                    activeUnit.Add(unit);
                }
            }

            foreach (Unit unit in activeUnit)
            {
                if (playerList[0].selectedUnit != unit &&
                        playerList[0].unitList.IndexOf(unit) > playerList[0].unitList.IndexOf(playerList[0].selectedUnit)) //Make sure it is going the next unit
                {
                    playerList[0].selectedBuilding = null;
                    buildingUI.SetActive(false);

                    playerList[0].selectedUnit = unit;
                    unitUI.SetActive(true);
                    unitName.text = playerList[0].selectedUnit.name;
                    break;
                }

                else if (playerList[0].unitList.IndexOf(unit) == playerList[0].unitList.IndexOf(activeUnit[activeUnit.Count - 1]))
                {
                    playerList[0].selectedBuilding = null;
                    buildingUI.SetActive(false);

                    playerList[0].selectedUnit = activeUnit[0];
                    unitUI.SetActive(true);
                    unitName.text = playerList[0].selectedUnit.name;
                    break;
                }
            }

            if (playerList[0].selectedUnit != null)
                cameraScirpt.MoveCamera(playerList[0].selectedUnit.currentPos);
                
        }
    }

    //Lens
    bool lenState = false;
    public GameObject lens1;
    public GameObject lens2;
    public GameObject lens3;

    public void lensOpen()
    {
        lenState = !lenState;
        lens1.SetActive(lenState);
        lens2.SetActive(lenState);
        lens3.SetActive(lenState);
    }
}
