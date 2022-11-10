using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreate : MonoBehaviour
{
    public GameObject aiPlayer;
    public int aiPlayerNum;

    GameObject tempPlayer;

    public void CreatePlayer()
    {
        GameObject playerList = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;

        for (int i = 0; i < aiPlayerNum; i++)
        {
            tempPlayer = Instantiate(aiPlayer, new Vector3(0, 0, 0), Quaternion.identity, playerList.transform);
            tempPlayer.name = "AI Player " + (i + 1);
            WorldController.playerList.Add(tempPlayer.GetComponent<Player>());
        }

        GetComponentInParent<UnitSpawn>().GenerateUnit();
    }
}
