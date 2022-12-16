using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    static public int turn;
    //Generator
    public MapCreate mapCreater;
    public PlayerCreate playerCreater;
    public UnitSpawn unitSpawner;

    //Controller
    static public PlayerController playerController;
    static public BuildingController buildingController;
    static public UnitController unitController;

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
        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        buildingController = GameObject.FindGameObjectWithTag("BuildingController").GetComponent<BuildingController>();
        unitController = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
        nextTurnFunction += turnButton.ChangeTurnText;
        playerStartFunction += CalcukateResource;
        InitializeWorld();
    }

    public List<Unit> testingList;
    private void Update()
    {
        testingList = activeUnitList;
    }

    void InitializeWorld()
    {
        mapCreater.GenerateWorld();
        playerCreater.CreatePlayer();
        unitSpawner.GenerateUnit();
        Destroy(unitSpawner.gameObject);
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
            unit.remainMove = unit.property.speed; //reset unit remain speed

            if (unit.isAutoMove)
                movingUnitList.Add(unit);
            else
                activeUnitList.Add(unit);
        }

        if (playerStartFunction != null)
            playerStartFunction();

        StartCoroutine(ChangePlayerAnimation());

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
    public static bool autoUnitArrive = false;
    public static Unit arriveUnit;

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

        if (autoUnitArrive)
        {
            Debug.Log("Arrive");
            autoUnitArrive = false;
            cameraScirpt.StartCamera(arriveUnit.currentPos);
            yield break;
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

    int nextPlayerIndex;

    void PlayerStart()
    {
        playerStartFunction -= currentPlayer.playerStartFunction;
        playerEndFunction -= currentPlayer.playerEndFunction;

        nextPlayerIndex = playerList.IndexOf(currentPlayer) + 1;
        if (nextPlayerIndex == playerList.Count)
        {
            NextTurn();
            nextPlayerIndex = 0;
        }

        currentPlayer = playerList[nextPlayerIndex];

        currentPlayer.UpdateResource();
        playerStartFunction += currentPlayer.playerStartFunction;
        playerEndFunction += currentPlayer.playerEndFunction;

        Debug.Log(currentPlayer.name + " Start");

        movingUnitList.Clear();
        activeUnitList.Clear();

        foreach (Unit unit in currentPlayer.unitList)
        {
            unit.remainMove = unit.property.speed; //reset unit remain speed
            unit.isAction = false;

            if (unit.isSleep)
                unit.isAction = true;

            else if (unit.isAutoMove)
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
        {
            StartCoroutine(ChangePlayerAnimation());

            if (activeUnitList.Count > 0)
            {
                PlayerController.selectedUnit = activeUnitList[0];
                UI.OpenUnitUI(PlayerController.selectedUnit);
                cameraScirpt.MoveCamera(activeUnitList[0].currentPos);
            }

            else
            {
                UI.CloseUnitUI();
                playerController.CancelUnitSelect();
            }
        }
            
    }

    public void CalcukateResource()
    {
        currentPlayer.money += currentPlayer.moneyIncome;
    }

    public void TurnBtn()
    {
        if (activeUnitList.Count != 0) 
        {
            NextUnit();
        }

        else
        {
            Debug.Log("Turn End Button Down");
            foreach (Unit unit in currentPlayer.unitList)
            {
                if(!unit.isAction)
                {
                    cameraScirpt.MoveCamera(movingUnitList[0].currentPos);
                    UI.CloseAllUI();
                    PlayerController.selectedUnit = null;
                    break;
                }
            }

            foreach (Unit unit in movingUnitList)
            {
                unit.startMove = true;
            }

            StopAllCoroutines();
            StartCoroutine(PlayerEnd());
        }
    }

    public void NextUnit()
    {
        if (playerController.NextUnit(activeUnitList))
            cameraScirpt.MoveCamera(PlayerController.selectedUnit.currentPos);
    }

    public GameObject changePlayerScene;
    public TextMeshProUGUI playerText;
    public float animationTime;

    IEnumerator ChangePlayerAnimation()
    {
        changePlayerScene.SetActive(true);
        playerText.text = currentPlayer.name;
        yield return new WaitForSeconds(animationTime);
        changePlayerScene.SetActive(false);
    }
}
