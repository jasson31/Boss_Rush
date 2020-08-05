using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBoss : Boss
{
    [SerializeField]
    private Bounds map;

    [SerializeField]
    private GameObject webBullet;



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(map.center, map.size);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();
        float rand = Random.value;


        switch (Phase)
        {
            case 0:
                nextRoutines.Enqueue(NewActionRoutine(WebShootRoutine()));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(2)));
                if (rand < 0.2f)
                {
                }
                break;
        }


        return nextRoutines;
    }


    private IEnumerator WebShootRoutine()
    {
        float webBulletSpeed = 10;
        Vector3 dir = (Game.inst.player.position - transform.position).normalized;
        GameObject newWebBullet = Instantiate(webBullet, transform.position, Quaternion.identity);
        newWebBullet.GetComponent<Rigidbody2D>().velocity = dir * webBulletSpeed;
        newWebBullet.AddComponent<WebBullet>();
        Destroy(newWebBullet, 5);
        yield return null;
    }


    protected override void OnStunned()
    {

    }
}

class WebBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
