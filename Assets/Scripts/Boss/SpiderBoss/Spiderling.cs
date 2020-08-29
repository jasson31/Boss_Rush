using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiderling : MonoBehaviour, IDamagable
{
    public int Health { get; protected set; }
    [SerializeField]
    private float moveSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Game.inst.player.GetDamaged(0.5f);
            GetComponent<Animator>().SetTrigger("Explode");
            Destroy(gameObject, 0.2f);
        }
    }
    public void GetDamaged(int damage)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetTrigger("Explode");
        Destroy(gameObject, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerDir = Game.inst.player.transform.position - transform.position;
        playerDir.y = 0;
        transform.position += playerDir.normalized * moveSpeed * Time.deltaTime;
        transform.localScale = new Vector2(1 * (playerDir.x > 0 ? 1 : -1), 1);
    }
}
