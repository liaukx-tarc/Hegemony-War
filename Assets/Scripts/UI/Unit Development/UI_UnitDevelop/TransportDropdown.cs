using TMPro;
using UnityEngine;

public class TransportDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI text;

    [Header("Transport List")]
    public TransportProperty[] vechicleTransports;
    public TransportProperty[] aircarftTransports;
    public TransportProperty[] shipTransports;

    TransportProperty[] transportList;

    void GenerateDropdownList(TransportProperty[] transportTypes)
    {
        foreach (TransportProperty item in transportTypes)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item.transportName });
        }

        transportList = transportTypes;

        dropdown.value = 0;
        text.text = transportList[dropdown.value].transportName;
        DropDownSelected(dropdown);
        dropdown.onValueChanged.AddListener(delegate { DropDownSelected(dropdown); });
    }

    public int index;

    public void DropDownSelected(TMP_Dropdown dropdown)
    {
        index = dropdown.value;
        WorldController.instance.uiController.accessoriesUI.ChangeTransport(transportList[index]);
    }

    public void onTransportTypeChange(TransportType transportType)
    {
        dropdown.options.Clear();

        switch (transportType)
        {
            case TransportType.Vechicle:
                GenerateDropdownList(vechicleTransports);
                break;

            case TransportType.Aircarft:
                GenerateDropdownList(aircarftTransports);
                break;

            case TransportType.Ship:
                GenerateDropdownList(shipTransports);
                break;
        }
    }
}
