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
        if(Input.GetKeyDown(KeyCode.T))
        {
            TestSlowBuff a = new TestSlowBuff();
            a.Init(3);
            a.slowSpeed = 3;
           
            Game.inst.player.AddBuffable(a);
        }
    }
}
public class TestSlowBuff : Buffable
{
    public float speed;
    public float slowSpeed;

    public override void StartDebuff(Player player) { }

    public override void Apply(Player player)
    {
        player.speed = slowSpeed;
    }

    public override void EndDebuff(Player player)
    {
        player.speed = player.originSpeed;
    }
}
