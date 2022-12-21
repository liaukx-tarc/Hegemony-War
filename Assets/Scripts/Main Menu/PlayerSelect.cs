using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Image playerColor; 

    public void RemovePlayer()
    {
        StartMenu.instance.colorList.Add(playerColor.color);
        StartMenu.instance.playerSelectsList.Remove(this);
        
        StartMenu.playerNum--;
        Destroy(gameObject);
    }
}
