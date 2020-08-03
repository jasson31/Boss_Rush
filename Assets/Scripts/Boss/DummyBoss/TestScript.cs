using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : SingletonBehaviour<TestScript>
{
    public Boss boss;
    public GameObject cursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cursor.GetComponent<RectTransform>().position = Input.mousePosition;
        if(Input.GetKeyDown(KeyCode.S))
        {
            Game.inst.player.GetComponent<Player>().GetDamaged(0);
        }
    }
}
