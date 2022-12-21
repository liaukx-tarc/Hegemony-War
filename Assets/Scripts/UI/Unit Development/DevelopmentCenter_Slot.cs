using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevelopmentCenter_Slot : MonoBehaviour
{
    public TextMeshProUGUI developmentPointText;
    public TextMeshProUGUI cityName;
    public Image cityNameBgr;
    public Image cityIcon;

    public City city;

    public void RemoveCity()
    {
        if(WorldController.instance.uiController.progressUI.selectedProject.developmentCenters.Count > 1)
        {
            WorldController.instance.uiController.progressUI.selectedProject.RemoveDevelopmentCenter(city);
            WorldController.instance.uiController.progressUI.ShowProjectDetail(WorldController.instance.uiController.progressUI.selectedProject);
        }
        
        else
        {
            StartCoroutine(WorldController.instance.uiController.progressUI.AddCityFail());
        }
    }
}
