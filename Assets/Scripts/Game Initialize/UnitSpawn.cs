using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitTemplatePrefab;
    public int unitNumber;

    public UnitProperty settlerProperty;

    Vector2 tempPos;

    public void GenerateUnit()
    {
        foreach (Player player in WorldController.instance.playerList)
        {
            GameObject tempTemplate = Instantiate(unitTemplatePrefab, WorldController.instance.uiController.unitTemplateListUI.unitTemplateListObj.transform);
            UnitTemplate unitTemplate = tempTemplate.GetComponent<UnitTemplate>();
            unitTemplate.CreateTemplate(settlerProperty, null);

            player.unitTemplateList.Add(unitTemplate);

            for (int i = 0; i < unitNumber; i++)
            {
                do
                {
                    //Random spawn unit at a position
                    tempPos = new Vector2(Random.Range(0, WorldController.instance.map.GetLength(0)),
                    Random.Range(0, WorldController.instance.map.GetLength(1)));

                } while (WorldController.instance.map[(int)tempPos.x, (int)tempPos.y].cost == 0 || WorldController.instance.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Coast ||
                             WorldController.instance.map[(int)tempPos.x, (int)tempPos.y].mapType == (int)MapTypeName.Ocean);

                WorldController.instance.unitController.GenerateUnit(player, settlerProperty, WorldController.instance.map[(int)tempPos.x, (int)tempPos.y]);
            }
        }
    }
}
