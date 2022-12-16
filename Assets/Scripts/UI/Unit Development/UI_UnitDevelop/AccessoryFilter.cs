using UnityEngine;

public class AccessoryFilter : MonoBehaviour
{
    public AccessoriesTypes type;

    public void Filter()
    {
        foreach (AccessorySelection accessory in UI_Controller.accessoriesUI.accessoriesListAll)
        {
            accessory.gameObject.SetActive(false);
        }

        foreach (AccessorySelection accessory in UI_Controller.accessoriesUI.accessoriesListType[(int)type])
        {
            accessory.gameObject.SetActive(true);
        }

    }

    public void AllFilter()
    {
        foreach (AccessorySelection accessory in UI_Controller.accessoriesUI.accessoriesListAll)
        {
            accessory.gameObject.SetActive(true);
        }
    }
}
