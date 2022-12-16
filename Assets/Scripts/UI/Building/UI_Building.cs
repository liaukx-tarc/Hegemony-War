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
    public GameObject groundUnitList;
    public GameObject navalUnitList;
    public GameObject airForceUnitList;

    public City selectedCity;
    public Area selectedArea;

    [Header("TabGroud")]
    public TabGroup tabGroup;

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

        produceObjImage.gameObject.SetActive(false);
        projectObjImage.gameObject.SetActive(false);

        UpdateCityInfo();
        UpdateTabBar();
        UpdateBuildingList();
        UpdateUnitTemplateList();

        tabGroup.OnTabButtonSelected(tabGroup.buttonsList[0]);
        UpdateProduceProgress();
    }

    public void CloseBuildingUI()
    {
        gameObject.SetActive(false);
    }


    IEnumerator UpdateUILayout()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    //City Info
    [Header("Info")]
    public TextMeshProUGUI cityName;
    public Image cityNameBgr;
    public TextMeshProUGUI healthPointText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI damageText;

    [Header("Resource")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI productivityText;
    public TextMeshProUGUI sciencePointText;
    public TextMeshProUGUI developmentPointText;

    public void UpdateCityInfo()
    {
        cityName.text= selectedCity.cityName;
        cityNameBgr.color = selectedCity.player.playerColor;

        healthPointText.text = selectedCity.healthPoint.ToString();
        defenseText.text = selectedCity.defense.ToString();
        damageText.text = selectedCity.damage.ToString();

        moneyText.text = selectedCity.moneyIncome.ToString();
        foodText.text = selectedCity.foodIncome.ToString();
        productivityText.text = selectedCity.productivityIncome.ToString();
        sciencePointText.text = selectedCity.sciencePointIncome.ToString();
        developmentPointText.text = selectedCity.developmentPointIncome.ToString();
    }

    //Tab bar
    [Header("Tab Bar")]
    public GameObject groundTabBar;
    public GameObject airForceTabBar;
    public GameObject navalTabBar;

    public void UpdateTabBar()
    {
        if (selectedCity.isDevelopmentGround)
            groundTabBar.SetActive(true);
        else
            groundTabBar.SetActive(false);

        if (selectedCity.isDevelopmentAirForce)
            airForceTabBar.SetActive(true);
        else
            airForceTabBar.SetActive(false);

        if (selectedCity.isDevelopmentNaval)
            navalTabBar.SetActive(true);
        else
            navalTabBar.SetActive(false);
    }

    //Produce Progress
    [Header("Produce")]
    public Image produceObjImage;
    public Image produceIconBgr;
    public Image produceIcon;
    public TextMeshProUGUI produceName;
    public TextMeshProUGUI productivityCompletedText;
    public TextMeshProUGUI productivityNeedText;
    public TextMeshProUGUI produceTurnLeftText;
    public void UpdateProduceProgress()
    {
        projectObjImage.gameObject.SetActive(false);

        if (selectedCity.producingArea != null)
        {
            produceIconBgr.color = selectedCity.producingArea.backgroundColor;
            produceIcon.sprite = selectedCity.producingArea.icon;
            produceIcon.color = selectedCity.producingArea.iconColor;
            produceName.text = selectedCity.producingArea.buildingName;
            
        }

        else if(selectedCity.producingUnit != null)
        {
            switch (selectedCity.producingUnit.property.transportProperty.transportType)
            {
                case TransportType.Vechicle:
                    produceIconBgr.color = WorldController.UI.vechicleColor;
                    break;

                case TransportType.Aircarft:
                    produceIconBgr.color = WorldController.UI.aircarftColor;
                    break;

                case TransportType.Ship:
                    produceIconBgr.color = WorldController.UI.shipColor;
                    break;
            }

            produceIcon.sprite = selectedCity.producingUnit.property.unitIcon;
            produceName.text = selectedCity.producingUnit.property.unitName;
        }

        else
        {
            produceObjImage.gameObject.SetActive(false);
            return;
        }

        produceObjImage.gameObject.SetActive(true);

        productivityCompletedText.text = selectedCity.productivityCompleted.ToString();
        productivityNeedText.text = selectedCity.productivityNeed.ToString();

        int remainTurn = (selectedCity.productivityNeed - selectedCity.productivityCompleted) / selectedCity.productivityIncome;
        if ((selectedCity.productivityNeed - selectedCity.productivityCompleted) % selectedCity.productivityIncome != 0)
            remainTurn++;
        produceTurnLeftText.text = remainTurn.ToString();

        StartCoroutine(UpdateUILayout());
    }

    public void CancelProduce()
    {
        selectedCity.CancelProduce();
        UpdateProduceProgress();
    }

    //Area Building
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
    public void UpdateBuildingList()
    {
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

        StartCoroutine(UpdateUILayout());
    }


    //Unit Template
    public void UpdateUnitTemplateList()
    {
        unitTemplates = WorldController.currentPlayer.unitTemplateList;

        groundUnitList.transform.parent.gameObject.SetActive(false);
        navalUnitList.transform.parent.gameObject.SetActive(false);
        airForceUnitList.transform.parent.gameObject.SetActive(false);

        foreach (UnitTemplateSelection selection in unitTemplatesSelection)
        {
            selection.gameObject.SetActive(false);
        }

        for (int i = 0; i < unitTemplates.Count; i++)
        {
            if (i >= unitTemplatesSelection.Count)
            {
                GameObject tempSlecionObj = Instantiate(UnitTemplateSelectionPrefab);
                unitTemplatesSelection.Add(tempSlecionObj.GetComponent<UnitTemplateSelection>());
            }

            unitTemplatesSelection[i].unitIcon.sprite = unitTemplates[i].property.unitIcon;
            unitTemplatesSelection[i].unitName.text = unitTemplates[i].property.unitName;
            unitTemplatesSelection[i].produceVariable.text = unitTemplates[i].property.produceCost.ToString();
            unitTemplatesSelection[i].unitTemplate = unitTemplates[i];

            switch (unitTemplates[i].property.transportProperty.transportType)
            {
                case TransportType.Vechicle:
                    unitTemplatesSelection[i].templateBackground.color = WorldController.UI.vechicleColor;
                    unitTemplatesSelection[i].transform.SetParent(groundUnitList.transform);
                    
                    if (selectedCity.isProduceGround)
                        groundUnitList.transform.parent.gameObject.SetActive(true);
                    break;

                case TransportType.Aircarft:
                    unitTemplatesSelection[i].templateBackground.color = WorldController.UI.aircarftColor;
                    unitTemplatesSelection[i].transform.SetParent(airForceUnitList.transform);
                    
                    if (selectedCity.isProduceAirForce)
                        airForceUnitList.transform.parent.gameObject.SetActive(true);
                    break;

                case TransportType.Ship:
                    unitTemplatesSelection[i].templateBackground.color = WorldController.UI.shipColor;
                    unitTemplatesSelection[i].transform.SetParent(navalUnitList.transform);
                    
                    if (selectedCity.isProduceNaval)
                        navalUnitList.transform.parent.gameObject.SetActive(true);
                    break;
            }
            unitTemplatesSelection[i].gameObject.SetActive(true);
        }

        StartCoroutine(UpdateUILayout());
    }

    public IEnumerator AddProduceFail()
    {
        produceObjImage.enabled = true;

        yield return new WaitForSeconds(0.5f);

        produceObjImage.enabled = false;
    }

    //Development Project
    [Header("Project")]
    public Image projectObjImage;
    public Image projectIconBgr;
    public Image projectIcon;
    public TextMeshProUGUI projectName;
    public TextMeshProUGUI developmentPointCompletedText;
    public TextMeshProUGUI developmentPointNeedText;
    public TextMeshProUGUI projectLeftText;

    public void UpdateProjectProgress()
    {
        produceObjImage.gameObject.SetActive(false);

        if (selectedCity.developmentProject == null)
        {
            projectObjImage.gameObject.SetActive(false);
            return;
        }

        projectIconBgr.color = selectedCity.developmentProject.iconBackground.color;
        projectIcon.sprite = selectedCity.developmentProject.icon.sprite;
        projectName.text = selectedCity.developmentProject.projectName;
        developmentPointCompletedText.text = selectedCity.developmentProject.progressBar.value.ToString();
        developmentPointNeedText.text = selectedCity.developmentProject.progressBar.maxValue.ToString();
        projectLeftText.text = selectedCity.developmentProject.CalculateRemainTurn().ToString();

        projectObjImage.gameObject.SetActive(true);
        StartCoroutine(UpdateUILayout());
    }

    public void CancelProject()
    {
        if (selectedCity.developmentProject.developmentCenters.Count > 1)
        {
            selectedCity.CancelProject();
            UpdateProjectProgress();
        }

        else
        {
            StartCoroutine(ChangeProjectFail());
        }
    }

    public IEnumerator ChangeProjectFail()
    {
        projectObjImage.enabled = true;

        yield return new WaitForSeconds(0.5f);

        projectObjImage.enabled = false;
    }
}
