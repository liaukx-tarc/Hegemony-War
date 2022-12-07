using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_AccessoriesEquip : MonoBehaviour
{
    [Header("Accessory List")]
    public List<AccessoryProperty> accessoriesList;
    public GameObject accessoryListObj;

    [Header("Prefabs")]
    public GameObject accessoryObj;

    [Header("Tag Prefabs")]
    public GameObject heavyWeaponTag;
    public GameObject mediumWeaponTag;
    public GameObject lightWeaponTag;
    public GameObject defenceEquipmentTag;
    public GameObject auxiliaryEquipmentTag;
    public GameObject fireControlSystemTag;
    public GameObject engineTag;

    public List<List<AccessorySelection>> accessoriesListType;
    public List<AccessorySelection> accessoriesListAll;

    public void UI_Start()
    {
        GenerateAccessoriesList();
        InitializeAccessorySlots();
    }

    void GenerateAccessoriesList()
    {
        GameObject temp;
        GameObject typeList;
        AccessorySelection accessorySelectionScirpt;
        TextMeshProUGUI nameText;
        Image icon;

        accessoriesListType = new List<List<AccessorySelection>>();
        accessoriesListAll = new List<AccessorySelection>();

        for (int i = 0; i < Enum.GetNames(typeof(AccessoriesTypes)).Length; i++)
        {
            accessoriesListType.Add(new List<AccessorySelection>());
        }

        foreach (AccessoryProperty accessory in accessoriesList)
        {
            //create gameObject
            temp = Instantiate(accessoryObj);

            //GetComponent
            nameText = temp.GetComponentInChildren<TextMeshProUGUI>();
            icon = temp.GetComponentsInChildren<Image>()[1];
            typeList = temp.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
            accessorySelectionScirpt = temp.GetComponent<AccessorySelection>();
            accessorySelectionScirpt.property = accessory;

            //set name & icon
            temp.name = nameText.text = accessory.accessoryName;
            icon.sprite = accessory.icon;

            accessoriesListAll.Add(accessorySelectionScirpt);

            //set tag
            foreach (AccessoriesTypes type in accessory.accessoryTypes)
            {
                accessoriesListType[(int)type].Add(accessorySelectionScirpt);

                switch (type)
                {
                    case AccessoriesTypes.LightWeapon:
                        Instantiate(lightWeaponTag, typeList.transform);
                        break;
                    case AccessoriesTypes.MediumWeapon:
                        Instantiate(mediumWeaponTag, typeList.transform);
                        break;
                    case AccessoriesTypes.HeavyWeapon:
                        Instantiate(heavyWeaponTag, typeList.transform);
                        break;
                    case AccessoriesTypes.DefenceEquipment:
                        Instantiate(defenceEquipmentTag, typeList.transform);
                        break;
                    case AccessoriesTypes.AuxiliaryEquipment:
                        Instantiate(auxiliaryEquipmentTag, typeList.transform);
                        break;
                    case AccessoriesTypes.FireControlSystem:
                        Instantiate(fireControlSystemTag, typeList.transform);
                        break;
                    case AccessoriesTypes.engine:
                        Instantiate(engineTag, typeList.transform);
                        break;
                }
            }
        }

        //Sort Name
        accessoriesListAll.Sort(CompareListByName);
        for (int i = 0; i < Enum.GetNames(typeof(AccessoriesTypes)).Length; i++)
            accessoriesListType[i].Sort(CompareListByName);

        foreach (AccessorySelection accessory in accessoriesListAll)
            accessory.transform.SetParent(accessoryListObj.transform);
    }

    //Sort by name funtion
    private static int CompareListByName(AccessorySelection i1, AccessorySelection i2)
    {
        return i1.gameObject.name.CompareTo(i2.gameObject.name);
    }
    

    [Header("Accessory Slots")]
    public AccessorySlot[] heavyWeaponSlots;
    public AccessorySlot[] mediumWeaponSlots;
    public AccessorySlot[] lightWeaponSlots;
    public AccessorySlot[] defenceEquipmentSlots;
    public AccessorySlot[] auxiliaryEquipmentSlots;
    public AccessorySlot fireControlSystemSlots;
    public AccessorySlot engineSlots;

    int slotsCount = 0;

    void InitializeAccessorySlots()
    {
        foreach (AccessorySlot slot in heavyWeaponSlots)
            slotsCount = SlotIndexAssign(slot, slotsCount);

        foreach (AccessorySlot slot in mediumWeaponSlots)
            slotsCount = SlotIndexAssign(slot, slotsCount);

        foreach (AccessorySlot slot in lightWeaponSlots)
            slotsCount = SlotIndexAssign(slot, slotsCount);

        foreach (AccessorySlot slot in defenceEquipmentSlots)
            slotsCount = SlotIndexAssign(slot, slotsCount);

        foreach (AccessorySlot slot in auxiliaryEquipmentSlots)
            slotsCount = SlotIndexAssign(slot, slotsCount);

        slotsCount = SlotIndexAssign(fireControlSystemSlots, slotsCount);
        slotsCount = SlotIndexAssign(engineSlots, slotsCount);

        equipedAccessories = new AccessoryProperty[slotsCount];
    }

    int SlotIndexAssign(AccessorySlot slot, int slotsCount)
    {
        slot.slotIndex = slotsCount;
        return ++slotsCount;
    }

    //Open UI
    public ProjectType projectType;
    public UnitTemplate originalTemplate;
    public Building building;

    public TextMeshProUGUI createBtnText;
    public Image createBtnBackground;
    public Color createColor;
    public Color modifyColor;
    public Color upgradeColor;
    const string Create = "Create";
    const string Modify = "Modify";
    const string Upgrade = "Upgrade";

    public static bool accessorySlotState = true;


    public void UIOpen(ProjectType projectType, TransportType transportType, UnitTemplate originalTemplate)
    {
        this.projectType = projectType;

        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                originalTemplate = null;

                accessorySlotState = true;
                transportDropdown.dropdown.interactable = true;
                transportDropdown.onTransportTypeChange(transportType);
                ChangeAccessoriesSelect(transportType, true);
                createBtnBackground.color = createColor;
                createBtnText.text = Create;

                unitName = unitNameInput.text = WorldController.currentPlayer.name + " Unit " + WorldController.currentPlayer.projectCount; ;
                unitNameInput.interactable = true;
                break;

            case ProjectType.UnitModify:
                this.originalTemplate = originalTemplate;

                accessorySlotState = true;
                transportDropdown.dropdown.interactable = false;
                transportDropdown.text.text = originalTemplate.property.transportProperty.transportName;
                ChangeTransport(originalTemplate.property.transportProperty);
                ChangeAccessoriesSelect(transportType, true);
                enquipOriginalAccessories(originalTemplate.property.accessoryProperty);
                createBtnBackground.color = modifyColor;
                createBtnText.text = Modify;

                unitName = unitNameInput.text = originalTemplate.property.unitName;
                unitNameInput.interactable = true;
                break;

            case ProjectType.UnitUpgrade:
                this.originalTemplate = originalTemplate;
                
                accessorySlotState = false;
                transportDropdown.dropdown.interactable = false;
                transportDropdown.text.text = originalTemplate.property.transportProperty.transportName;
                ChangeTransport(originalTemplate.property.transportProperty);
                ChangeAccessoriesSelect(transportType, false);
                enquipOriginalAccessories(originalTemplate.property.accessoryProperty);
                createBtnBackground.color = upgradeColor;
                createBtnText.text = Upgrade;

                unitName = unitNameInput.text = originalTemplate.property.unitName;
                unitNameInput.interactable = false;
                break;
        }

        createButton.SetActive(true);
        costSelection.SetActive(true);
    }

    void enquipOriginalAccessories(AccessoryProperty[] accessoriesProperty)
    {
        for (int i = 0; i < accessoriesProperty.Length; i++)
        {
            if (accessoriesProperty[i] == null)
                continue;
            else
            {
                Debug.Log(accessoriesProperty[i].accessoryName);
            }

            if (!unitTagsList.Contains(accessoriesProperty[i].accessoryTag) && accessoriesProperty[i].accessoryTag != UnitTag.None)
                unitTagsList.Add(accessoriesProperty[i].accessoryTag);

            if (i < 3) //Heavy Weapon
                heavyWeaponSlots[i].equitAccessory(accessoriesProperty[i]);

            else if (i < 6)
                mediumWeaponSlots[i - 3].equitAccessory(accessoriesProperty[i]);

            else if (i < 9)
                lightWeaponSlots[i - 6].equitAccessory(accessoriesProperty[i]);

            else if (i < 12)
                defenceEquipmentSlots[i - 9].equitAccessory(accessoriesProperty[i]);

            else if (i < 16)
                auxiliaryEquipmentSlots[i - 12].equitAccessory(accessoriesProperty[i]);

            else if (i < 17)
                fireControlSystemSlots.equitAccessory(accessoriesProperty[i]);

            else if (i < 18)
                engineSlots.equitAccessory(accessoriesProperty[i]);
        }

        ChangeTag();
    }



    //Change Transport
    [Header("Transport")]
    public TransportDropdown transportDropdown;
    public TransportProperty transportProperty;
   
    public void ChangeAccessoriesSelect(TransportType transportType, bool accessoriesSlectState)
    {
        filterButton.SetActive(accessoriesSlectState);

        bool tagMatch;

        foreach (AccessorySelection accessorySelection in accessoriesListAll)
        {
            if (!accessoriesSlectState)
            {
                accessorySelection.gameObject.SetActive(false);
                continue;
            }

            tagMatch = false;

            foreach (TransportType transport in accessorySelection.property.transportType)
            {
                if (transport == transportType)
                {
                    accessorySelection.gameObject.SetActive(true);
                    tagMatch = true;
                    break;
                }
            }

            if (!tagMatch)
                accessorySelection.gameObject.SetActive(false);
        }
    }

    public void ChangeTransport(TransportProperty transport)
    {
        transportProperty = transport;
        unitNameInput.interactable = true;

        for (int i = 0; i < heavyWeaponSlots.Length; i++)
        {
            heavyWeaponSlots[i].GetComponent<Button>().interactable = accessorySlotState;

            if (heavyWeaponSlots[i].isEquip)
                heavyWeaponSlots[i].removeAccessory();

            if (i <= transport.heavyWeaponNum - 1)
                heavyWeaponSlots[i].gameObject.SetActive(true);
            else
                heavyWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < mediumWeaponSlots.Length; i++)
        {
            mediumWeaponSlots[i].GetComponent<Button>().interactable = accessorySlotState;

            if (mediumWeaponSlots[i].isEquip)
                mediumWeaponSlots[i].removeAccessory();

            if (i <= transport.mediumWeaponNum - 1)
                mediumWeaponSlots[i].gameObject.SetActive(true);
            else
                mediumWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < lightWeaponSlots.Length; i++)
        {
            lightWeaponSlots[i].GetComponent<Button>().interactable = accessorySlotState;

            if (lightWeaponSlots[i].isEquip)
                lightWeaponSlots[i].removeAccessory();

            if (i <= transport.lightWeaponNum - 1)
                lightWeaponSlots[i].gameObject.SetActive(true);
            else
                lightWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < defenceEquipmentSlots.Length; i++)
        {
            defenceEquipmentSlots[i].GetComponent<Button>().interactable = accessorySlotState;

            if (defenceEquipmentSlots[i].isEquip)
                defenceEquipmentSlots[i].removeAccessory();

            if (i <= transport.defenceEquipmentNum - 1)
                defenceEquipmentSlots[i].gameObject.SetActive(true);
            else
                defenceEquipmentSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < auxiliaryEquipmentSlots.Length; i++)
        {
            auxiliaryEquipmentSlots[i].GetComponent<Button>().interactable = accessorySlotState;

            if (auxiliaryEquipmentSlots[i].isEquip)
                auxiliaryEquipmentSlots[i].removeAccessory();

            if (i <= transport.auxiliaryEquipmentNum - 1)
            {
                if (i == 2)
                    auxiliaryEquipmentSlots[i].transform.parent.gameObject.SetActive(true);
                auxiliaryEquipmentSlots[i].gameObject.SetActive(true);
            }
            else
            {
                if (i == 2)
                    auxiliaryEquipmentSlots[i].transform.parent.gameObject.SetActive(false);
                auxiliaryEquipmentSlots[i].gameObject.SetActive(false);
            }
        }

        fireControlSystemSlots.GetComponent<Button>().interactable = accessorySlotState;

        if (fireControlSystemSlots.isEquip)
        {
            
            fireControlSystemSlots.removeAccessory();
        }

        engineSlots.GetComponent<Button>().interactable = accessorySlotState;

        if (engineSlots.isEquip)
        {
            
            engineSlots.removeAccessory();
        }
            
        ResetUI_AccessoriesEquip();
        maxHp = transport.maxHp;
        armor = transport.armor;
        speed = transport.speed;
        maxLoad.text = " / " + transport.load.ToString();

        budgetCost = transport.budgetCost;
        developCost = transport.developCost;
        maintanceCost = transport.maintanceCost;
        produceCost = transport.produceCost;

        UpdateUnitProperty();
    }

    //Equip Function
    [Space]
    public string projectName;
    public AccessoryProperty[] equipedAccessories;

    int maxHp = 0;
    [HideInInspector] public int finalMaxHp = 0;
    public TextMeshProUGUI maxHpText;

    int armor = 0;
    [HideInInspector] public int finalArmor = 0;
    public TextMeshProUGUI armorText;

    int damage = 0;
    [HideInInspector] public int finalDamage = 0;
    public TextMeshProUGUI damageText;

    int range = 0;
    public TextMeshProUGUI rangeText;

    int speed = 0;
    public TextMeshProUGUI speedText;

    [HideInInspector] public int weight = 0;
    public TextMeshProUGUI weightText;

    public TextMeshProUGUI maxLoad;

    int budgetCost = 0;
    [HideInInspector] public int finalBudgetCost = 0;
    public TextMeshProUGUI budgetCostText;

    int developCost = 0;
    [HideInInspector] public int finalDevelopCost = 0;
    public TextMeshProUGUI developCostText;

    int maintanceCost = 0;
    [HideInInspector] public int finalMaintanceCost = 0;
    public TextMeshProUGUI maintanceCostText;

    int produceCost = 0;
    [HideInInspector] public int finalProduceCost = 0;
    public TextMeshProUGUI produceCostText;

    [HideInInspector] public int budgetPercentage = 100, qualityPercentage = 100, simplifyPercentage = 100, reduceCostPercentage = 100;
    public UI_CostChange budgetSlider;
    public UI_CostChange qualitySlider;
    public UI_CostChange simplifySlider;
    public UI_CostChange reduceCostSlider;

    const float Percentage = 100;
    [Header("Cost effect Unit Property")]
    public float propertyIncrease;
    public float costDecrease;
    public float minDevelopmentPercentage;
    public float modifyCostPercentage;
    public float upgradeCostPercentage;

    float percentageChange = 0;

    public void UpdateUnitProperty()
    {
        finalBudgetCost = budgetCost;
        finalMaxHp = maxHp;
        finalArmor = armor;
        finalDamage = damage;

        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                finalBudgetCost = budgetCost;
                finalDevelopCost = developCost;
                break;

            case ProjectType.UnitModify:
                finalBudgetCost = (int)Mathf.Round(budgetCost * modifyCostPercentage);
                finalDevelopCost = (int)Mathf.Round(developCost * modifyCostPercentage);
                break;

            case ProjectType.UnitUpgrade:
                finalBudgetCost = (int)Mathf.Round(budgetCost * upgradeCostPercentage);
                finalDevelopCost = (int)Mathf.Round(developCost * upgradeCostPercentage);
                break;
        }

        finalMaintanceCost = maintanceCost;
        finalProduceCost = produceCost;


        //Budget Percentage
        percentageChange = (budgetPercentage - Percentage) / Percentage;
        finalBudgetCost += (int)Mathf.Round(budgetCost * percentageChange);

        finalMaxHp += (int)Mathf.Round((float)maxHp * propertyIncrease * percentageChange);
        finalArmor += (int)Mathf.Round((float)armor * propertyIncrease * percentageChange);
        finalDamage += (int)Mathf.Round((float)damage * propertyIncrease * percentageChange);
        finalMaintanceCost -= (int)Mathf.Round((float)maintanceCost * costDecrease * percentageChange);
        finalProduceCost -= (int)Mathf.Round((float)produceCost * costDecrease * percentageChange);

        //Quality Percentage
        percentageChange = (qualityPercentage - Percentage) / Percentage;
        finalDevelopCost += (int)Mathf.Round(developCost * percentageChange);

        finalMaxHp += (int)Mathf.Round((float)maxHp * propertyIncrease * percentageChange);
        finalArmor += (int)Mathf.Round((float)armor * propertyIncrease * percentageChange);
        finalDamage += (int)Mathf.Round((float)damage * propertyIncrease * percentageChange);

        //Simplify Percentage
        percentageChange = (simplifyPercentage - Percentage) / Percentage;
        finalDevelopCost += (int)Mathf.Round(developCost * percentageChange);

        finalProduceCost -= (int)Mathf.Round((float)produceCost * costDecrease * percentageChange);

        //Reduce Cost Percentage
        percentageChange = (reduceCostPercentage - Percentage) / Percentage;
        finalDevelopCost += (int)Mathf.Round(developCost * percentageChange);

        finalMaintanceCost -= (int)Mathf.Round((float)maintanceCost * costDecrease * percentageChange);

        //Final Development Cost not less than original Development Cost
        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                finalDevelopCost = Mathf.Max(finalDevelopCost, (int)Mathf.Round(developCost * minDevelopmentPercentage));
                break;

            case ProjectType.UnitModify:
                finalDevelopCost = Mathf.Max(finalDevelopCost, (int)Mathf.Round(developCost * modifyCostPercentage * minDevelopmentPercentage));
                break;

            case ProjectType.UnitUpgrade:
                finalDevelopCost = Mathf.Max(finalDevelopCost, (int)Mathf.Round(developCost * upgradeCostPercentage * minDevelopmentPercentage));
                break;
        }

        

        maxHpText.text = finalMaxHp.ToString();
        armorText.text = finalArmor.ToString();
        damageText.text = finalDamage.ToString();
        rangeText.text = range.ToString();
        speedText.text = speed.ToString();
        weightText.text = weight.ToString();

        budgetCostText.text = "- " + finalBudgetCost.ToString();
        developCostText.text = finalDevelopCost.ToString();
        maintanceCostText.text = finalMaintanceCost.ToString();
        produceCostText.text = finalProduceCost.ToString();
    }

    public void ResetUI_AccessoriesEquip()
    {
        maxHp = 0;
        armor = 0;
        damage = 0;
        range = 0;
        speed = 0;
        weight = 0;

        budgetCost = 0;
        developCost = 0;
        maintanceCost = 0;
        produceCost = 0;

        budgetSlider.ChangeCost(100);
        qualitySlider.ChangeCost(100);
        simplifySlider.ChangeCost(100);
        reduceCostSlider.ChangeCost(100);
    }

    public void equipAccessory(AccessoryProperty accessory, int slotIndex)
    {
        maxHp += accessory.maxHp;
        armor += accessory.armor;
        damage += accessory.damage;
        range = Mathf.Max(this.range, accessory.range);
        speed += accessory.speed;
        weight += accessory.weight;

        budgetCost += accessory.budgetCost;
        developCost += accessory.developCost;
        maintanceCost += accessory.maintanceCost;
        produceCost += accessory.produceCost;

        if (tagController.AddTagFunction(accessory.accessoryTag))
            ChangeTag();

        if(createFunction != null)
            createFunction();
        
        equipedAccessories[slotIndex] = accessory;
        UpdateUnitProperty();
    }

    public void removeAccessory(AccessoryProperty accessory, int slotIndex)
    {
        maxHp -= accessory.maxHp;
        armor -= accessory.armor;
        damage -= accessory.damage;
        speed -= accessory.speed;
        weight -= accessory.weight;

        budgetCost -= accessory.budgetCost;
        developCost -= accessory.developCost;
        maintanceCost -= accessory.maintanceCost;
        produceCost -= accessory.produceCost;

        equipedAccessories[slotIndex] = null;
        
        range = 0;
        foreach (AccessoryProperty curAccessory in equipedAccessories)
        {
            if (curAccessory != null)
                range = Mathf.Max(this.range, curAccessory.range); //Find the accessory that have largest attack range
        }

        if (tagController.RemoveTagFunction(accessory.accessoryTag))
            ChangeTag();

        if (createFunction != null)
            createFunction();

        UpdateUnitProperty();
    }

    [Header("Unit Tag")]
    public List<TextMeshProUGUI> tagTextArray;
    public TagController tagController;
    public List<UnitTag> unitTagsList = new List<UnitTag>();

    public TagController.UnitFunction createFunction;
    public TagController.UnitFunction runtimeFunction;

    public void ChangeTag()
    {
        unitTagsList.Sort(CompareListByUnitTag);
        for (int i = 0; i < tagTextArray.Count; i++)
        {
            if (i < unitTagsList.Count) 
            {
                tagTextArray[i].text = unitTagsList[i].ToString();
                tagTextArray[i].transform.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tagTextArray[0].transform.parent.GetComponent<RectTransform>());
            }

            else
            {
                tagTextArray[i].transform.parent.gameObject.SetActive(false);
            }    
        }
    }

    public static int CompareListByUnitTag(UnitTag i1, UnitTag i2)
    {
        return i1.CompareTo(i2);
    }

    [Header("CreateProject")]
    public GameObject projectPrefab;
    public Transform projectList;
    public Sprite defualtIcon;

    UnitProperty unitProperty;

    public void CreateProject()
    {
        unitProperty = ScriptableObject.CreateInstance<UnitProperty>();
        unitProperty.Create(unitName, defualtIcon, finalMaxHp, finalArmor, finalDamage, range, speed, weight, finalBudgetCost, finalDevelopCost, finalMaintanceCost, finalProduceCost, transportProperty, equipedAccessories, slotsCount);

        GameObject tempProject = Instantiate(projectPrefab, projectList);
        projectName = unitName + " Project";

        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                tempProject.GetComponent<UnitDevelopmentProject>().ProjectCreate(WorldController.currentPlayer, building, projectType, projectName, finalBudgetCost, finalDevelopCost, null, unitProperty, runtimeFunction);
                WorldController.currentPlayer.projectCount++;
                break;

            case ProjectType.UnitModify:
                tempProject.GetComponent<UnitDevelopmentProject>().ProjectCreate(WorldController.currentPlayer, building, projectType, projectName, finalBudgetCost, finalDevelopCost, originalTemplate, unitProperty, runtimeFunction); //same Function
                WorldController.currentPlayer.projectCount++;
                break;

            case ProjectType.UnitUpgrade:
                tempProject.GetComponent<UnitDevelopmentProject>().ProjectCreate(WorldController.currentPlayer, building, projectType, projectName, finalBudgetCost, finalDevelopCost, originalTemplate, unitProperty, runtimeFunction); //same Function
                break;

            default:
                break;
        }

        if(WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer humanPlayer = (HumanPlayer)WorldController.currentPlayer;
            humanPlayer.projectList.Add(tempProject);
        }
    }

    public string unitName;
    public TMP_InputField unitNameInput;

    public void onChangeName()
    {
        unitName = unitNameInput.text;
    }

    public GameObject createButton;
    public GameObject costSelection;
    public GameObject filterButton;

    public void showTemplateDetail(UnitProperty property)
    {
        ChangeTransport(property.transportProperty);

        unitNameInput.text = property.unitName;
        unitNameInput.interactable = false;

        maxHpText.text = property.maxHp.ToString();
        armorText.text = property.armor.ToString();
        damageText.text = property.damage.ToString();
        rangeText.text = property.range.ToString();
        speedText.text = property.speed.ToString();
        weightText.text = property.weight.ToString();

        budgetCostText.text = "- " + property.budgetCost.ToString();
        developCostText.text = property.developCost.ToString();
        maintanceCostText.text = property.maintanceCost.ToString();
        produceCostText.text = property.produceCost.ToString();

        transportDropdown.text.text = property.transportProperty.transportName;
        transportDropdown.dropdown.interactable = false;

        foreach (AccessorySelection accessorySelection in accessoriesListAll)
        {
            accessorySelection.gameObject.SetActive(false);
        }

        unitTagsList = new List<UnitTag>();

        for (int i = 0; i < property.accessoryProperty.Length; i++)
        {
            AccessoryProperty accessory = property.accessoryProperty[i];

            if (accessory == null)
                continue;
                

            if (!unitTagsList.Contains(accessory.accessoryTag) && accessory.accessoryTag != UnitTag.None)
            {
                unitTagsList.Add(accessory.accessoryTag);
            }

            if (i < 3) //Heavy Weapon
            {
                heavyWeaponSlots[i].accessory = accessory;
                heavyWeaponSlots[i].showingIcon.sprite = accessory.icon;
                heavyWeaponSlots[i].showingIcon.enabled = true;
                heavyWeaponSlots[i].isEquip = true;
            }

            else if (i < 6)
            {
                mediumWeaponSlots[i - 3].accessory = accessory;
                mediumWeaponSlots[i - 3].showingIcon.sprite = accessory.icon;
                mediumWeaponSlots[i - 3].showingIcon.enabled = true;
                mediumWeaponSlots[i - 3].isEquip = true;
            }

            else if (i < 9)
            {
                lightWeaponSlots[i - 6].accessory = accessory;
                lightWeaponSlots[i - 6].showingIcon.sprite = accessory.icon;
                lightWeaponSlots[i - 6].showingIcon.enabled = true;
                lightWeaponSlots[i - 6].isEquip = true;
            }

            else if (i < 12)
            {
                defenceEquipmentSlots[i - 9].accessory = accessory;
                defenceEquipmentSlots[i - 9].showingIcon.sprite = accessory.icon;
                defenceEquipmentSlots[i - 9].showingIcon.enabled = true;
                defenceEquipmentSlots[i - 9].isEquip = true;
            }

            else if (i < 16)
            {
                auxiliaryEquipmentSlots[i - 12].accessory = accessory;
                auxiliaryEquipmentSlots[i - 12].showingIcon.sprite = accessory.icon;
                auxiliaryEquipmentSlots[i - 12].showingIcon.enabled = true;
                auxiliaryEquipmentSlots[i - 12].isEquip = true;
            }

            else if (i < 17)
            {
                fireControlSystemSlots.accessory = accessory;
                fireControlSystemSlots.showingIcon.sprite = accessory.icon;
                fireControlSystemSlots.showingIcon.enabled = true;
                fireControlSystemSlots.isEquip = true;
            }

            else if (i < 18)
            {
                engineSlots.accessory = accessory;
                engineSlots.showingIcon.sprite = accessory.icon;
                engineSlots.showingIcon.enabled = true;
                engineSlots.isEquip = true;
            }
        }

        ChangeTag();
        createButton.SetActive(false);
        costSelection.SetActive(false);
        filterButton.SetActive(false);
    }
}
