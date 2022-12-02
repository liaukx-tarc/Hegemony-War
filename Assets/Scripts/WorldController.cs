using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    static public int turn;
    
    //Player
    static public List<Player> playerList = new List<Player>();
    static public Player currentPlayer;
    
    //Map
    static public MapCell[,] map;
    static public Vector2 mapSize;

    //UI Controller
    static public UI_Controller UI;

    //Unit List
    static public List<Unit> activeUnitList = new List<Unit>();
    static public List<Unit> movingUnitList = new List<Unit>();

    public CameraControl cameraScirpt;

    public GameObject worldInit;

    bool allUnitMove = false;

    // Start is called before the first frame update
    void Start()
    {
        turn = 0;
        UI = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Controller>();
        InitializeWorld();
    }

    void InitializeWorld()
    {
        GameObject worldGenerator = Instantiate(worldInit, gameObject.transform);
        worldGenerator.GetComponent<MapCreate>().GenerateWorld();
        worldGenerator.GetComponent<PlayerCreate>().CreatePlayer();
        worldGenerator.GetComponent<UnitSpawn>().GenerateUnit();
    }

    IEnumerator endTurnCoroutine;

    public delegate void TurnEndFunction();
    public static TurnEndFunction turnEndFunction;

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

    public delegate void StartTurnFunction();
    public static StartTurnFunction startTurnFunction;

    public void TurnStart()
    {
        turn++;

        foreach (Unit unit in currentPlayer.unitList)
        {
            unit.remainMove = unit.template.property.speed; //reset unit remain speed

            if (unit.isMoving)
                movingUnitList.Add(unit);
            else
                activeUnitList.Add(unit);
        }

        if(startTurnFunction != null)
            startTurnFunction();
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
        UI.Disable(UI.buildingUI);

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
            UI.showUnit(player.selectedUnit.name, player.selectedUnit.template.property.maxHp, player.selectedUnit.currentHp, player.selectedUnit.template.property.armor, player.selectedUnit.damage, player.selectedUnit.remainMove);
            UI.Enable(UI.unitUI);
            cameraScirpt.MoveCamera(player.selectedUnit.currentPos);
        }
    }
}
