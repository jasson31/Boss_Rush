using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBoss : Boss
{
    [SerializeField]
    private Bounds map;

    [SerializeField]
    private WebBullet webBullet;
    [SerializeField]
    private WebTrapBullet webTrapBullet;
    private float moveSpeed = 2;


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
                if (rand < 0.2f)
                {
                }
                nextRoutines.Enqueue(NewActionRoutine(WebTrapShootRoutine(5)));
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(2)));
                break;
        }


        return nextRoutines;
    }

    private IEnumerator WebShootRoutine()
    {
        float webBulletSpeed = 10;
        Vector3 dir = (Game.inst.player.position - transform.position).normalized;
        WebBullet newWebBullet = Instantiate(webBullet, transform.position, Quaternion.identity);
        newWebBullet.GetComponent<Rigidbody2D>().velocity = dir * webBulletSpeed;
        Destroy(newWebBullet, 5);
        yield return null;
    }

    private IEnumerator WebTrapShootRoutine(int trapCount)
    {
        float webTrapBulletSpeed = 5;
        for (int i = 0; i < 5; i++)
        {
            float randPos = Random.Range(map.min.x, map.max.x);
            Vector2 trapPos = new Vector2(randPos, map.min.y);
            WebTrapBullet newWebTrapBullet = Instantiate(webTrapBullet, transform.position, Quaternion.identity);
            newWebTrapBullet.GetComponent<Rigidbody2D>().velocity = ((Vector3)trapPos - transform.position).normalized * webTrapBulletSpeed;
        }
        yield return null;
    }

    private IEnumerator IdleRoutine(float time)
    {
        float moveTime;
        for (float t = 0; t < time; t += moveTime)
        {
            moveTime = Random.Range(0.5f, 2f);
            if (t + moveTime > time)
            {
                moveTime = time - t + float.Epsilon;
            }

            float x = Random.Range(map.min.x, map.max.x);
            float y = Random.Range(map.min.y, map.max.y);
            float z = Random.Range(map.min.z, map.max.z);
            yield return MoveRoutine(new Vector3(x, y, z), Vector2.Distance(transform.position, new Vector2(x, y)) / moveSpeed);
        }
    }


    protected override void OnStunned()
    {

    }
}

public class SpiderBossSlowDebuff : Buffable
{
    public float speed;
    public float slowSpeed;

    public override void StartDebuff(Player player) { }

    public override void Apply(Player player)
    {
        player.speed = slowSpeed;
    }

    public override void EndDebuff(Player player)
    {
        player.speed = player.originSpeed;
    }
}

public class SpiderBossStunDebuff : Buffable
{
    public override void StartDebuff(Player player)
    {
        player.StopPlayer();
    }

    public override void Apply(Player player)
    {
        player.isControllable = false;
    }

    public override void EndDebuff(Player player)
    {
        player.isControllable = true;
    }
}

