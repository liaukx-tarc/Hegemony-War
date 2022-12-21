using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    //Input
    const string cancel = "Cancel";

    //Sound
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;

    //Canvas
    [SerializeField]
    public Canvas canvas;
    public bool isUIOpen = false;

    //Delegate
    public delegate void CloseAllUIFunction();
    public CloseAllUIFunction closeAllUIFunction;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        buildingUIController = buildingUI.GetComponent<UI_Building>();
        UD_Awake();
    }

    void Start()
    {
        buildingUIController.BuildingUI_Start();
        UnitUIStart();
        UD_Start();
        unitTemplateListUI.UnitTemplateUI_Start();
        closeAllUIFunction += CloseMenu;
        closeAllUIFunction += DisableScreenBlock;
        WorldController.instance.playerEndFunction += CloseAllUI;
    }

    bool isEscape = false;

    public void Update()
    {
        if (Input.GetButton(cancel) && !isEscape)
        {
            if(isUIOpen)
            {
                closeAllUIFunction();
            }

            else
            {
                OpenMenu();
            }

            isEscape = true;
            StartCoroutine(EescapeCooldown());
        }
            
    }

    IEnumerator EescapeCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        isEscape = false;
    }

    private void FixedUpdate()
    {
        if(WorldController.instance.currentPlayer != null)
        {
            ResourceUpdate();
            UnitUIUpdate();
        }
    }

    public void CloseAllUI()
    {
        closeAllUIFunction();
    }

    //Click Sound
    public void ClickSound()
    {
        uiAudioSource.Stop();
        uiAudioSource.clip = buttonClickSound;
        uiAudioSource.Play();
    }

    //Screen Block
    public GameObject screenBlock;

    public void EnableScreenBlock()
    {
        screenBlock.SetActive(true);
        isUIOpen = true;
    }

    public void DisableScreenBlock()
    {
        screenBlock.SetActive(false);
        isUIOpen = false;
    }

    //Top Bar UI
    [Header("Player Name")]
    public Image playerNameBackground;
    public TextMeshProUGUI playerNameText;

    public void ChangePlayerName(Player player)
    {
        playerNameBackground.color = player.playerColor;
        playerNameText.text = player.playerName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameBackground.rectTransform);
    }


    [Header("Resource UI")]
    public RectTransform resourcePanel;
    public RectTransform money;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI moneyIncomeText;
    public RectTransform sciencePoint;
    public TextMeshProUGUI sciencePointText;
    public Color increaseColor;
    public Color decreaseColor;

    const string Plus = "+";

    public void ResourceUpdate()
    {
        WorldController.instance.currentPlayer.UpdateResource();

        moneyText.text = WorldController.instance.currentPlayer.money.ToString();
        
        if (WorldController.instance.currentPlayer.moneyIncome >= 0)
        {
            moneyIncomeText.text = Plus + WorldController.instance.currentPlayer.moneyIncome.ToString();
            moneyIncomeText.color = increaseColor;
        }
        else
        {
            moneyIncomeText.text = WorldController.instance.currentPlayer.moneyIncome.ToString();
            moneyIncomeText.color = decreaseColor;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(money);

        sciencePointText.text = WorldController.instance.currentPlayer.sciencePoint.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(sciencePoint);

        LayoutRebuilder.ForceRebuildLayoutImmediate(resourcePanel);
    }

    //Unit UI
    [Header("Unit UI")]
    public GameObject unitUI;
    public GameObject buttonList;

    [Header("Info")]
    public Image unitNameBackground;
    public Image unitIconBackground;
    public Image unitIcon;
    public Color vechicleColor;
    public Color aircarftColor;
    public Color shipColor;
    public Color whiteColor;

    [Header("Unit state Text")]
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI lifeVariable;
    public TextMeshProUGUI armorVariable;
    public TextMeshProUGUI damageVariable;
    public TextMeshProUGUI rangeVariable;
    public TextMeshProUGUI speedVariable;
    public TextMeshProUGUI maintainCostVariable;

    [Header("Unit UI Button")]
    public GameObject buildButton;
    public GameObject sleepButton;
    public GameObject wakeupButton;

    [Space]
    public Unit showingUnit;
    public bool unitUIOn = false;

    public void UnitUIStart()
    {
        closeAllUIFunction += CloseUnitUI;
    }

    public void OpenUnitUI(Unit unit)
    {
        showingUnit = unit;

        unitNameBackground.color = unit.player.playerColor;

        switch (unit.property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                unitIconBackground.color = vechicleColor;
                break;

            case TransportType.Aircarft:
                unitIconBackground.color = aircarftColor;
                break;

            case TransportType.Ship:
                unitIconBackground.color = shipColor;
                break;
        }

        unitIcon.sprite = unit.property.unitIcon;
        
        unitName.text = showingUnit.property.unitName;
        maintainCostVariable.text = showingUnit.property.maintanceCost.ToString();
        ChangeTag(showingUnit.property);

        if (unit.player == WorldController.instance.currentPlayer)
        {
            buttonList.SetActive(true);

            if (!unit.property.isSettler)
            {
                buildButton.SetActive(false);
            }

            else
            {
                buildButton.SetActive(true);
            }

            if (unit.isSleep)
            {
                sleepButton.SetActive(false);
                wakeupButton.SetActive(true);
            }

            else
            {
                wakeupButton.SetActive(false);
                sleepButton.SetActive(true);
            }
        }

        else
        {
            buttonList.SetActive(false);
        }

        unitUIOn = true;
        unitUI.SetActive(true);
    }

    public void CloseUnitUI()
    {
        showingUnit = null;
        WorldController.instance.playerController.DisablePathShow();
        unitUIOn = false;
        unitUI.SetActive(false);
    }

    void UnitUIUpdate()
    {
        if(unitUIOn)
        {
            lifeVariable.text = showingUnit.currentHp.ToString() + " / " + showingUnit.property.maxHp.ToString();
            armorVariable.text = showingUnit.property.armor.ToString();
            damageVariable.text = showingUnit.damage.ToString();
            rangeVariable.text = showingUnit.property.range.ToString();
            speedVariable.text = showingUnit.remainMove.ToString();
        }
    }

    public List<TextMeshProUGUI> tagTextList;

    void ChangeTag(UnitProperty unitProperty)
    {
        List<UnitTag> unitTagsList = new List<UnitTag>();

        foreach (AccessoryProperty accessory in unitProperty.accessoryProperty)
        {
            if (accessory != null && !unitTagsList.Contains(accessory.accessoryTag) && accessory.accessoryTag != UnitTag.None)
            {
                unitTagsList.Add(accessory.accessoryTag);
            }
        }

        unitTagsList.Sort(accessoriesUI.CompareListByUnitTag);
        for (int i = 0; i < tagTextList.Count; i++)
        {
            if (i < unitTagsList.Count)
            {
                tagTextList[i].text = unitTagsList[i].ToString();
                tagTextList[i].transform.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tagTextList[0].transform.parent.GetComponent<RectTransform>());
            }

            else
            {
                tagTextList[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    //Unit Build city Button
    public void UnitBuildCity()
    {
        ClickSound();
        if (WorldController.instance.playerController.selectedUnit.cityModel != null)
            Destroy(WorldController.instance.playerController.selectedUnit.cityModel);

        WorldController.instance.playerController.isMoving = false;
        WorldController.instance.playerController.BuildCity();
    }

    //Unit Move Button
    public void UnitMove()
    {
        ClickSound();
        WorldController.instance.playerController.isBuildingCity = false;
        WorldController.instance.playerController.isMoving = true;
    }

    //Unit Skip Button
    public void UnitSkip()
    {
        ClickSound();
        WorldController.instance.playerController.isMoving = WorldController.instance.playerController.isBuildingCity = false;
        WorldController.instance.playerController.selectedUnit.isAction = true;
        WorldController.instance.activeUnitList.Remove(WorldController.instance.playerController.selectedUnit);
        WorldController.instance.NextUnit();
    }

   //Unit Sleep Button
   public void UnitSleep()
    {
        ClickSound();
        WorldController.instance.playerController.isMoving = WorldController.instance.playerController.isBuildingCity = false;
        WorldController.instance.playerController.selectedUnit.isAction = true;
        WorldController.instance.playerController.selectedUnit.isSleep = true;
        WorldController.instance.activeUnitList.Remove(WorldController.instance.playerController.selectedUnit);
        CloseUnitUI();
    }

    //Unit Wake up Button
    public void UnitWakeup()
    {
        ClickSound();
        WorldController.instance.playerController.selectedUnit.isSleep = false;
        WorldController.instance.activeUnitList.Add(WorldController.instance.playerController.selectedUnit);
        sleepButton.SetActive(true);
        wakeupButton.SetActive(false);
    }

    //Unit Destroy Button
    public void DestroyUnit()
    {
        ClickSound();
        if (WorldController.instance.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.instance.currentPlayer;

            WorldController.instance.playerController.selectedUnit.UnitDestroy();
        }
    } 

    //Building UI
    [Header("Building UI")]
    public GameObject buildingUI;
    public UI_Building buildingUIController;
    public TextMeshProUGUI buildingName;

    ////Lens
    //[Header("Lens")]
    //public GameObject lens1;
    //public GameObject lens2;
    //public GameObject lens3;
    //bool lenState = false;

    //public void lensBtn()
    //{
    //    lenState = !lenState;
    //    lens1.SetActive(lenState);
    //    lens2.SetActive(lenState);
    //    lens3.SetActive(lenState);
    //}

    //Unit Development
    [Header("Unit Development")]
    public RectTransform UD_Transform;
    public TextMeshProUGUI UD_title;
    Vector2 UD_Position;

    //Uint Development Progress
    const string UDProgressTitle = "Unit Development Progress";
    public GameObject progressUIObj;
    public UI_Progress progressUI;

    //Accessories Eqiup
    public GameObject accessoriesEquipUIObj;
    public UI_AccessoriesEquip accessoriesUI;

    //Unit Template UI
    public UI_UnitTemplateList unitTemplateListUI;
    public GameObject unitTemplateListObj;

    void UD_Awake()
    {
        accessoriesUI = accessoriesEquipUIObj.GetComponentInChildren<UI_AccessoriesEquip>();
        progressUI = progressUIObj.GetComponentInChildren<UI_Progress>();
        unitTemplateListUI = unitTemplateListObj.GetComponent<UI_UnitTemplateList>();
    }

    void UD_Start()
    {
        accessoriesUI.UI_Start();

        UD_Position = UD_Transform.anchoredPosition;

        WorldController.instance.playerStartFunction += progressUI.ActivePlayerProject;
        WorldController.instance.playerEndFunction += progressUI.DisablePlayerProject;

        closeAllUIFunction += UD_Close;
    }

    public void UD_Close()
    {
        progressUIObj.SetActive(false);
        accessoriesUI.gameObject.SetActive(false);
        DisableScreenBlock();

        UD_Transform.gameObject.SetActive(false);
    }

    public void OpenUDProgressUI()
    {
        ClickSound();
        closeAllUIFunction();

        EnableScreenBlock();

        UD_Transform.gameObject.SetActive(true);
        progressUI.OpenProgressUI();

        UD_Transform.anchoredPosition = UD_Position;
        UD_title.text = UDProgressTitle;
    }

    public void OpenAccessoriesUI()
    {
        closeAllUIFunction();

        EnableScreenBlock();

        accessoriesUI.gameObject.SetActive(true);
        UD_Transform.gameObject.SetActive(true);
        UD_Transform.anchoredPosition = UD_Position;
    }

    //Menu
    [Header("Menu")]
    public GameObject menuObj;

    public void OpenMenu()
    {
        ClickSound();
        CloseAllUI();
        EnableScreenBlock();
        menuObj.SetActive(true);
    }

    public void CloseMenu()
    {
        DisableScreenBlock();
        menuObj.SetActive(false);
    }

    //Lose Scene
    [Header("Lose")]
    public TextMeshProUGUI lostPlayerName;
    public Image lostPlayerBackground;

    public void PlayerLost(Player player)
    {
        lostPlayerName.text = player.playerName;
        lostPlayerBackground.color = player.playerColor;
    }

    //Win Scene
    [Header("Win")]
    public Image endScene;
    public TextMeshProUGUI winPlayerName;
    public Image winPlayerBackground;
    public Animator victoryAnimator;
    public Animator drawAnimator;
    public GameObject background;
    public GameObject menuButton;

    bool isWin = false;
    bool isDraw = false;

    public void PlayerWin(Player player)
    {
        endScene.gameObject.SetActive(true);
        winPlayerName.text = player.playerName;
        winPlayerBackground.color = player.playerColor;
        isWin = true;
        StartCoroutine(EndSceneAnimation());
    }

    public void PlayerDraw()
    {
        endScene.gameObject.SetActive(true);
        isDraw = true;
        StartCoroutine(EndSceneAnimation());
    }

    public float sceneAlpha;
    public float animationTime;

    IEnumerator EndSceneAnimation()
    {
        do
        { 
            endScene.color += new Color(0, 0, 0, sceneAlpha);
            yield return new WaitForFixedUpdate();
        } while (endScene.color.a < 1);

        if(isWin)
        {
            victoryAnimator.gameObject.SetActive(true);
            victoryAnimator.SetTrigger("Play");
        }
        else if(isDraw)
        {
            drawAnimator.gameObject.SetActive(true);
            drawAnimator.SetTrigger("Play");
        }

        yield return new WaitForSeconds(animationTime);

        if(isWin)
        {
            winPlayerBackground.gameObject.SetActive(true);
            background.SetActive(true);
        }
        
        menuButton.SetActive(true);
    }

    [Header("Confrim Menu")]
    public bool isMainMenu = false;
    public GameObject confirmMenu;
    public GameObject mainMenuText;

    public void MainMenu()
    {
        isMainMenu = true;
        confirmMenu.SetActive(true);
        mainMenuText.SetActive(true);
    }

    public void Confirm()
    {
        if(isMainMenu)
        {
            isMainMenu = false;
            StartCoroutine(LoadScene("Main Menu"));
        }
    }

    public void Cancel()
    {
        confirmMenu.SetActive(false);

        if (isMainMenu)
        {
            mainMenuText.SetActive(false);
            isMainMenu = false;
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }
}
