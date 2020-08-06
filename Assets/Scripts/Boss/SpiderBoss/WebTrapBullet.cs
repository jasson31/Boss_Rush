using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebTrapBullet : MonoBehaviour
{
    [SerializeField]
    private GameObject webTrap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Destroy(Instantiate(webTrap, transform.position, Quaternion.identity).AddComponent<WebTrap>().gameObject, 10);
            Destroy(gameObject);
        }
    }
}

public class WebTrap : MonoBehaviour
{
    private bool activated = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && activated)
        {
            SpiderBossStunDebuff debuff = new SpiderBossStunDebuff();
            debuff.Init(2);
            Game.inst.player.GetComponent<Player>().AddBuffable(debuff);
            activated = false;
            Destroy(gameObject, 2);
        }
    }
}
