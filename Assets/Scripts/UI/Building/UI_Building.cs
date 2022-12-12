using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour
{
    public GameObject UnitTemplateSelectionPrefab;

    public List<UnitTemplate> unitTemplates;
    public List<UnitTemplateSelection> unitTemplatesSelection;
    public GameObject unitTemplatesSelectionListObj;

    public City selectedCity;
    public Area selectedArea;

    [Header("Building Template")]
    public GameObject militaryList;
    public GameObject baseGround;
    public GameObject baseNaval;
    public GameObject baseAirForce;

    [Space]
    public GameObject developmentList;
    public GameObject developmentGround;
    public GameObject developmentNaval;
    public GameObject developmentAirForce;

    [Header("Info")]
    public TextMeshProUGUI healthPointText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI damageText;

    [Header("Resource")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI productivityText;
    public TextMeshProUGUI sciencePointText;
    public TextMeshProUGUI developmentPointText;

    public void BuildingUI_Start()
    {
        UI_Controller.closeAllUIFunction += CloseBuildingUI;
    }

    public void OpenBuildingUI(Building building)
    {
        if (building.GetType() == typeof(City))
            selectedCity = (City)building;
        else if (building.GetType() == typeof(Area))
            selectedArea = (Area)building;

        gameObject.SetActive(true);
        UpdateBuildingList();
        UpdateUnitTemplateList();
    }

    public void CloseBuildingUI()
    {
        gameObject.SetActive(false);
    }

    public void UpdateBuildingList()
    {
        healthPointText.text = selectedCity.healthPoint.ToString();
        defenseText.text = selectedCity.defense.ToString();
        damageText.text = selectedCity.damage.ToString();

        moneyText.text = selectedCity.moneyIncome.ToString();
        foodText.text = selectedCity.foodIncome.ToString();
        productivityText.text = selectedCity.productivityIncome.ToString();
        sciencePointText.text = selectedCity.sciencePointIncome.ToString();
        developmentPointText.text = selectedCity.developmentPointIncome.ToString();

        if (selectedCity.isProduceGround && selectedCity.isProduceNaval && selectedCity.isProduceAirForce)
        {
            militaryList.SetActive(false);
        }
        else
        {
            if (selectedCity.isProduceGround)
                baseGround.SetActive(false);
            else
                baseGround.SetActive(true);

            if (selectedCity.isProduceNaval)
                baseNaval.SetActive(false);
            else
                baseNaval.SetActive(true);

            if (selectedCity.isProduceAirForce)
                baseAirForce.SetActive(false);
            else
                baseAirForce.SetActive(true);

            militaryList.SetActive(true);
        }

        if (selectedCity.isDevelopmentGround && selectedCity.isDevelopmentNaval && selectedCity.isDevelopmentAirForce)
        {
            developmentList.SetActive(false);
        }

        else
        {
            if (selectedCity.isDevelopmentGround)
                developmentGround.SetActive(false);
            else
                developmentGround.SetActive(true);

            if (selectedCity.isDevelopmentNaval)
                developmentNaval.SetActive(false);
            else
                developmentNaval.SetActive(true);

            if (selectedCity.isDevelopmentAirForce)
                developmentAirForce.SetActive(false);
            else
                developmentAirForce.SetActive(true);

            developmentList.SetActive(true);
        }

        StartCoroutine(UpdateTemplateLayout());
    }

    IEnumerator UpdateTemplateLayout()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(militaryList.transform.parent.parent.GetComponent<RectTransform>());
    }

    public void UpdateUnitTemplateList()
    {
        unitTemplates = WorldController.currentPlayer.unitTemplateList;

        foreach (UnitTemplateSelection selection in unitTemplatesSelection)
        {
            selection.gameObject.SetActive(false);
        }

        for (int i = 0; i < unitTemplates.Count; i++)
        {
            if (i < unitTemplatesSelection.Count)
            {
                unitTemplatesSelection[i].unitIcon.sprite = unitTemplates[i].property.unitIcon;
                unitTemplatesSelection[i].unitName.text = unitTemplates[i].property.unitName;
                switch (unitTemplates[i].property.transportProperty.transportType)
                {
                    case TransportType.Infantry:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].infantryColor;
                        break;
                    case TransportType.Vechicle:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].vechicleColor;
                        break;
                    case TransportType.Aircarft:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].aircarftColor;
                        break;
                    case TransportType.Ship:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].shipColor;
                        break;
                }
                unitTemplatesSelection[i].gameObject.SetActive(true);
            }

            else
            {
                GameObject tempSlecionObj = Instantiate(UnitTemplateSelectionPrefab, unitTemplatesSelectionListObj.transform);
                unitTemplatesSelection.Add(tempSlecionObj.GetComponent<UnitTemplateSelection>());
                unitTemplatesSelection[i].unitIcon.sprite = unitTemplates[i].property.unitIcon;
                unitTemplatesSelection[i].unitName.text = unitTemplates[i].property.unitName;
                switch (unitTemplates[i].property.transportProperty.transportType)
                {
                    case TransportType.Infantry:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].infantryColor;
                        break;
                    case TransportType.Vechicle:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].vechicleColor;
                        break;
                    case TransportType.Aircarft:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].aircarftColor;
                        break;
                    case TransportType.Ship:
                        unitTemplatesSelection[i].templateBackground.color = unitTemplates[i].shipColor;
                        break;
                }
                unitTemplatesSelection[i].gameObject.SetActive(true);
            }
        }
    }
}
