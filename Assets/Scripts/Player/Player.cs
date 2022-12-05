using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Data")]
    public List<UnitTemplate> unitTemplateList = new List<UnitTemplate>();

    //Unit List
    public List<Unit> unitList = new List<Unit>();
    public List<GameObject> projectList = new List<GameObject>();

    //Turn Function
    public WorldController.PlayerStartFunction playerStartFunction;
    public WorldController.PlayerEndFunction playerEndFunction;



    //Testing
    public int buildNum = 0;

    [Header("Prefabs")]
    public GameObject unitListObj;
    public GameObject buildingListObj;

    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    public UnitProperty[] unitProperties = new UnitProperty[10];

    private void Update()
    {
        for (int i = 0; i < unitTemplateList.Count; i++)
        {
            unitProperties[i] = unitTemplateList[i].property;
        }
    }
}
