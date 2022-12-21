using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public static WorldController instance;
    public int turn;

    //Generator
    public MapCreate mapCreater;
    public PlayerCreate playerCreater;
    public UnitSpawn unitSpawner;

    //Controller
    public PlayerController playerController;
    public BuildingController buildingController;
    public UnitController unitController;
    public UI_Controller uiController;

    //Player
    public List<Player> playerList = new List<Player>();
    public Player currentPlayer;
    
    //Map
    public MapCell[,] map;
    public Vector2 mapSize;   

    //Turn Button
    public TurnButton turnButton;

    //Unit List
    public List<Unit> activeUnitList = new List<Unit>();
    public List<Unit> movingUnitList = new List<Unit>();

    public CameraControl cameraScirpt;

    public GameObject worldInit;

    private void Awake()
    {
        instance = this;
        uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Controller>();
        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        buildingController = GameObject.FindGameObjectWithTag("BuildingController").GetComponent<BuildingController>();
        unitController = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        turn = 0;
        nextTurnFunction += turnButton.ChangeTurnText;
        InitializeWorld();
    }

    void InitializeWorld()
    {
        mapCreater.GenerateWorld();
        playerCreater.CreatePlayer();
        unitSpawner.GenerateUnit();
        Destroy(unitSpawner.gameObject);
        GameStart();
    }

    void GameStart()
    {
        currentPlayer = playerList[0];
        nextPlayerIndex = 0;
        playerStartFunction += currentPlayer.playerStartFunction;
        playerEndFunction += currentPlayer.playerEndFunction;
        uiController.ChangePlayerName(currentPlayer);

        cameraScirpt.StartCamera(currentPlayer.unitList[0].currentPos);

        movingUnitList.Clear();
        activeUnitList.Clear();

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
    public NextTurnFunction nextTurnFunction;

    void NextTurn()
    {
        turn++;
        if (nextTurnFunction != null)
            nextTurnFunction();
    }

    public delegate void PlayerEndFunction();
    public PlayerEndFunction playerEndFunction;

    public delegate void PlayerStartFunction();
    public PlayerStartFunction playerStartFunction;

    public int nextPlayerIndex = 0;

    void PlayerStart()
    {
        movingUnitList.Clear();
        activeUnitList.Clear();

        playerStartFunction -= currentPlayer.playerStartFunction;
        playerEndFunction -= currentPlayer.playerEndFunction;

        nextPlayerIndex++;
        if (nextPlayerIndex >= playerList.Count)
        {
            NextTurn();
            nextPlayerIndex = 0;
        }

        currentPlayer = playerList[nextPlayerIndex];

        uiController.ChangePlayerName(currentPlayer);
        
        playerStartFunction += currentPlayer.playerStartFunction;
        playerEndFunction += currentPlayer.playerEndFunction;

        Debug.Log(currentPlayer.name + " Start");

        foreach (Unit unit in currentPlayer.unitList)
        {
            unit.TurnStart();

            if (unit.isSleep)
                unit.isAction = true;

            else if (unit.isAutoMove)
                movingUnitList.Add(unit);

            else
                activeUnitList.Add(unit);
        }

        if(playerStartFunction != null)
            playerStartFunction();

        //Resource Calculate
        currentPlayer.UpdateResource();
        CalcukateResource();
        CheckVictory();

        if (currentPlayer.GetType() == typeof(HumanPlayer))
        {
            StartCoroutine(ChangePlayerAnimation());

            if (activeUnitList.Count > 0)
            {
                playerController.selectedUnit = activeUnitList[0];
                uiController.OpenUnitUI(playerController.selectedUnit);
                cameraScirpt.MoveCamera(activeUnitList[0].currentPos);
            }

            else
            {
                uiController.CloseUnitUI();
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
        uiController.ClickSound();
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
                    cameraScirpt.MoveCamera(unit.currentPos);
                    uiController.CloseAllUI();
                    playerController.selectedUnit = null;
                    break;
                }
            }

            turnButton.turnBtn.interactable = false;
            StartCoroutine(StartAutoUnitMove());
        }
    }

    public bool isAutoUnitMoving = false;
    public bool isAutoUnitArrive = false;

    IEnumerator StartAutoUnitMove()
    {
        while (movingUnitList.Count > 0)
        {
            if (isAutoUnitArrive)
            {
                isAutoUnitArrive = false;
                turnButton.turnBtn.interactable = true;
                yield break;
            }

            if (!isAutoUnitMoving)
            {
                movingUnitList[0].startMove = true;
                movingUnitList[0].path.Clear();
                movingUnitList[0].CheckPath(movingUnitList[0].targetPos);
                cameraScirpt.MoveCamera(movingUnitList[0].currentPos);
                isAutoUnitMoving = true;
            }
            else
            {
                yield return null;
            }

        }

        if (isAutoUnitArrive)
        {
            isAutoUnitArrive = false;
            turnButton.turnBtn.interactable = true;
            yield break;
        }

        if (playerEndFunction != null)
            playerEndFunction();

        PlayerStart();
        turnButton.turnBtn.interactable = true;

    }

    public void NextUnit()
    {
        if (playerController.NextUnit(activeUnitList))
            cameraScirpt.MoveCamera(playerController.selectedUnit.currentPos);
    }

    public GameObject changePlayerScene;
    public TextMeshProUGUI playerText;
    public Image playerBackground;
    public float animationTime;

    IEnumerator ChangePlayerAnimation()
    {
        changePlayerScene.SetActive(true);
        playerText.text = currentPlayer.name;
        playerBackground.color = currentPlayer.playerColor;
        yield return new WaitForSeconds(animationTime);
        changePlayerScene.SetActive(false);
    }

    [Header("Win & Lose")]
    public GameObject playerDefeatObj;
    public List<Player> losePlayerList = new List<Player>();
    bool isStartLose = false;
    bool isShowingLoser = false;
    bool isCurrentLose = false;

    [Header("Lose Condition")]
    public int defeatMoney;

    public void CheckVictory()
    {
        foreach(Player player in playerList)
        {
            if((player.unitList.Count == 0 && player.cityList.Count == 0) || player.money <= defeatMoney)
            {
                losePlayerList.Add(player);
                isStartLose = false;

                if (currentPlayer == player)
                    isCurrentLose = true;
            }
        }

        

        if (losePlayerList.Count > 0 && playerList.Count > 1)
        {
            if (!isStartLose)
            {
                StartCoroutine(PlayerLost());
                isStartLose = true;
            }

            if (isCurrentLose)
            {
                foreach (Player player in losePlayerList)
                {
                    playerList.Remove(player);
                    nextPlayerIndex--;
                }

                isCurrentLose = false;
                PlayerStart();
            }
        }

        else if (playerList.Count == 1)
        {
            uiController.PlayerWin(playerList[0]);
        }

        else if (playerList.Count == 0)
        {
            uiController.PlayerDraw();
        }
        
    }

    IEnumerator PlayerLost()
    {
        do
        {
            if(!isShowingLoser)
            {
                playerDefeatObj.SetActive(true);
                uiController.PlayerLost(losePlayerList[0]);
                isShowingLoser = true;
            }

            yield return new WaitForSeconds(2);

            Destroy(losePlayerList[0].gameObject);
            isShowingLoser = false;
            losePlayerList.RemoveAt(0);

        } while (losePlayerList.Count > 0);

        playerDefeatObj.SetActive(false);
    }
}
