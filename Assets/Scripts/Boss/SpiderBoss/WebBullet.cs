using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            SpiderBossSlowDebuff debuff = new SpiderBossSlowDebuff();
            debuff.Init(3);
            debuff.slowSpeed = 3;
            Game.inst.player.GetComponent<Player>().AddBuffable(debuff);
            Game.inst.player.GetComponent<Player>().GetDamaged(0.5f);
            Destroy(gameObject);
        }
    }
}
