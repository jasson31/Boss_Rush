using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBullet : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<Player>() != null && !collision.GetComponent<Player>().isInvincible)
		{
			Game.inst.player.GetDamaged(0.25f);
			Destroy(gameObject);
		}
	}
}
