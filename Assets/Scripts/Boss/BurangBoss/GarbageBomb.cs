﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBomb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == GameObject.Find("Player"))
        {
            float damage = GameObject.Find("BurangBoss").GetComponent<BurangBoss>().BossDamage;
            Game.inst.player.GetDamaged(damage);
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}