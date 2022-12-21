using System.Collections.Generic;
using UnityEngine;

public class PlayerCreate : MonoBehaviour
{
    public Transform playerListObj;

    [Space]
    public GameObject humanPlayer;
    public int humanPlayerNum;
    public List<string> humanPlayerNames = new List<string>();
    public List<Color> humanPlayerColors = new List<Color>();

    private void Awake()
    {
        humanPlayerNum = StartMenu.playerNum;
        humanPlayerNames = StartMenu.playerNameList;
        humanPlayerColors = StartMenu.playerColorList;
    }

    public void CreatePlayer()
    {
        GameObject empty = new GameObject();

        for (int i = 0; i < humanPlayerNum; i++)
        {
            HumanPlayer player = Instantiate(humanPlayer, playerListObj).GetComponent<HumanPlayer>();
            WorldController.instance.playerList.Add(player);
            
            player.playerName = player.gameObject.name = humanPlayerNames[i];
            player.playerColor = humanPlayerColors[i];

            player.unitListObj = Instantiate(empty, player.transform);
            player.unitListObj.name = "UnitList";
        }
    }
}