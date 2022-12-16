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
        if(UI_Controller.progressUI.selectedProject.developmentCenters.Count > 1)
        {
            UI_Controller.progressUI.selectedProject.RemoveDevelopmentCenter(city);
            UI_Controller.progressUI.ShowProjectDetail(UI_Controller.progressUI.selectedProject);
        }
        
        else
        {
            StartCoroutine(UI_Controller.progressUI.AddCityFail());
        }
    }
}
