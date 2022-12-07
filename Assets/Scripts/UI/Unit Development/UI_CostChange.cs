using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum developmentCostType
{
    Budget,
    Quality,
    Simplify,
    ReduceCost
}

public class UI_CostChange : MonoBehaviour
{
    public developmentCostType costType;
    int costPercentage;

    public TMP_InputField inputField;
    public Slider slider;

    public void OnCostChange_Slider()
    {
        costPercentage = (int)slider.value;
        inputField.text = costPercentage.ToString();
        costChange();
    }

    public void OnCostChange_InputField()
    {
        costPercentage = Mathf.Min(Convert.ToUInt16(inputField.text), 200);
        costPercentage = Mathf.Max(Convert.ToUInt16(inputField.text), 50);
        inputField.text = costPercentage.ToString();
        slider.value = costPercentage;
        costChange();
    }

    public void ChangeCost(int changedValue)
    {
        costPercentage = changedValue;
        slider.value = changedValue;
        inputField.text = changedValue.ToString();
        costChange();
    }

    void costChange()
    {
        switch (costType)
        {
            case developmentCostType.Budget:
                UI_Controller.accessoriesUI.budgetPercentage = costPercentage;
                break;
            case developmentCostType.Quality:
                UI_Controller.accessoriesUI.qualityPercentage = costPercentage;
                break;
            case developmentCostType.Simplify:
                UI_Controller.accessoriesUI.simplifyPercentage = costPercentage;
                break;
            case developmentCostType.ReduceCost:
                UI_Controller.accessoriesUI.reduceCostPercentage = costPercentage;
                break;
        }

        UI_Controller.accessoriesUI.UpdateUnitProperty();
    }
}
