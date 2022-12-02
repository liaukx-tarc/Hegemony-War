using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum UnitTag
{
    None,
    antiAir,
    antiTank,
    missle,
    sniper,
    computer,
    testing
}

public class TagController : MonoBehaviour
{
    public delegate void UnitFunction();

    public bool AddTagFunction(UnitTag tag)
    {
        if (UI_Controller.accessoriesUI.unitTagsList.Contains(tag))
            return false; //Already have this tag

        switch (tag)
        {
            case UnitTag.antiAir:
                UI_Controller.accessoriesUI.runtimeFunction += AntiAirFunction;
                break;

            case UnitTag.antiTank:

                break;

            case UnitTag.missle:

                break;

            case UnitTag.sniper:
                UI_Controller.accessoriesUI.runtimeFunction += SniperFunction;
                break;

            case UnitTag.computer:
                UI_Controller.accessoriesUI.createFunction += ComputerFunction;
                break;

            case UnitTag.testing:
                UI_Controller.accessoriesUI.createFunction += testFunction;
                break;

            default:
                break;
        }

        if (tag != UnitTag.None)
            UI_Controller.accessoriesUI.unitTagsList.Add(tag);
        return true;
    }

    public bool RemoveTagFunction(UnitTag tag)
    {
        foreach (AccessoryProperty accessory in UI_Controller.accessoriesUI.equipedAccessories)
        {
            if (accessory != null && accessory.accessoryTag == tag)
                return false; //Still have accessory having this tag
        }

        switch (tag)
        {
            case UnitTag.antiAir:
                UI_Controller.accessoriesUI.runtimeFunction -= AntiAirFunction;
                break;

            case UnitTag.antiTank:

                break;

            case UnitTag.missle:

                break;

            case UnitTag.sniper:
                UI_Controller.accessoriesUI.runtimeFunction -= SniperFunction;
                break;

            case UnitTag.computer:
                UI_Controller.accessoriesUI.createFunction -= ComputerFunction;
                break;

            case UnitTag.testing:
                UI_Controller.accessoriesUI.createFunction -= testFunction;
                break;

            default:
                break;
        }

        if (tag != UnitTag.None)
            UI_Controller.accessoriesUI.unitTagsList.Remove(tag);
        return true;
    }

    public void AntiAirFunction()
    {
        Debug.Log("Anti Air Function");
    }

    public void AntiTankFunction()
    {

    }

    public void MissleFunction()
    {

    }

    public void SniperFunction()
    {
        Debug.Log("Sniper Function");
    }

    public void ComputerFunction()
    {
        Debug.Log("Computer Function");
    }

    public void testFunction()
    {
        Debug.Log("Test Function");
    }
}
