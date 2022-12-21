using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject startMenu;

    public void OpenStartMenu()
    {
        startMenu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
