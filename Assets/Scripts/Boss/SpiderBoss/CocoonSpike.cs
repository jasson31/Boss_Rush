using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoonSpike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Game.inst.player.GetDamaged(0.5f);
        }
    }
}
