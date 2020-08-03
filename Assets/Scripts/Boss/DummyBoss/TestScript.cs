using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : SingletonBehaviour<TestScript>
{
    public Boss boss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Game.inst.player.GetComponent<Player>().GetDamaged(0);
        }
    }
}
