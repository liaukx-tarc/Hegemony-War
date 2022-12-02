using System.Collections;
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

    //Building UI
    [Header("Building UI")]
    public GameObject buildingUI;
    public TextMeshProUGUI buildingName;

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        UD_Start();
    }

    public void Update()
    {
        if (Input.GetButton(cancel))
            UD_Close();
    }

    //Enable UI component
    public void Enable(GameObject ui)
    {
        ui.SetActive(true);
    }

    //Disable UI component
    public void Disable(GameObject ui)
    {
        ui.SetActive(false);
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

    public void showUnit(string name, int maxHp, int currentHp, int armor, int damage, float remainMovement)
    {
        unitName.text = name;
        lifeVariable.text = currentHp.ToString() + " / " + maxHp.ToString(); 
        armorVariable.text = armor.ToString();
        damageVariable.text = damage.ToString();
        rangeVariable.text = "0";
        speedVariable.text = remainMovement.ToString();
    }

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
    public static bool UD_State = false;
    Vector2 UD_Position;

    //Uint Development Progress
    const string UDProgressTitle = "Unit Development Progress";
    public GameObject progressUI;

    //Accessories Eqiup
    const string UnitDevelopmentTitle = "Unit Development";
    const string UnitModifyTitle = "Unit Modify";
    const string UnitUpgradeTitle = "Unit Upgrade";
    public GameObject accessoriesEquipUI;
    public static UI_AccessoriesEquip accessoriesUI;
    public static TransportType UD_transportType;

    void UD_Start()
    {
        accessoriesUI = accessoriesEquipUI.GetComponentInChildren<UI_AccessoriesEquip>();
        accessoriesUI.UI_Start();

        UD_Position = UD_Transform.anchoredPosition;

        Debug.Log("Testing");
        UD_transportType = TransportType.Vechicle;
    }

    public void UD_Close()
    {
        UD_State = !UD_State;

        progressUI.SetActive(UD_State);
        accessoriesUI.gameObject.SetActive(UD_State);

        UD_Transform.gameObject.SetActive(UD_State);
    }

    public void OpenUDProgressUI()
    {
        accessoriesUI.gameObject.SetActive(false);

        UD_State = !UD_State;
        UD_Transform.gameObject.SetActive(UD_State);
        progressUI.gameObject.SetActive(UD_State);

        if (UD_State)
        {
            UD_Transform.anchoredPosition = UD_Position;
            UD_title.text = UDProgressTitle;
        }
    }

    public void OpenUnitDevelopmentUI()
    {
        UD_State = !UD_State;
        UD_Transform.gameObject.SetActive(UD_State);
        accessoriesUI.gameObject.SetActive(UD_State);

        if (UD_State)
        {
            UD_Transform.anchoredPosition = UD_Position;
            UD_title.text = UnitDevelopmentTitle;
        }

        accessoriesUI.UIOpen(ProjectType.UnitDevelopment, UD_transportType, null);
    }

    public void OpenUnitModifyUI()
    {
        UD_State = !UD_State;
        UD_Transform.gameObject.SetActive(UD_State);

        if (UD_State)
        {
            UD_Transform.anchoredPosition = UD_Position;
            UD_title.text = UnitModifyTitle;
        }

        //Open Unit Template Select UI
    }    

    public void OpenUnitUpgradeUI()
    {
        UD_State = !UD_State;
        UD_Transform.gameObject.SetActive(UD_State);

        if (UD_State)
        {
            UD_Transform.anchoredPosition = UD_Position;
            UD_title.text = UnitUpgradeTitle;
        }

        //Open Unit Template Select UI
    }
}
