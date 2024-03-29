﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebTrapBullet : MonoBehaviour
{
    [SerializeField]
    private GameObject webTrap;
    private bool dealToPlayer = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") || collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Destroy(Instantiate(webTrap, transform.position - new Vector3(0, 0.2f), Quaternion.identity).AddComponent<WebTrap>().gameObject, 10);
            Destroy(gameObject);
        }
        if(dealToPlayer && collision.GetComponent<Player>() != null)
        {
            Game.inst.player.GetDamaged(0.25f);
            dealToPlayer = false;
        }
    }
}

public class WebTrap : MonoBehaviour
{
    private bool activated = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && !collision.GetComponent<Player>().isInvincible && activated)
        {
            SpiderBossStunDebuff debuff = new SpiderBossStunDebuff();
            debuff.Init(2);
            Game.inst.player.AddBuffable(debuff);
            activated = false;
            Destroy(gameObject, 2);
        }
    }
}
