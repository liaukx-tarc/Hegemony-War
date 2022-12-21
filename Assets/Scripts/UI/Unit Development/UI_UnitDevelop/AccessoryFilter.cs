using UnityEngine;

public class AccessoryFilter : MonoBehaviour
{
    public AccessoriesTypes type;

    public void Filter()
    {
        foreach (AccessorySelection accessory in WorldController.instance.uiController.accessoriesUI.accessoriesListAll)
        {
            accessory.gameObject.SetActive(false);
        }

        foreach (AccessorySelection accessory in WorldController.instance.uiController.accessoriesUI.accessoriesListType[(int)type])
        {
            if (accessory.property.transportType.Contains(WorldController.instance.uiController.accessoriesUI.transportProperty.transportType))
                accessory.gameObject.SetActive(true);
        }

    }

    public void AllFilter()
    {
        foreach (AccessorySelection accessory in WorldController.instance.uiController.accessoriesUI.accessoriesListAll)
        {
            if (accessory.property.transportType.Contains(WorldController.instance.uiController.accessoriesUI.transportProperty.transportType))
                accessory.gameObject.SetActive(true);
        }
    }
}
