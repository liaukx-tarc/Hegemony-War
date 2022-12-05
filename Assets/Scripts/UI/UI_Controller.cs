using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UI_Controller;
using static UnityEngine.UI.CanvasScaler;

public class UI_Controller : MonoBehaviour
{
    //Input
    const string cancel = "Cancel";

    //Canvas
    [SerializeField]
    public static Canvas canvas;

    //Building UI
    [Header("Building UI")]
    public GameObject buildingUI;
    public static UI_Building buildingUIController;
    public TextMeshProUGUI buildingName;

    //Delegate
    public delegate void CloseAllUIFunction();
    public static CloseAllUIFunction closeAllUIFunction;

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        buildingUIController = buildingUI.GetComponent<UI_Building>();
        UnitUIStart();
        UD_Start();
        unitTemplateListUI.UnitTemplateUI_Start();
    }

    public void Update()
    {
        if (Input.GetButton(cancel))
            UD_Close();
    }

    private void FixedUpdate()
    {
        UnitUIUpdate();
    }

    //Unit UI
    [Header("Unit UI")]
    public GameObject unitUI;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI lifeVariable;
    public TextMeshProUGUI armorVariable;
    public TextMeshProUGUI damageVariable;
    public TextMeshProUGUI rangeVariable;
    public TextMeshProUGUI speedVariable;

    public Unit showingUnit;
    public bool unitUIOn = false;

    public void UnitUIStart()
    {
        closeAllUIFunction += CloseUnitUI;
    }

    public void OpenUnitUI(Unit unit)
    {
        showingUnit = unit;
        unitUIOn = true;
        unitUI.SetActive(true);
    }

    public void CloseUnitUI()
    {
        showingUnit = null;
        unitUIOn = false;
        unitUI.SetActive(false);
    }

    void UnitUIUpdate()
    {
        if(unitUIOn)
        {
            unitName.text = showingUnit.name;
            lifeVariable.text = showingUnit.currentHp.ToString() + " / " + showingUnit.property.maxHp.ToString();
            armorVariable.text = showingUnit.property.armor.ToString();
            damageVariable.text = showingUnit.damage.ToString();
            rangeVariable.text = "0";
            speedVariable.text = showingUnit.remainMove.ToString();
        }
    }

    public void UnitSkip()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.currentPlayer;
            if (PlayerController.selectedUnit != null)
            {
                PlayerController.selectedUnit.isAction = true;
                WorldController.activeUnitList.Remove(PlayerController.selectedUnit);
                GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>().NextUnit();
            }
        }
    } //Unit Skip Button

    public void UnitMove()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.currentPlayer;
            PlayerController.isMoving = true;
        }
    } //Unit Move Button

    public void DestroyUnit()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.currentPlayer;

            humanPlayer.unitList.Remove(PlayerController.selectedUnit);
            PlayerController.selectedUnit.currentPos.unitsList.Remove(PlayerController.selectedUnit);
            Destroy(PlayerController.selectedUnit.gameObject);
            PlayerController.selectedUnit = null;
            WorldController.UI.CloseUnitUI();
        }
    } //Unit Destroy Button

    public void UnitBuilding()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.currentPlayer;
            PlayerController.isBuilding= true;
            Debug.Log("Building Testing");
            PlayerController.Building.buildingRange = 2;
        }
            
    } //Unit Build city Button


    //Lens
    [Header("Lens")]
    public GameObject lens1;
    public GameObject lens2;
    public GameObject lens3;
    bool lenState = false;

    public void lensBtn()
    {
        lenState = !lenState;
        lens1.SetActive(lenState);
        lens2.SetActive(lenState);
        lens3.SetActive(lenState);
    }

    //Unit Development
    [Header("Unit Development")]
    public RectTransform UD_Transform;
    public TextMeshProUGUI UD_title;
    Vector2 UD_Position;

    //Uint Development Progress
    const string UDProgressTitle = "Unit Development Progress";
    public GameObject progressUIObj;
    public static UI_Progress progressUI;

    //Accessories Eqiup
    const string UnitDevelopmentTitle = "Unit Development";
    const string UnitModifyTitle = "Unit Modify";
    const string UnitUpgradeTitle = "Unit Upgrade";
    public GameObject accessoriesEquipUIObj;
    public static UI_AccessoriesEquip accessoriesUI;
    public static TransportType UD_transportType;

    //Unit Template UI
    public static UI_UnitTemplateList unitTemplateListUI;
    public GameObject unitTemplateListObj;

    void UD_Start()
    {
        accessoriesUI = accessoriesEquipUIObj.GetComponentInChildren<UI_AccessoriesEquip>();
        progressUI = progressUIObj.GetComponentInChildren<UI_Progress>();
        unitTemplateListUI = unitTemplateListObj.GetComponent<UI_UnitTemplateList>();
        accessoriesUI.UI_Start();

        UD_Position = UD_Transform.anchoredPosition;

        WorldController.playerStartFunction += progressUI.ActivePlayerProject;
        WorldController.playerEndFunction += progressUI.DisablePlayerProject;

        closeAllUIFunction += UD_Close;

        Debug.Log("Testing");
        UD_transportType = TransportType.Vechicle;
    }

    public void UD_Close()
    {
        progressUIObj.SetActive(false);
        accessoriesUI.gameObject.SetActive(false);

        UD_Transform.gameObject.SetActive(false);
    }

    public void OpenUDProgressUI()
    {
        closeAllUIFunction();

        UD_Transform.gameObject.SetActive(true);
        progressUI.gameObject.SetActive(true);

        progressUI.infoPanel.SetActive(false);
        selectedUnitTemplateProperty = null;

        UD_Transform.anchoredPosition = UD_Position;
        UD_title.text = UDProgressTitle;
    }

    public void OpenUnitDevelopmentUI()
    {
        closeAllUIFunction();

        UD_Transform.gameObject.SetActive(true);
        accessoriesUI.gameObject.SetActive(true);

        UD_Transform.anchoredPosition = UD_Position;
        UD_title.text = UnitDevelopmentTitle;

        accessoriesUI.UIOpen(ProjectType.UnitDevelopment, UD_transportType, null);
    }

    public void OpenUnitModifyUI()
    {
        closeAllUIFunction();

        UD_Transform.gameObject.SetActive(true);

        UD_Transform.anchoredPosition = UD_Position;
        UD_title.text = UnitModifyTitle;

        //Open Unit Template Select UI
    }

    public void OpenUnitUpgradeUI()
    {
        closeAllUIFunction();

        UD_Transform.gameObject.SetActive(true);

        UD_Transform.anchoredPosition = UD_Position;
        UD_title.text = UnitUpgradeTitle;

        //Open Unit Template Select UI
    }

    public static UnitProperty selectedUnitTemplateProperty;

    public void OpenTemplateDetail()
    {
        progressUI.gameObject.SetActive(false);
        accessoriesUI.gameObject.SetActive(true);

        accessoriesUI.showTemplateDetail(selectedUnitTemplateProperty);
    }
}
