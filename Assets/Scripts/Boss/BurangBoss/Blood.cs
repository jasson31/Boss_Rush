using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            float damage = GameObject.Find("BurangBoss").GetComponent<BurangBoss>().BossDamage;
            //Game.inst.player.GetDamaged(damage);
        }
        else if (col.gameObject == GameObject.Find("Wall") || col.gameObject == GameObject.Find("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
