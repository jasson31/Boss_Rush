using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpiderBoss : Boss
{
    [SerializeField]
    private Bounds map;

    #region Phase1 Variables
    [SerializeField]
    private Vector3 shootPos;

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
    private float phase1MoveSpeed = 2;
    private CocoonLine curCocoonLine;

    [SerializeField]
    private Vector3 bitePos;
    [SerializeField]
    private float biteRange;

    private float biteAttackDelay = 10f;
    private float lastBiteAttackTime = -100f;
    #endregion


    #region Phase2 Variables
    private float phase2Height = -2.98f;
    [SerializeField]
    private float phase2MoveSpeed = 2;

    [SerializeField]
    private Spiderling spiderling;

    [SerializeField]
    private LineRenderer fallRoutineShadow;
    private bool isFallRoutine = false;

    [SerializeField]
    private LineRenderer legSpikeAlert;
    [SerializeField]
    private GameObject legSpike;
    private List<LineRenderer> legSpikeAlerts = new List<LineRenderer>();
    #endregion


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
                }
                else
                {
                    if (rand < 0.4f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebShootRoutine((GetPlayerPos() - transform.position).normalized)));
                    }
                    else if (rand < 0.7f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebTrapShootRoutine(10)));
                    }
                    else if (rand < 0.95f)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WebConeShootRoutine()));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(CocoonRoutine()));
                    }
                }
                nextRoutines.Enqueue(NewActionRoutine(Phase1IdleRoutine(0.5f)));
                break;
            case 1:
                /*if (rand < 0.3f)
                {

                }
                else if (rand < 0.45f)
                {

                }
                else if (rand < 0.7f)
                {

                }
                else if (rand < 0.9f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(LegSpikeRoutine()));
                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine(FallRoutine()));
                }*/
                nextRoutines.Enqueue(NewActionRoutine(SpawnSpiderlingRoutine(10)));
                nextRoutines.Enqueue(NewActionRoutine(Phase2IdleRoutine(2)));

                break;
        }


        return nextRoutines;
    }

    public override void GetDamaged(int damage)
    {
        base.GetDamaged(damage);


        //if (MaxHealth * 0.1f >= Health && Phase == 0)
        if (Phase == 0)
        {
            StartCoroutine(PhaseChangeRoutine());
        }
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator PhaseChangeRoutine()
    {
        Stun(3);

        StartCoroutine(MoveRoutine(new Vector2(map.center.x, map.min.y), 3));
        yield return new WaitForSeconds(3);
        animator.SetTrigger("Phase1To2");
        Phase = 1;
    }

    #region Phase1
    private IEnumerator WebShootRoutine(Vector3 dir)
    {
        StartCoroutine(WebShootRoutine(dir, webBulletSpeed));
        yield return null;
    }

    private IEnumerator WebShootRoutine(Vector3 dir, float speed)
    {
        animator.SetTrigger("WebShoot");
        WebBullet newWebBullet = Instantiate(webBullet, transform.position + shootPos, Quaternion.identity);
        newWebBullet.GetComponent<Rigidbody2D>().velocity = dir * speed;
        Destroy(newWebBullet.gameObject, 4);
        yield return null;
    }

    private IEnumerator WebTrapShootRoutine(int trapCount)
    {
        animator.SetTrigger("WebShoot");
        float webTrapBulletSpeed = 5;
        for (int i = 0; i < 5; i++)
        {
            float randPosX = Random.Range(map.min.x, map.max.x);
            float randPosY = Random.Range(map.min.y, map.max.y / 2);
            Vector2 trapPos = new Vector2(randPosX, randPosY);
            WebTrapBullet newWebTrapBullet = Instantiate(webTrapBullet, transform.position + shootPos, Quaternion.identity);
            newWebTrapBullet.GetComponent<Rigidbody2D>().velocity = ((Vector3)trapPos - transform.position).normalized * webTrapBulletSpeed;
        }
        yield return null;
    }

    private IEnumerator BiteRoutine()
    {
        animator.SetTrigger("Bite");
        player.GetDamaged(0.75f);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator WebConeShootRoutine()
    {
        float angle;
        Vector2 playerDir = (GetPlayerPos() - transform.position).normalized;

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
        yield return StartCoroutine(MoveRoutine(map.center, Vector3.Distance(map.center, transform.position) / phase1MoveSpeed));
        //Heal();

        animator.SetTrigger("CocoonEnter");
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
                Destroy(Instantiate(cocoonSpike, spikePos, Quaternion.Euler(0, 0, Mathf.Atan2(spikeDir.y, spikeDir.x) * Mathf.Rad2Deg - 90), transform), 2);

            }
        }
    }

    private IEnumerator Phase1IdleRoutine(float time)
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

            Vector3 dir = new Vector3(x, y, z).normalized * phase1MoveSpeed * moveTime;
            Vector3 dest = transform.position + dir;
            if (dest.x > map.max.x || dest.x < map.min.x) dest.x = transform.position.x - dir.x;
            if (dest.y > map.max.y || dest.y < map.min.y) dest.y = transform.position.y - dir.y;
            dest.x = Mathf.Clamp(dest.x, map.min.x, map.max.x);
            dest.y = Mathf.Clamp(dest.y, map.min.y, map.max.y);
            yield return MoveRoutine(dest, moveTime);
        }
    }
    #endregion

    #region Phase2

    private IEnumerator SpawnSpiderlingRoutine(int count)
    {


        for(int i = -count / 2; i < count / 2; i++)
        {
            Vector2 spawnPoint = new Vector2(transform.position.x + i * 0.5f, transform.position.y);
            Instantiate(spiderling, spawnPoint, Quaternion.identity);
            yield return null;

        }
    }

    private IEnumerator LegSpikeRoutine()
    {
        animator.SetTrigger("LegSpike");
        yield return new WaitForSeconds(1);

        for(int i = 0; i < 8; i++)
        {
            LineRenderer newLegSpikeAlert = Instantiate(legSpikeAlert, new Vector2(GetPlayerPos().x, map.min.y), Quaternion.identity);
            Vector2 legSpikePos = new Vector2(GetPlayerPos().x, map.min.y);
            newLegSpikeAlert.SetPosition(0, legSpikePos);
            newLegSpikeAlert.SetPosition(1, new Vector2(legSpikePos.x, map.max.y));
            legSpikeAlerts.Add(newLegSpikeAlert);
            yield return new WaitForSeconds(1);

            legSpikeAlerts.Remove(newLegSpikeAlert);
            Destroy(newLegSpikeAlert.gameObject);
            GameObject newLegSpike = Instantiate(legSpike, legSpikePos, Quaternion.identity);
            Destroy(newLegSpike, 1);
            yield return new WaitForSeconds(0.5f);
        }
        animator.SetTrigger("LegSpikeEnd");
    }

    private IEnumerator FallRoutine()
    {
        animator.SetTrigger("JumpUp");
        yield return new WaitForSeconds(0.24f);
        yield return MoveRoutine(new Vector2(transform.position.x, map.max.y + 5), 1);

        yield return new WaitForSeconds(0.5f);
        fallRoutineShadow.gameObject.SetActive(true);
        Vector2 fallPos = new Vector2(Random.Range(map.min.x, map.max.x), map.min.y);
        transform.position = new Vector2(fallPos.x, map.max.y);
        fallRoutineShadow.SetPosition(1, transform.position);
        fallRoutineShadow.SetPosition(0, fallPos);
        yield return new WaitForSeconds(2);

        isFallRoutine = true;
        fallRoutineShadow.gameObject.SetActive(false);
        animator.SetTrigger("JumpDown");
        yield return MoveRoutine(fallPos, 0.2f);
        transform.position = fallPos;
        isFallRoutine = false;
    }

    private IEnumerator Phase2IdleRoutine(float time)
    {
        for(float t = 0; t < time; t += Time.deltaTime)
        {
            Vector3 playerDir = GetPlayerPos() - transform.position;
            playerDir.y = 0;
            transform.position += playerDir.normalized * phase2MoveSpeed * Time.deltaTime;
            transform.localScale = new Vector2(1 * (playerDir.x > 0 ? 1 : -1), 1);
            yield return null;
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Phase == 1 && collision.GetComponent<Player>() != null)
        {
            if(isFallRoutine)
            {
                Game.inst.player.GetDamaged(0.75f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Phase == 1 && collision.GetComponent<Player>() != null)
        {
            if (!isFallRoutine)
            {
                Game.inst.player.GetDamaged(0.25f);
            }
        }
    }


    protected override void OnStunned()
    {
        if(curCocoonLine != null)
        {
            Destroy(curCocoonLine.gameObject);
        }
        foreach(var child in GetComponentsInChildren<CocoonSpike>())
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < legSpikeAlerts.Count; i++)
        {
            LineRenderer lr = legSpikeAlerts[0];
            legSpikeAlerts.Remove(lr);
            Destroy(lr);
        }
    }

    private void Awake()
    {
        MaxHealth = 100;
        Health = 100;
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

