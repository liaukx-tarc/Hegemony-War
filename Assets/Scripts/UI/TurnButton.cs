using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    public TextMeshProUGUI turnTxt;
    public Button turnBtn;
    public TextMeshProUGUI turnBtnTxt;

    string activeTxt = "NEXT\nUNIT";
    Color activeColor = new Color(0.08f, 0.3f, 1);
    Color activeSelectColor = new Color(0.08f, 0.6f, 1);

    string nextTxt = "END\nTURN";
    Color nextColor = new Color(0.95f, 0.75f, 0.08f);
    Color nextSelectColor = new Color(0.98f, 0.85f, 0.45f);

    private void Update()
    {
        if (WorldController.activeUnitList.Count != 0)
        {
            ColorBlock turnBtnColors = turnBtn.colors;
            turnBtnColors.normalColor = turnBtnColors.disabledColor = turnBtnColors.selectedColor = turnBtnColors.pressedColor = activeColor;
            turnBtnColors.highlightedColor = activeSelectColor;
            turnBtnColors.disabledColor = Color.gray;
            turnBtn.colors = turnBtnColors;
            turnBtnTxt.text = activeTxt;
            turnBtnTxt.color = Color.white;
        }

        else
        {
            ColorBlock turnBtnColors = turnBtn.colors;
            turnBtnColors.normalColor = turnBtnColors.disabledColor = turnBtnColors.selectedColor = turnBtnColors.pressedColor = nextColor;
            turnBtnColors.highlightedColor = nextSelectColor;
            turnBtnColors.disabledColor = Color.gray;
            turnBtn.colors = turnBtnColors;
            turnBtnTxt.text = nextTxt;
            turnBtnTxt.color = Color.black;
        }
    }

    public void ChangeTurnText()
    {
        turnTxt.text = "TURN " + WorldController.turn;
    }
}
