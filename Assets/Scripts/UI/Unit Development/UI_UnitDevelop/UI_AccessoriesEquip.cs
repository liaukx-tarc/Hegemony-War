using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
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
    public Building building;

    public TextMeshProUGUI createBtnText;
    const string Create = "Create";
    const string Modify = "Modify";
    const string Upgrade = "Upgrade";

    public void UIOpen(ProjectType projectType, TransportType transportType, UnitTemplate originalTemplate)
    {
        this.projectType = projectType;

        switch (projectType)
        {
            case ProjectType.UnitDevelopment:
                transportDropdown.onTransportTypeChange(transportType);
                ChangeAccessoriesSelect(transportType);
                createBtnText.text = Create;
                break;

            case ProjectType.UnitModify:
                transportDropdown.dropdown.interactable = false;
                transportDropdown.text.text = originalTemplate.property.transportProperty.transportName;
                ChangeAccessoriesSelect(transportType);
                createBtnText.text = Modify;
                break;

            case ProjectType.UnitUpgrade:
                createBtnText.text = Upgrade;
                break;
        }

        
    }

    //Change Transport
    [Header("Transport")]
    public TransportDropdown transportDropdown;
    public TransportProperty transportProperty;
   
    public void ChangeAccessoriesSelect(TransportType transportType)
    {
        bool tagMatch;

        foreach (AccessorySelection accessorySelection in accessoriesListAll)
        {
            tagMatch = false;

            foreach (TransportType transport in accessorySelection.property.transportType)
            {
                if(transport == transportType)
                {
                    accessorySelection.gameObject.SetActive(true);
                    tagMatch = true;
                    break;
                }
            }

            if(!tagMatch)
                accessorySelection.gameObject.SetActive(false);
        }
    }

    public void ChangeTransport(TransportProperty transport)
    {
        transportProperty = transport;

        for (int i = 0; i < heavyWeaponSlots.Length; i++)
        {
            if (heavyWeaponSlots[i].isEquip)
                heavyWeaponSlots[i].removeAccessory();

            if (i <= transport.heavyWeaponNum - 1)
                heavyWeaponSlots[i].gameObject.SetActive(true);
            else
                heavyWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < mediumWeaponSlots.Length; i++)
        {
            if (mediumWeaponSlots[i].isEquip)
                mediumWeaponSlots[i].removeAccessory();

            if (i <= transport.mediumWeaponNum - 1)
                mediumWeaponSlots[i].gameObject.SetActive(true);
            else
                mediumWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < lightWeaponSlots.Length; i++)
        {
            if (lightWeaponSlots[i].isEquip)
                lightWeaponSlots[i].removeAccessory();

            if (i <= transport.lightWeaponNum - 1)
                lightWeaponSlots[i].gameObject.SetActive(true);
            else
                lightWeaponSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < defenceEquipmentSlots.Length; i++)
        {
            if (defenceEquipmentSlots[i].isEquip)
                defenceEquipmentSlots[i].removeAccessory();

            if (i <= transport.defenceEquipmentNum - 1)
                defenceEquipmentSlots[i].gameObject.SetActive(true);
            else
                defenceEquipmentSlots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < auxiliaryEquipmentSlots.Length; i++)
        {
            if (auxiliaryEquipmentSlots[i].isEquip)
                auxiliaryEquipmentSlots[i].removeAccessory();

            if (i <= transport.auxiliaryEquipmentNum - 1)
            {
                if(i == 2)
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

        if (fireControlSystemSlots.isEquip)
            fireControlSystemSlots.removeAccessory();

        if (engineSlots.isEquip)
            engineSlots.removeAccessory();

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

    const float Percentage = 100;
    [Header("Cost effect Unit Property")]
    public float propertyIncrease;
    public float costDecrease;

    float percentageChange = 0;

    public void UpdateUnitProperty()
    {
        finalBudgetCost = budgetCost;
        finalMaxHp = maxHp;
        finalArmor = armor;
        finalDamage = damage;
        finalBudgetCost = budgetCost;
        finalDevelopCost = developCost;
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

        budgetPercentage = 100;
        qualityPercentage = 100;
        simplifyPercentage = 100;
        reduceCostPercentage = 100;
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
            }

            else
            {
                tagTextArray[i].transform.parent.gameObject.SetActive(false);
            }    
        }
    }

    private static int CompareListByUnitTag(UnitTag i1, UnitTag i2)
    {
        return i1.CompareTo(i2);
    }

    [Header("CreateProject")]
    public GameObject projectPrefab;
    public Transform projectList;

    public void CreateProject()
    {
        UnitProperty unitProperty = ScriptableObject.CreateInstance<UnitProperty>();
        unitProperty.Create(finalMaxHp, finalArmor, finalDamage, range, speed, weight, finalBudgetCost, finalDevelopCost, finalMaintanceCost, finalProduceCost, transportProperty, equipedAccessories, slotsCount);

        UnitTemplate unitTemplate = new UnitTemplate(unitProperty, runtimeFunction);

        GameObject tempProject = Instantiate(projectPrefab, projectList);
        tempProject.GetComponent<UnitDevelopmentProject>().ProjectCreate(WorldController.playerList[0], building, projectType, projectName, finalBudgetCost, finalDevelopCost, null, unitTemplate);
    }
}
