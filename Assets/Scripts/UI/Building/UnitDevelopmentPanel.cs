using UnityEngine;

public class UnitDevelopmentPanel : MonoBehaviour
{
    public TransportType transportType;
    const string UnitDevelopmentTitle = "Unit Development";
    const string UnitModifyTitle = "Unit Modify";
    const string UnitUpgradeTitle = "Unit Upgrade";

    public void OpenUnitDevelopmentUI()
    {
        WorldController.instance.uiController.ClickSound();
        if (WorldController.instance.uiController.buildingUIController.selectedCity.developmentProject == null)
        {
            WorldController.instance.uiController.OpenAccessoriesUI();
            WorldController.instance.uiController.UD_title.text = UnitDevelopmentTitle;

            WorldController.instance.uiController.accessoriesUI.UIOpen(WorldController.instance.uiController.buildingUIController.selectedCity, ProjectType.UnitDevelopment, transportType, null);
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenUnitModifyUI()
    {
        WorldController.instance.uiController.ClickSound();
        if (WorldController.instance.uiController.buildingUIController.selectedCity.developmentProject == null)
        {
            WorldController.instance.uiController.closeAllUIFunction();
            WorldController.instance.uiController.UD_title.text = UnitModifyTitle;

            WorldController.instance.uiController.unitTemplateListUI.isModify = true;
            WorldController.instance.uiController.unitTemplateListUI.OpenTemplateUI(transportType);
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenUnitUpgradeUI()
    {
        WorldController.instance.uiController.ClickSound();
        if (WorldController.instance.uiController.buildingUIController.selectedCity.developmentProject == null)
        {
            WorldController.instance.uiController.closeAllUIFunction();
            WorldController.instance.uiController.UD_title.text = UnitUpgradeTitle;

            WorldController.instance.uiController.unitTemplateListUI.isUpgrade = true;
            WorldController.instance.uiController.unitTemplateListUI.OpenTemplateUI(transportType);
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenSupportProjectUI()
    {
        WorldController.instance.uiController.ClickSound();
        if (WorldController.instance.uiController.buildingUIController.selectedCity.developmentProject == null)
        {
            WorldController.instance.uiController.progressUI.isAddingCity = true;
            WorldController.instance.uiController.OpenUDProgressUI();
        }

        else
        {
            StartCoroutine(WorldController.instance.uiController.buildingUIController.ChangeProjectFail());
        }
    }
}
