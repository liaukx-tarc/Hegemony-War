using UnityEngine;

public class PlayerCreate : MonoBehaviour
{
    public GameObject aiPlayer;
    public int aiPlayerNum;

    GameObject tempPlayer;

    public void CreatePlayer()
    {
        //Adding the human player to playerlist
        foreach (GameObject humanPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            WorldController.playerList.Add(humanPlayer.GetComponent<HumanPlayer>());
        }
        
        GameObject playerListObj = WorldController.playerList[0].transform.parent.gameObject;

        //Create AI player
        for (int i = 0; i < aiPlayerNum; i++)
        {
            tempPlayer = Instantiate(aiPlayer, new Vector3(0, 0, 0), Quaternion.identity, playerListObj.transform);
            tempPlayer.name = "AI Player " + (i + 1);
            WorldController.playerList.Add(tempPlayer.GetComponent<AI_Player>());
        }

        //Create unit list and building list for all player
        GameObject empty = new GameObject();
        foreach (Player player in WorldController.playerList)
        {
            player.unitListObj = Instantiate(empty, player.transform);
            player.unitListObj.name = "UnitList";

            player.buildingListObj = Instantiate(empty, player.transform);
            player.buildingListObj.name = "BuildingList";
        }
    }
}