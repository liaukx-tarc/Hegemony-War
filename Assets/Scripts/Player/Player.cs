using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected WorldController worldController;

    public List<Unit> unitList = new List<Unit>();
    public int buildNum = 0;

    // Start is called before the first frame update
    protected void Start()
    {
        worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }
}
