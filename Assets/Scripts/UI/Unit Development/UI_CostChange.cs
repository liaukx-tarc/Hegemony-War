using System;
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
                WorldController.instance.uiController.accessoriesUI.budgetPercentage = costPercentage;
                break;
            case developmentCostType.Quality:
                WorldController.instance.uiController.accessoriesUI.qualityPercentage = costPercentage;
                break;
            case developmentCostType.Simplify:
                WorldController.instance.uiController.accessoriesUI.simplifyPercentage = costPercentage;
                break;
            case developmentCostType.ReduceCost:
                WorldController.instance.uiController.accessoriesUI.reduceCostPercentage = costPercentage;
                break;
        }

        WorldController.instance.uiController.accessoriesUI.UpdateUnitProperty();
    }
}
