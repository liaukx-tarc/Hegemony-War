using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    //Turn Button
    public TextMeshProUGUI turnTxt;
    public Button turnBtn;
    public TextMeshProUGUI turnBtnTxt;

    //Unit UI
    public GameObject unitUI;
    public TextMeshProUGUI unitName;

    //Building UI
    public GameObject buildingUI;
    public TextMeshProUGUI buildingName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //Enable UI component
    public void Enable(GameObject ui)
    {
        ui.SetActive(true);
    }

    //Disable UI component
    public void Disable(GameObject ui)
    {
        ui.SetActive(false);
    }
}
