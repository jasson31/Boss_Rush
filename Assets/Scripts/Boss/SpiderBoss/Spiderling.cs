using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiderling : MonoBehaviour, IDamagable
{
    public float Health { get; protected set; }
    private bool isSelfExplode = false;
    public bool isStopped = false;

    private SpiderBoss spiderBoss;

    [SerializeField]
    private float moveSpeed;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if(isSelfExplode)
            {
                Game.inst.player.GetDamaged(0.75f);
            }
            else if(!isStopped)
            {
                Game.inst.player.GetDamaged(0.5f);
                GetComponent<Animator>().SetTrigger("Explode");
                spiderBoss.spiderlings.Remove(this);
                Destroy(gameObject, 0.2f);
            }
        }
    }

    private IEnumerator SelfExplode()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().speed = 1;
        isSelfExplode = true;
        GetComponent<Animator>().SetTrigger("Explode");
        spiderBoss.spiderlings.Remove(this);
        Destroy(gameObject, 0.2f);
    }

    public void HearScream()
    {
        StartCoroutine(SelfExplode());
    }

    public void GetDamaged(float damage)
    {
        if (!isStopped)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Animator>().SetTrigger("Explode");
            Destroy(gameObject, 0.2f);
        }
    }

    private void Start()
    {
        spiderBoss = FindObjectOfType<SpiderBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStopped)
        {
            Vector3 playerDir = Game.inst.player.transform.position - transform.position;
            playerDir.y = 0;
            transform.position += playerDir.normalized * moveSpeed * Time.deltaTime;
            transform.localScale = new Vector2(1 * (playerDir.x > 0 ? 1 : -1), 1);
        }
    }
}
