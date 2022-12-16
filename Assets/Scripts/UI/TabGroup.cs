using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<Tab_Button> buttonsList;
    public Tab_Button startButton;

    Tab_Button selectedButton;

    public Color enterColor;
    public Color exitColor;
    public Color selectedColor;

    private void Start()
    {
        selectedButton = startButton;
        startButton.background.color = selectedColor;
    }

    public void Subscribe(Tab_Button button)
    {
        if(buttonsList == null)
            buttonsList = new List<Tab_Button>();

        buttonsList.Add(button);
    }

    public void OnTabButtonEnter(Tab_Button button)
    {
        if (button != selectedButton)
            button.background.color = enterColor;
    }

    public void OnTabButtonExit(Tab_Button button)
    {
        if (button != selectedButton)
            button.background.color = exitColor;
    }

    public void OnTabButtonSelected(Tab_Button button)
    {
        if(selectedButton != null)
        {
            selectedButton.background.color = exitColor;
            selectedButton.ui.SetActive(false);
        }

        selectedButton = button;
        button.background.color = selectedColor;
        button.ui.SetActive(true);
    }
}
