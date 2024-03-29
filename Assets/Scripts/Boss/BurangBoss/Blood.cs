﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    private void Update()
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        if(direction.x >= 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(0, -1), direction));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -1 * Vector2.Angle(new Vector2(0, -1), direction));
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            float damage = GameObject.Find("BurangBoss").GetComponent<BurangBoss>().BossDamage;
            Game.inst.player.GetDamaged(damage);
        }
        else if (col.gameObject == GameObject.Find("Wall") || col.gameObject == GameObject.Find("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
