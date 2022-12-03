using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected WorldController worldController;

    public List<UnitTemplate> unitTemplateList = new List<UnitTemplate>();

    //Unit List
    public List<Unit> unitList = new List<Unit>();

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
        worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();
    }
}
