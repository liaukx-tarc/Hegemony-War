using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Building : MonoBehaviour
{
    public GameObject buildingSelectionPrefab;
    public GameObject UnitTemplateSelectionPrefab;

    public List<BuildingProperty> buildingTemplates;
    public List<BuildingSelection> buildingTemplatesSelection;

    public List<UnitTemplate> unitTemplates;
    public List<UnitTemplateSelection> unitTemplatesSelection;
    public GameObject unitTemplatesSelectionListObj;

    public void BuildingUI_Start()
    {
        foreach (Player player in WorldController.playerList)
        {
            if(player.GetType() == typeof(HumanPlayer))
            {
                player.playerStartFunction += UpdateBuildingList;
                player.playerStartFunction += UpdateUnitTemplateList;
            }
        }
    }

    public void UpdateBuildingList()
    {

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
                unitTemplatesSelection[i].gameObject.SetActive(true);
            }

            else
            {
                GameObject tempSlecionObj = Instantiate(UnitTemplateSelectionPrefab, unitTemplatesSelectionListObj.transform);
                unitTemplatesSelection.Add(tempSlecionObj.GetComponent<UnitTemplateSelection>());
                unitTemplatesSelection[i].unitIcon.sprite = unitTemplates[i].property.unitIcon;
                unitTemplatesSelection[i].unitName.text = unitTemplates[i].property.unitName;
                unitTemplatesSelection[i].gameObject.SetActive(true);
            }
        }
    }
}
