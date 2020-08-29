using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            UnityEngine.Debug.Log("Boom");
            float damage = GameObject.Find("BurangBoss").GetComponent<BurangBoss>().BossDamage;

            BurangBossStunDebuff debuff = new BurangBossStunDebuff();
            debuff.Init(2);
            Game.inst.player.AddBuffable(debuff);//플레이어 스턴

            Game.inst.player.GetDamaged(damage);
        }
    }

    private void OnDisable()
    {
        //독데미지 장판 소환
    }
}
