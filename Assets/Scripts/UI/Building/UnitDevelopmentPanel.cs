using UnityEngine;

public class UnitDevelopmentPanel : MonoBehaviour
{
    public TransportType transportType;
    const string UnitDevelopmentTitle = "Unit Development";
    const string UnitModifyTitle = "Unit Modify";
    const string UnitUpgradeTitle = "Unit Upgrade";

    public void OpenUnitDevelopmentUI()
    {
        if(UI_Controller.buildingUIController.selectedCity.developmentProject == null)
        {
            WorldController.UI.OpenAccessoriesUI();
            WorldController.UI.UD_title.text = UnitDevelopmentTitle;

            UI_Controller.accessoriesUI.UIOpen(UI_Controller.buildingUIController.selectedCity, ProjectType.UnitDevelopment, transportType, null);
        }

        else
        {
            StartCoroutine(UI_Controller.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenUnitModifyUI()
    {
        if (UI_Controller.buildingUIController.selectedCity.developmentProject == null)
        {
            UI_Controller.closeAllUIFunction();
            WorldController.UI.UD_title.text = UnitModifyTitle;

            UI_Controller.unitTemplateListUI.isModify = true;
            UI_Controller.unitTemplateListUI.OpenTemplateUI(transportType);
        }

        else
        {
            StartCoroutine(UI_Controller.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenUnitUpgradeUI()
    {
        if (UI_Controller.buildingUIController.selectedCity.developmentProject == null)
        {
            UI_Controller.closeAllUIFunction();
            WorldController.UI.UD_title.text = UnitUpgradeTitle;

            UI_Controller.unitTemplateListUI.isUpgrade = true;
            UI_Controller.unitTemplateListUI.OpenTemplateUI(transportType);
        }

        else
        {
            StartCoroutine(UI_Controller.buildingUIController.ChangeProjectFail());
        }
    }

    public void OpenSupportProjectUI()
    {
        if (UI_Controller.buildingUIController.selectedCity.developmentProject == null)
        {
            UI_Controller.progressUI.isAddingCity = true;
            WorldController.UI.OpenUDProgressUI();
        }

        else
        {
            StartCoroutine(UI_Controller.buildingUIController.ChangeProjectFail());
        }
    }
}
