using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            float damage = GameObject.Find("BurangBoss").GetComponent<BurangBoss>().BossDamage;
            Game.inst.player.GetDamaged(damage);
        }
    }
}
