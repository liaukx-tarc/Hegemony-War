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
        if (WorldController.instance.uiController.accessoriesUI.unitTagsList.Contains(tag))
            return false; //Already have this tag

        switch (tag)
        {
            case UnitTag.antiAir:
                WorldController.instance.uiController.accessoriesUI.runtimeFunction += AntiAirFunction;
                break;

            case UnitTag.antiTank:

                break;

            case UnitTag.missle:

                break;

            case UnitTag.sniper:
                WorldController.instance.uiController.accessoriesUI.runtimeFunction += SniperFunction;
                break;

            case UnitTag.computer:
                WorldController.instance.uiController.accessoriesUI.createFunction += ComputerFunction;
                break;

            case UnitTag.testing:
                WorldController.instance.uiController.accessoriesUI.createFunction += testFunction;
                break;

            default:
                break;
        }

        if (tag != UnitTag.None)
            WorldController.instance.uiController.accessoriesUI.unitTagsList.Add(tag);
        return true;
    }

    public bool RemoveTagFunction(UnitTag tag)
    {
        foreach (AccessoryProperty accessory in WorldController.instance.uiController.accessoriesUI.equipedAccessories)
        {
            if (accessory != null && accessory.accessoryTag == tag)
                return false; //Still have accessory having this tag
        }

        switch (tag)
        {
            case UnitTag.antiAir:
                WorldController.instance.uiController.accessoriesUI.runtimeFunction -= AntiAirFunction;
                break;

            case UnitTag.antiTank:

                break;

            case UnitTag.missle:

                break;

            case UnitTag.sniper:
                WorldController.instance.uiController.accessoriesUI.runtimeFunction -= SniperFunction;
                break;

            case UnitTag.computer:
                WorldController.instance.uiController.accessoriesUI.createFunction -= ComputerFunction;
                break;

            case UnitTag.testing:
                WorldController.instance.uiController.accessoriesUI.createFunction -= testFunction;
                break;

            default:
                break;
        }

        if (tag != UnitTag.None)
            WorldController.instance.uiController.accessoriesUI.unitTagsList.Remove(tag);
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
