using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    static public int turn;
    static public List<Player> playerList = new List<Player>();
    static public MapCell[,] map;

    public TextMeshProUGUI turnTxt;
    public Button turnBtn;

    static public List<Unit> activeUnitList = new List<Unit>();
    static public List<Unit> movingUnitList = new List<Unit>();

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

    bool allUnitMove = false;

    // Start is called before the first frame update
    void Start()
    {
        turn = 0;
        playerList.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>());
        Instantiate(worldInit, gameObject.transform);
    }

    void Update()
    {
        if (activeUnitList.Count != 0)
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

    IEnumerator endTurnCoroutine;

    public void NextTurn()
    {
        endTurnCoroutine = EndTurn();
        StartCoroutine(endTurnCoroutine);
        StopCoroutine(endTurnCoroutine);
    }

    IEnumerator EndTurn()
    {
        allUnitMove = true;

        foreach (Unit unit in playerList[0].unitList /*current player*/)
        {
            if(!unit.isAction)
            {
                allUnitMove = false;
                break;
            }
        }

        if (!allUnitMove)
            yield return null;
        else
            TurnStart();
    }

    public void TurnStart()
    {
        turn++;
        turnTxt.text = "TURN " + turn;

        foreach (Unit unit in playerList[0].unitList)
        {
            unit.remainMove = unit.movement; //reset unit remain movement

            if (unit.isMoving)
                movingUnitList.Add(unit);
            else
                activeUnitList.Add(unit);
        }
    }

    public void TurnBtn()
    {
        if (activeUnitList.Count != 0) 
        {
            NextUnit();
        }

        else
        {
            foreach (Unit unit in movingUnitList)
            {
                unit.startMove = true;
            }

            NextTurn();
        }
    }



    public void NextUnit()
    {
        HumanPlayer player = (HumanPlayer)playerList[0];

        player.selectedBuilding = null;
        buildingUI.SetActive(false);

        foreach (Unit unit in activeUnitList)
        {
            if (unit != player.selectedUnit &&
                player.unitList.IndexOf(unit) > player.unitList.IndexOf(player.selectedUnit)) //Make sure it is going the next unit
            {
                player.selectedUnit = unit;
                break;
            }

            else if (player.unitList.IndexOf(unit) == activeUnitList.Count - 1)
            {
                player.selectedUnit = activeUnitList[0]; //go back to first active unit in the list
                break;
            }
        }

        if (player.selectedUnit != null)
        {
            unitName.text = player.selectedUnit.name;
            unitUI.SetActive(true);
            cameraScirpt.MoveCamera(player.selectedUnit.currentPos);
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
