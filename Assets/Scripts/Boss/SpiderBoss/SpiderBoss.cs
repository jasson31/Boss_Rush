﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpiderBoss : Boss
{
    [SerializeField]
    private Bounds map;

    private float webBulletSpeed = 10;
    private float webConeBulletSpeed = 5;
    [SerializeField]
    private WebBullet webBullet;
    [SerializeField]
    private WebTrapBullet webTrapBullet;
    [SerializeField]
    private CocoonLine cocoonLine;
    [SerializeField]
    private GameObject cocoonSpike;
    [SerializeField]
    private List<Transform> cocoonSpikePos;
    [SerializeField]
    private float moveSpeed = 2;

    private CocoonLine curCocoonLine;

    [SerializeField]
    private Vector3 bitePos;
    [SerializeField]
    private float biteRange;


    private float biteAttackDelay = 10f;
    private float lastBiteAttackTime = -100f;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(map.center, map.size);
        Gizmos.DrawWireSphere(bitePos + transform.position, biteRange);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();
        float rand = Random.value;

        switch (Phase)
        {
            case 0:
                if(Vector2.Distance(bitePos + transform.position, player.transform.position) < biteRange && Time.time - lastBiteAttackTime > biteAttackDelay)
                {
                    lastBiteAttackTime = Time.time;
                    nextRoutines.Enqueue(NewActionRoutine(BiteRoutine()));
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(1)));
                }
                else
                {
                    if (rand < 0.4f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebShootRoutine((Game.inst.player.transform.position - transform.position).normalized)));
                        nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(1)));
                    }
                    else if (rand < 0.7f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebTrapShootRoutine(10)));
                        nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(2)));
                    }
                    else if (rand < 0.95f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebConeShootRoutine()));
                        nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(1)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(CocoonRoutine()));
                    }
                }
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(10)));
                break;
            case 1:
                break;
        }


        return nextRoutines;
    }

    #region Phase1
    private IEnumerator WebShootRoutine(Vector3 dir)
    {
        StartCoroutine(WebShootRoutine(dir, webBulletSpeed));
        yield return null;
    }

    private IEnumerator WebShootRoutine(Vector3 dir, float speed)
    {
        WebBullet newWebBullet = Instantiate(webBullet, transform.position, Quaternion.identity);
        newWebBullet.GetComponent<Rigidbody2D>().velocity = dir * speed;
        Destroy(newWebBullet.gameObject, 4);
        yield return null;
    }

    private IEnumerator WebTrapShootRoutine(int trapCount)
    {
        float webTrapBulletSpeed = 5;
        for (int i = 0; i < 5; i++)
        {
            float randPosX = Random.Range(map.min.x, map.max.x);
            float randPosY = Random.Range(map.min.y, map.max.y / 2);
            Vector2 trapPos = new Vector2(randPosX, randPosY);
            WebTrapBullet newWebTrapBullet = Instantiate(webTrapBullet, transform.position, Quaternion.identity);
            newWebTrapBullet.GetComponent<Rigidbody2D>().velocity = ((Vector3)trapPos - transform.position).normalized * webTrapBulletSpeed;
        }
        yield return null;
    }

    private IEnumerator BiteRoutine()
    {
        Debug.Log("Bite Attack");
        player.GetDamaged(0.75f);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator WebConeShootRoutine()
    {
        float angle;
        Vector2 playerDir = (Game.inst.player.transform.position - transform.position).normalized;

        for (int i = -2; i < 3; i++)
        {
            angle = 7 * i * Mathf.Deg2Rad;               
            Vector2 curDir = new Vector2(playerDir.x * Mathf.Cos(angle) - playerDir.y * Mathf.Sin(angle),
                                            playerDir.x * Mathf.Sin(angle) + playerDir.y * Mathf.Cos(angle)).normalized;
            StartCoroutine(WebShootRoutine(curDir, webConeBulletSpeed));
        }
        yield return null;
    }

    public IEnumerator CocoonRoutine()
    {
        yield return StartCoroutine(MoveRoutine(map.center, Vector3.Distance(map.center, transform.position) / moveSpeed));
        //Heal();
        curCocoonLine = Instantiate(cocoonLine, (new Vector3(map.center.x, map.max.y) + map.center) / 2, Quaternion.identity, transform);
        curCocoonLine.spiderBoss = this;
        while(true)
        {
            yield return new WaitForSeconds(3);
            List<Transform> posCands = new List<Transform>(cocoonSpikePos);
            for (int i = 0; i < 6; i++)
            {
                int rand = Random.Range(0, posCands.Count);
                Vector3 spikePos = posCands[rand].position;
                posCands.RemoveAt(rand);

                Vector3 spikeDir = (spikePos - transform.position).normalized;
                Destroy(Instantiate(cocoonSpike, spikePos, Quaternion.Euler(0, 0, Mathf.Atan2(spikeDir.y, spikeDir.x) * Mathf.Rad2Deg), transform), 2);

            }
        }
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

            Vector3 dir = new Vector3(x, y, z).normalized * moveSpeed * moveTime;
            Vector3 dest = transform.position + dir;
            if (dest.x > map.max.x || dest.x < map.min.x) dest.x = transform.position.x - dir.x;
            if (dest.y > map.max.y || dest.y < map.min.y) dest.y = transform.position.y - dir.y;
            dest.x = Mathf.Clamp(dest.x, map.min.x, map.max.x);
            dest.y = Mathf.Clamp(dest.y, map.min.y, map.max.y);
            yield return MoveRoutine(dest, moveTime);
        }
    }
    #endregion


    protected override void OnStunned()
    {
        Destroy(curCocoonLine.gameObject);
        foreach(var child in GetComponentsInChildren<CocoonSpike>())
        {
            Destroy(child.gameObject);
        }
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
        player.SetPlayerControllable(false);
    }

    public override void EndDebuff(Player player)
    {
        player.SetPlayerControllable(true);
    }
}

