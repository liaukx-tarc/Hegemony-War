using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Progress : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI projectNameText;

    public TextMeshProUGUI maxHpText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI maintanceCostText;
    public TextMeshProUGUI produceCostText;

    public void DisablePlayerProject()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.currentPlayer;
            foreach (GameObject project in player.projectList)
            {
                project.SetActive(false);
            }
        }
    }

    public void ActivePlayerProject()
    {
        if (WorldController.currentPlayer.GetType() == typeof(HumanPlayer))
        {
            HumanPlayer player = (HumanPlayer)WorldController.currentPlayer;
            foreach (GameObject project in player.projectList)
            {
                project.SetActive(true);
            }
        }
    }

    public void ShowProjectDetail(string projectName, UnitProperty property, List<Building> developmentCenters)
    {
        projectNameText.text = projectName;

        maxHpText.text = property.maxHp.ToString();
        armorText.text = property.armor.ToString();
        damageText.text = property.damage.ToString();
        rangeText.text = property.range.ToString();
        speedText.text = property.speed.ToString();
        weightText.text = property.weight.ToString();

        maintanceCostText.text = property.maintanceCost.ToString();
        produceCostText.text = property.produceCost.ToString();

        ChangeTag(property);
        ChangeCity(developmentCenters);

        infoPanel.SetActive(true);
    }

    public List<TextMeshProUGUI> tagTextList;

    void ChangeTag(UnitProperty unitProperty)
    {
        List<UnitTag> unitTagsList = new List<UnitTag>();

        foreach (AccessoryProperty accessory in unitProperty.accessoryProperty)
        {
            if (accessory != null && !unitTagsList.Contains(accessory.accessoryTag) && accessory.accessoryTag != UnitTag.None)
            {
                unitTagsList.Add(accessory.accessoryTag);
            }
        }

        unitTagsList.Sort(UI_AccessoriesEquip.CompareListByUnitTag);
        for (int i = 0; i < tagTextList.Count; i++)
        {
            if (i < unitTagsList.Count)
            {
                tagTextList[i].text = unitTagsList[i].ToString();
                tagTextList[i].transform.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tagTextList[0].transform.parent.GetComponent<RectTransform>());
            }

            else
            {
                tagTextList[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public List<Image> cityIconList;
    public List<Button> removeButtonList;

    void ChangeCity(List<Building> developmentCenters)
    {
        for (int i = 0; i < cityIconList.Count; i++)
        {
            if (i < developmentCenters.Count)
            {
                //cityIconList[i].sprite = developmentCenters.icon;
                removeButtonList[i].gameObject.SetActive(true);
                cityIconList[i].gameObject.SetActive(true);
            }
            else
            {
                removeButtonList[i].gameObject.SetActive(false);
                cityIconList[i].gameObject.SetActive(false);
            }
        }
    }
}
