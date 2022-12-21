using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTemplate: MonoBehaviour
{
    public UnitProperty property;
    public TagController.UnitFunction runTimeFunction;

    public Image iconBackground;
    public Image icon;
    public TextMeshProUGUI unitNameText;

    public TextMeshProUGUI maxHpText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI speedText;

    public TextMeshProUGUI maintanceCostText;
    public TextMeshProUGUI produceCostText;

    public void CreateTemplate(UnitProperty property, TagController.UnitFunction runTimeFunction)
    {
        this.property = property;
        this.runTimeFunction = runTimeFunction;

        unitNameText.text = property.unitName;
        switch (property.transportProperty.transportType)
        {
            case TransportType.Vechicle:
                iconBackground.color = WorldController.instance.uiController.vechicleColor;
                break;

            case TransportType.Aircarft:
                iconBackground.color = WorldController.instance.uiController.aircarftColor;
                break;

            case TransportType.Ship:
                iconBackground.color = WorldController.instance.uiController.shipColor;
                break;
        }

        icon.sprite = property.unitIcon;

        maxHpText.text = property.maxHp.ToString();
        armorText.text = property.armor.ToString();
        damageText.text = property.damage.ToString();
        rangeText.text = property.range.ToString();
        speedText.text = property.speed.ToString();

        maintanceCostText.text = property.maintanceCost.ToString();
        produceCostText.text = property.produceCost.ToString();
    }


    public void UpdateTemplateInfo()
    {
        icon.sprite = property.unitIcon;

        maxHpText.text = property.maxHp.ToString();
        armorText.text = property.armor.ToString();
        damageText.text = property.damage.ToString();
        rangeText.text = property.range.ToString();
        speedText.text = property.speed.ToString();

        maintanceCostText.text = property.maintanceCost.ToString();
        produceCostText.text = property.produceCost.ToString();
    }

    public void UnitTemplateDetail()
    {
        WorldController.instance.uiController.ClickSound();
        WorldController.instance.uiController.unitTemplateListUI.ShowTemplateInfo(this);
    }
}
