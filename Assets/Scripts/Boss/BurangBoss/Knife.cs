using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject Boss = GameObject.Find("BurangBoss");
        if (col.GetComponent<Player>() != null)
        {
            if(Boss.GetComponent<BurangBoss>().IsRollDodgeable)
            {
                if(FindObjectOfType<Player>().GetComponent<Player>().isControllable)
                {
                    float damage = Boss.GetComponent<BurangBoss>().BossDamage;
                    //Game.inst.player.GetDamaged(damage);
                }
            }
            else
            {
                float damage = Boss.GetComponent<BurangBoss>().BossDamage;
                //Game.inst.player.GetDamaged(damage);
            }
        }
    }
}
