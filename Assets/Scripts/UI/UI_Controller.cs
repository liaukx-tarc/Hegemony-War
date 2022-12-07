using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    //Input
    const string cancel = "Cancel";

    //Canvas
    [SerializeField]
    public static Canvas canvas;
    public static bool isUIOpen = false;

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

        closeAllUIFunction += DisableScreenBlock;
        WorldController.playerEndFunction += CloseAllUI;
    }

    public void Update()
    {
        if (Input.GetButton(cancel))
            closeAllUIFunction();
    }

    private void FixedUpdate()
    {
        UnitUIUpdate();
    }

    public void CloseAllUI()
    {
        closeAllUIFunction();
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

    //Unit UI
    [Header("Unit UI")]
    public GameObject unitUI;

    public Image iconBackground;
    public Image icon;
    public Color infantryColor;
    public Color vechicleColor;
    public Color aircarftColor;
    public Color shipColor;

    public TextMeshProUGUI unitName;
    public TextMeshProUGUI lifeVariable;
    public TextMeshProUGUI armorVariable;
    public TextMeshProUGUI damageVariable;
    public TextMeshProUGUI rangeVariable;
    public TextMeshProUGUI speedVariable;
    public TextMeshProUGUI maintainCostVariable;

    public Unit showingUnit;
    public bool unitUIOn = false;

    public void UnitUIStart()
    {
        closeAllUIFunction += CloseUnitUI;
    }

    public void OpenUnitUI(Unit unit)
    {
        showingUnit = unit;
        
        switch (unit.property.transportProperty.transportType)
        {
            case TransportType.Infantry:
                iconBackground.color = infantryColor;
                break;

            case TransportType.Vechicle:
                iconBackground.color = vechicleColor;
                break;

            case TransportType.Aircarft:
                iconBackground.color = aircarftColor;
                break;

            case TransportType.Ship:
                iconBackground.color = shipColor;
                break;
        }

        icon.sprite = unit.property.unitIcon;
        
        unitName.text = showingUnit.property.unitName;
        maintainCostVariable.text = showingUnit.property.maintanceCost.ToString();
        ChangeTag(showingUnit.property);

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

        unitTagsList.Sort(UI_AccessoriesEquip.CompareListByUnitTag);
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
            WorldController.activeUnitList.Remove(PlayerController.selectedUnit);
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

    //Building UI
    [Header("Building UI")]
    public GameObject buildingUI;
    public static UI_Building buildingUIController;
    public TextMeshProUGUI buildingName;

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
        DisableScreenBlock();

        UD_Transform.gameObject.SetActive(false);
    }

    public void OpenUDProgressUI()
    {
        closeAllUIFunction();
        EnableScreenBlock();

        UD_Transform.gameObject.SetActive(true);
        progressUI.gameObject.SetActive(true);

        progressUI.infoPanel.SetActive(false);
        selectedUnitTemplateProperty = null;

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

    public void OpenUnitDevelopmentUI()
    {
        OpenAccessoriesUI();
        UD_title.text = UnitDevelopmentTitle;

        accessoriesUI.UIOpen(ProjectType.UnitDevelopment, UD_transportType, null);
    }

    public void OpenUnitModifyUI()
    {
        closeAllUIFunction();
        UD_title.text = UnitModifyTitle;

        unitTemplateListUI.isModify = true;
        unitTemplateListUI.OpenTemplateUI();
    }

    public void OpenUnitUpgradeUI()
    {
        closeAllUIFunction();
        UD_title.text = UnitUpgradeTitle;

        unitTemplateListUI.isUpgrade = true;
        unitTemplateListUI.OpenTemplateUI();
    }

    public static UnitProperty selectedUnitTemplateProperty;

    public void OpenTemplateDetail()
    {
        EnableScreenBlock();

        progressUI.gameObject.SetActive(false);
        accessoriesUI.gameObject.SetActive(true);

        accessoriesUI.showTemplateDetail(selectedUnitTemplateProperty);
    }
}
