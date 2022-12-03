using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static WorldController;

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

    //Turn Button
    public TurnButton turnButton;

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
        nextTurnFunction += turnButton.ChangeTurnText;
        InitializeWorld();
    }

    void InitializeWorld()
    {
        GameObject worldGenerator = Instantiate(worldInit, gameObject.transform);
        worldGenerator.GetComponent<MapCreate>().GenerateWorld();
        worldGenerator.GetComponent<PlayerCreate>().CreatePlayer();
        worldGenerator.GetComponent<UnitSpawn>().GenerateUnit();

        GameStart();
    }

    //Testing Function
    public void AIAutoEnd()
    {
        Debug.Log(currentPlayer.name + " End");

        if (playerEndFunction != null)
            playerEndFunction();
        PlayerStart();
    }

    void GameStart()
    {
        currentPlayer = playerList[0];
        playerStartFunction += currentPlayer.playerStartFunction;
        playerEndFunction += currentPlayer.playerEndFunction;

        cameraScirpt.StartCamera(currentPlayer.unitList[0].currentPos);

        foreach (Unit unit in currentPlayer.unitList)
        {
            unit.remainMove = unit.template.property.speed; //reset unit remain speed

            if (unit.isMoving)
                movingUnitList.Add(unit);
            else
                activeUnitList.Add(unit);
        }

        if (playerStartFunction != null)
            playerStartFunction();

        NextTurn();
    }

    public delegate void NextTurnFunction();
    public static NextTurnFunction nextTurnFunction;

    void NextTurn()
    {
        turn++;
        if (nextTurnFunction != null)
            nextTurnFunction();
    }

    public delegate void PlayerEndFunction();
    public static PlayerEndFunction playerEndFunction;

    IEnumerator PlayerEnd()
    {
        allUnitMove = true;

        foreach (Unit unit in currentPlayer.unitList)
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
        {
            if (playerEndFunction != null)
                playerEndFunction();

            PlayerStart();
        }
            
    }

    public delegate void PlayerStartFunction();
    public static PlayerStartFunction playerStartFunction;

    void PlayerStart()
    {
        playerStartFunction -= currentPlayer.playerStartFunction;
        playerEndFunction -= currentPlayer.playerEndFunction;

        int nextPlayerIndex = playerList.IndexOf(currentPlayer) + 1;
        if (nextPlayerIndex == playerList.Count)
        {
            NextTurn();
            nextPlayerIndex = 0;
        }

        currentPlayer = playerList[nextPlayerIndex];
        playerStartFunction += currentPlayer.playerStartFunction;
        playerEndFunction += currentPlayer.playerEndFunction;

        Debug.Log(currentPlayer.name + " Start");

        movingUnitList.Clear();
        activeUnitList.Clear();

        foreach (Unit unit in currentPlayer.unitList)
        {
            unit.remainMove = unit.template.property.speed; //reset unit remain speed

            if (unit.isMoving)
                movingUnitList.Add(unit);
            else
                activeUnitList.Add(unit);
        }

        if(playerStartFunction != null)
            playerStartFunction();

        //Testing
        if(currentPlayer.GetType() == typeof(AI_Player))
            AIAutoEnd();
        else if(currentPlayer.GetType() == typeof(HumanPlayer))
            cameraScirpt.MoveCamera(currentPlayer.unitList[0].currentPos);
    }

    //Turn Button Funciton
    IEnumerator endPlayerCoroutine;

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

            endPlayerCoroutine = PlayerEnd();
            StartCoroutine(endPlayerCoroutine);
            StopCoroutine(endPlayerCoroutine);
        }
    }

    public void NextUnit()
    {
        HumanPlayer player = (HumanPlayer)currentPlayer;

        player.selectedBuilding = null;
        UI.Disable(UI.buildingUI);

        Debug.Log(activeUnitList.Count);
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
