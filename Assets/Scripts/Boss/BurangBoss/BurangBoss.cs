using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BurangBoss : Boss
{
    [SerializeField]
    private GameObject ThrowingknifePrefab;

    [SerializeField]
    private GameObject garbagePrefab;

    [SerializeField]
    private GameObject garbagePrefabV2;

    [SerializeField]
    private GameObject garbagePrefabV3;

    [SerializeField]
    private GameObject garbageAttachedPrefab;

    [SerializeField]
    private GameObject shadowPrefab;

    [SerializeField]
    private GameObject BurangBossCol;

    [SerializeField]
    private GameObject DashKnife;

    [SerializeField]
    private GameObject DiagonalDashKnife;

    [SerializeField]
    private GameObject NearAttack;

    [SerializeField]
    private GameObject Blood;

    [SerializeField]
    private GameObject Explosion;

    [SerializeField]
    private LayerMask wallfloorLayerMask;

    [SerializeField]
    private LayerMask WFPLayerMask;

    [SerializeField]
    private float NearDist;

    bool Invincible = false;

    bool bombArmor = false;
    
    bool IsHit = false;

    [HideInInspector]
    public bool IsRollDodgeable = false;

    Animator ani;
    SpriteRenderer rend;

    Transform Parent;
    Transform PlayerParent;

    private int FirstPhase2;//페이즈 시작 특별 패턴
    private int FirstPhase3;

    [HideInInspector]
    public float BossDamage;

    protected override void Start()
    {
        base.Start();
        Phase = 0;
        //Health = MaxHealth * 0.5f;
        Parent = GameObject.Find("BurangBoss").GetComponent<Transform>();
        PlayerParent = GameObject.Find("Player").GetComponent<Transform>();
        ani = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rend.flipX = false;
        FirstPhase2 = 1;
        FirstPhase3 = 1;
        BossDamage = 0.25f;
        ani.SetInteger("Phase", 0);
    }

    private float XDist()
    {
        return GetPlayerPos().x - transform.position.x;
    }

    private float YDist()
    {
        return GetPlayerPos().y - transform.position.y;
    }

    private bool BossDir()
    {
        if (XDist() >= 0)
        {
            rend.flipX = false;
            return true;
        }
        else
        {
            rend.flipX = true;
            return false;
        }
    }

    public override void GetDamaged(float damage)
    {
        if (!Invincible) { base.GetDamaged(damage); }
        else if(bombArmor){ IsHit = true; }
        if (MaxHealth * 0.1f >= Health && Phase == 0)
        {
            Phase = 1;
            ani.SetInteger("Phase", 1);
            Health = MaxHealth * 0.3f;
        }
        if (MaxHealth * 0.1f >= Health && Phase == 1)
        {
            Phase = 2;
            ani.SetInteger("Phase", 2);
            Health = MaxHealth * 0.2f;
        }
        if(Phase == 2)
        {
            int i;
            for (i = 10; Health < (MaxHealth * 0.2f) / 10 * i; i--)
            BossDamage = 0.25f + 0.25f * (10 - i);
        }
        if (Health <= 0 && Phase == 2) //dead
        {
            UnityEngine.Debug.Log("Boss Defeated!");
            gameObject.SetActive(false);
        }
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();
        float rand = UnityEngine.Random.value;

        Vector3 BossPos = transform.position;
        Vector3 playerPos = GetPlayerPos();
        Vector3 dist = BossPos - playerPos;

        switch (Phase)
        {
            case 0:
                ani.SetInteger("Phase", 0);
                if (rand < 0.33f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DashStab(0.5f, 50f, 0.25f)));
                }
                else if(rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DiagonalStab(0.1f, 15f, 0.8f, 50f)));
                }
                else
                {
                    if(dist.magnitude < NearDist)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(ChargeTripleStab(0.5f)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(GarbageBomb(0.5f, 3, 2f, 0.1f, 2f, 1f)));
                    }
                }
                float idleTime1 = UnityEngine.Random.Range(1.2f, 1.7f);
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(idleTime1,3f)));
                break;
            case 1:
                ani.SetInteger("Phase", 1);
                {
                    if (FirstPhase2 == 1)//특별 패턴
                    {
                        nextRoutines.Enqueue(NewActionRoutine(Phase1to2()));
                        nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(0)));
                    }
                    else
                    {
                        if(rand < 0.25f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(0.5f)));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1f)));
                        }
                        else if(rand < 0.5f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ThrowTeleportDashStab(0.1f,20f,50f,0.25f)));
                        }
                        else if(rand < 0.75f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(DashStab(0.5f, 50f, 0.25f)));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.1f)));
                            nextRoutines.Enqueue(NewActionRoutine(DashStab(0.5f, 50f, 0.25f)));
                        }
                        else
                        {
                            nextRoutines.Enqueue(NewActionRoutine(GarbageBombArmor(3f)));
                        }
                    }
                    float idleTime2 = UnityEngine.Random.Range(1.7f, 2.2f);
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(idleTime2,3f)));
                }
                break;
            case 2:
                ani.SetInteger("Phase", 2);
                {
                    if (FirstPhase3 == 1)//특별 패턴
                    {
                        nextRoutines.Enqueue(NewActionRoutine(Phase2to3()));
                    }
                    else
                    {
                        if(playerPos.x <= -8 || playerPos.x >= 8)
                        {
                            if (rand < 0.25f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb(0.1f, 50f, 50f, 2f)));
                            }
                            else if (rand < 0.5f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit(30f)));
                            }
                            else if (rand < 0.75f)
                            {
                                if (dist.magnitude < NearDist)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance(0.1f, 1.5f)));
                                }
                                else
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(Uppercut(50f)));
                                }
                            }
                            else
                            {
                                nextRoutines.Enqueue(NewActionRoutine(WallThump(30f)));
                            }
                        }
                        else
                        {
                            if(rand < 0.33f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb(0.1f,50f,50f,2f)));
                            }
                            else if(rand < 0.66f && Health > MaxHealth * 0.03)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit(30f)));
                            }
                            else
                            {
                                if(dist.magnitude < NearDist)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance(1f, 1.5f)));
                                }
                                else
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(Uppercut(30f)));
                                }
                            }
                        }
                    }
                    float idleTime3 = UnityEngine.Random.Range(2.2f, 2.5f);
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(idleTime3,2f)));
                }
                break;
        }
        return nextRoutines;
    }
    private IEnumerator Phase1to2()
    {
        Invincible = true;
        ani.SetBool("Phase1to2", true);
        BossDir();
        yield return new WaitForSeconds(2f);
        ani.SetBool("Phase1to2", false);
        FirstPhase2 = 0;
        Invincible = false;
    }

    private IEnumerator Phase2to3()
    {
        Invincible = true;
        ani.SetTrigger("Phase2to3");
        BossDir();
        yield return new WaitForSeconds(3f);
        ani.SetTrigger("Phase3Idle");
        FirstPhase3 = 0;
        Invincible = false;
    }

    private IEnumerator DashStab(float waitTime, float dashSpeed, float dashTime)
    {
        IsRollDodgeable = true;

        ani.SetBool("DashStab", true);

        bool bDir = BossDir();

        Vector2 direction = new Vector2(XDist(), 0);

        yield return new WaitForSeconds(waitTime);//경고 동작

        GameObject child = Instantiate(DashKnife, transform.position, Quaternion.identity) as GameObject;
        if(!bDir)
        {
            child.transform.localScale = new Vector3(-1, 1, 1);
        }
        child.transform.parent = Parent;

        ani.SetTrigger("DashStab_1");

        direction.Normalize();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector2.zero;
        Destroy(child);

        ani.SetBool("DashStab", false);

        IsRollDodgeable = false;
    }

    private IEnumerator DiagonalStab(float waitTime, float jumpSpeed, float jumpTime, float dashSpeed)
    {
        IsRollDodgeable = true;

        yield return new WaitForSeconds(waitTime);//점프 준비 동작

        ani.SetBool("DiagonalStab", true);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, jumpSpeed);

        yield return new WaitForSeconds(jumpTime);

        GameObject child = Instantiate(DiagonalDashKnife, transform.position, Quaternion.identity) as GameObject;
        if (BossDir())
        {
            child.transform.rotation = Quaternion.Euler(0, 0, -37.64f);
        }
        else
        {
            child.transform.rotation = Quaternion.Euler(0, 0, 37.64f);
            child.transform.localScale = new Vector3(-1, 1, 1);
        }
        child.transform.parent = Parent;

        Vector2 direction;
        if(XDist()>=0)
        {
            direction = new Vector2(2, -1);
        }
        else
        {
            direction = new Vector2(-2, -1);
        }
        rb.velocity = direction.normalized * dashSpeed;

        var hit = Physics2D.Raycast(transform.position, direction, float.MaxValue, wallfloorLayerMask);
        Collider2D col = GetComponent<Collider2D>();

        while (!col.IsTouching(hit.collider))
        {
            yield return null;
        }

        rb.velocity = Vector2.zero;

        Destroy(child);

        ani.SetBool("DiagonalStab", false);

        IsRollDodgeable = false;
    }

    private IEnumerator ChargeTripleStab(float waitTime)
    {
        bool bDir = BossDir();

        ani.SetBool("ChargeTripleStab", true);

        yield return new WaitForSeconds(waitTime);

        ani.SetTrigger("ChargeTripleStab_1");
        //찌르기 모션 수정 필요
        GameObject child1 = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if(!bDir)
        {
            child1.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(child1);
        GameObject child2 = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if (!bDir)
        {
            child2.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(child2);
        GameObject child3 = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if (!bDir)
        {
            child3.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(child3);

        ani.SetBool("ChargeTripleStab", false);
    }

    private IEnumerator GarbageBomb(float waitTime, int count, float ArriveTime, float intervalTime, float intervalDistance, float ExplodeTime)
    {
        BossDir();

        ani.SetBool("GarbageBomb", true);

        yield return new WaitForSeconds(waitTime);

        ani.SetTrigger("GarbageBomb_1");
        float d = XDist();
        if(d >= 0)
        {
            if(count % 2 == 0) { d -= intervalDistance * ((float)count / 2 - 0.5f); }
            else { d -= intervalDistance * ((float)(count - 1) / 2); }
        }
        else
        {
            if(count % 2 == 0) { d += intervalDistance * ((float)count / 2 - 0.5f); }
            else { d += intervalDistance * ((float)(count - 1) / 2); }
        }

        GameObject[] prefab = new GameObject[3];
        for (int i = 0; i < count; i++)
        {
            prefab[i] = Instantiate(garbagePrefab, transform.position, Quaternion.identity) as GameObject;
            prefab[i].GetComponent<Rigidbody2D>().velocity = new Vector2(2 * d / ArriveTime, 9.8f * ArriveTime / 2);
            if(d>=0) { d += intervalDistance; }
            else { d -= intervalDistance; }
            yield return new WaitForSeconds(intervalTime);
        }

        ani.SetBool("GarbageBomb", false);

        yield return new WaitForSeconds(ArriveTime/2 + ExplodeTime);

        for(int i = 0; i < count; i++)
        {
            if(prefab[i] != null)
            {
                Destroy(prefab[i]);
                GameObject explode = Instantiate(Explosion, prefab[i].transform.position, Quaternion.identity) as GameObject;
                Destroy(explode, 0.5f);
            }
            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator ShadowTeleportStab(float waitTime)
    {
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;

        Invincible = true;
        GameObject shadowEnter = Instantiate(shadowPrefab, transform.position - new Vector3(0, 1.3f, 0), Quaternion.identity);
        yield return new WaitForSeconds(waitTime);
        Vector3 teleportPos = GetPlayerPos() - new Vector3(0, 0.8f, 0);
        GameObject shadowExit = Instantiate(shadowPrefab, teleportPos, Quaternion.identity);

        ani.SetBool("ShadowTeleportStab", true);

        yield return new WaitForSeconds(0.25f);
        shadowEnter.GetComponent<Animator>().SetTrigger("ShadowEnd");
        yield return new WaitForSeconds(0.25f);
        transform.position = teleportPos + new Vector3(0,1.2f,0);
        Destroy(shadowEnter);
        shadowExit.GetComponent<Animator>().SetTrigger("ShadowEnd");
        yield return new WaitForSeconds(0.2f);
        Destroy(shadowExit);

        Invincible = false;
        bool bDir = BossDir();
        //찌르기 모션 수정 필요
        GameObject child1 = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if(!bDir)
        {
            child1.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(child1);
        GameObject child2 = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if (!bDir)
        {
            child2.transform.localScale = new Vector3(-1, 1, 1);
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(child2);

        ani.SetBool("ShadowTeleportStab", false);

        GetComponent<Rigidbody2D>().gravityScale = 1.0f;
    }

    private IEnumerator ThrowTeleportDashStab(float waitTime, float knifeSpeed, float dashSpeed, float dashTime)
    {
        Invincible = true;
        yield return new WaitForSeconds(waitTime);

        BossDir();
        ani.SetBool("ThrowTeleportDashStab", true);
        yield return new WaitForSeconds(0.1f);
        GameObject prefab = Instantiate(ThrowingknifePrefab, transform.position, Quaternion.identity);
        Vector2 direction = new Vector2(XDist(), YDist()).normalized;

        float ang = Vector2.Angle(new Vector2(1,0),direction);
        if (ang>90 && ang<270)
        {
            prefab.transform.localScale = new Vector3(-2, 2, 1);
            prefab.transform.rotation = Quaternion.Euler(0, 0,-45 + Vector2.Angle(new Vector2(-1, 0), direction));
        }
        else
        {
            prefab.transform.rotation = Quaternion.Euler(0, 0,45 + Vector2.Angle(new Vector2(1, 0), direction));
        }

        prefab.GetComponent<Rigidbody2D>().velocity = direction * knifeSpeed;


        var hit = Physics2D.Raycast(prefab.transform.position, direction, float.MaxValue, wallfloorLayerMask);
        Collider2D col = prefab.GetComponent<Collider2D>();

        while (!col.IsTouching(hit.collider))
        {
            hit = Physics2D.Raycast(prefab.transform.position, prefab.GetComponent<Rigidbody2D>().velocity, float.MaxValue, wallfloorLayerMask);
            yield return null;
        }

        prefab.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        ani.SetTrigger("ThrowTeleportDashStabCol");
        yield return new WaitForSeconds(0.1f);
        Vector3 Pos = Vector3.zero;
        if(prefab.transform.position.y >= 6)
        {
            Pos += new Vector3(0, -1.5f, 0);
        }
        else
        {
            Pos += new Vector3(0, 1.5f, 0);
        }
        if(prefab.transform.position.x > 13)
        {
            Pos += new Vector3(-1f, 0, 0);
        }
        else if(prefab.transform.position.x < -13)
        {
            Pos += new Vector3(1f, 0, 0);
        }
        transform.position = prefab.transform.position + Pos;
        Destroy(prefab);

        Invincible = false;

        BossDir();
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        yield return new WaitForSeconds(0.5f);
        bool bDir = BossDir();

        GameObject child = Instantiate(DashKnife, transform.position, Quaternion.identity) as GameObject;
        if(!bDir)
        {
            child.transform.localScale = new Vector3(-1, 1, 1);
        }
        child.transform.parent = Parent;

        Vector2 dashDir = new Vector2(XDist(), YDist()).normalized;
        if (bDir)
        {
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(1, 0),dashDir));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(-1, 0),dashDir));
        }
        
        GetComponent<Rigidbody2D>().velocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Destroy(child);

        ani.SetBool("ThrowTeleportDashStab", false);

        transform.rotation = Quaternion.identity;

        GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator GarbageBombArmor(float invincibleTime)
    {
        BossDir();

        ani.SetBool("GarbageBombArmor", true);
        yield return new WaitForSeconds(0.5f);
        Invincible = true;
        bombArmor = true;
        for(float i = 0.1f;invincibleTime > i;i+=0.1f)
        {
            if(IsHit)
            {
                GameObject explode = Instantiate(Explosion, GetPlayerPos(), Quaternion.identity) as GameObject;
                Destroy(explode, 0.5f);
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        Invincible = false;
        ani.SetBool("GarbageBombArmor", false);

        GameObject explode1 = Instantiate(Explosion, transform.position, Quaternion.identity) as GameObject;
        Destroy(explode1, 0.5f);
        GameObject explode2 = Instantiate(Explosion, transform.position + new Vector3(-0.6f, -0.3f, 0), Quaternion.identity) as GameObject;
        Destroy(explode2, 0.5f);
        GameObject explode3 = Instantiate(Explosion, transform.position + new Vector3(0.6f, -0.3f, 0), Quaternion.identity) as GameObject;
        Destroy(explode3, 0.5f);

        if (!IsHit)
        {
            GetDamaged(10);
            yield return StunRoutine(2f);
        }
        IsHit = false;
        bombArmor = false;
    }

    private IEnumerator AttachGarbageBomb(float waitTime, float dashSpeed, float bombSpeed, float explodeTime)
    {
        yield return new WaitForSeconds(waitTime);
        ani.SetBool("AttachGarbageBomb", true);
        yield return new WaitForSeconds(0.5f);
        BossDir();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(XDist(), 0).normalized * dashSpeed;

        while(XDist() > 2 || XDist() < -2)
        {
            yield return null;
        }
        rb.velocity = Vector2.zero;
        ani.SetTrigger("AttachGarbageBomb_1");

        GameObject prefab = Instantiate(garbagePrefabV2, transform.position, Quaternion.identity) as GameObject;
        prefab.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        Vector2 direction = new Vector2(XDist(), YDist());
        prefab.GetComponent<Rigidbody2D>().velocity = direction.normalized * bombSpeed;

        LayerMask PLayerMask = 1 << LayerMask.NameToLayer("Player");
        var hit = Physics2D.Raycast(transform.position, direction, float.MaxValue, WFPLayerMask);
        var hit2 = Physics2D.Raycast(transform.position, direction, float.MaxValue, PLayerMask.value);
        Collider2D col = prefab.GetComponent<Collider2D>();

        while (!col.IsTouching(hit.collider))//뭐든 닿음
        {
            yield return null;
        }
        if (col.IsTouching(hit2.collider))//플레이어에 닿음
        {
            if(FindObjectOfType<Player>().GetComponent<Player>().isControllable)//안 구르는 중
            {
                Destroy(prefab);
                GameObject prefab2 = Instantiate(garbageAttachedPrefab,GetPlayerPos(),Quaternion.identity) as GameObject;
                prefab2.transform.parent = PlayerParent;
                yield return new WaitForSeconds(explodeTime);
                if (FindObjectOfType<Player>().GetComponent<Player>().isControllable)
                {
                    //플레이어 스턴 추가 필요
                }
                GameObject explode = Instantiate(Explosion, prefab2.transform.position, Quaternion.identity) as GameObject;
                Destroy(explode, 0.5f);
                Destroy(prefab2);
            }
            else//구르는 중
            {
                GameObject prefab1 = Instantiate(garbagePrefabV3, prefab.transform.position, Quaternion.identity) as GameObject;
                Destroy(prefab);
                prefab1.GetComponent<Rigidbody2D>().velocity = direction.normalized * bombSpeed;
                Collider2D col1 = prefab1.GetComponent<Collider2D>();
                var hit1 = Physics2D.Raycast(transform.position, direction, float.MaxValue, wallfloorLayerMask);
                while (!col1.IsTouching(hit1.collider))
                {
                    yield return null;
                }
                Destroy(prefab1);
            }
        }
        else//벽, 땅에 닿음
        {
            prefab.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GameObject explode = Instantiate(Explosion, prefab.transform.position, Quaternion.identity) as GameObject;
            Destroy(explode, 0.5f);
            Destroy(prefab);
        }
        ani.SetBool("AttachGarbageBomb", false);
    }

    private IEnumerator BloodVomit(float bloodSpeed)
    {
        BossDir();
        ani.SetBool("BloodVomit", true);
        yield return new WaitForSeconds(0.5f);
        float dir = 1;
        float dirD;
        if(XDist() >= 0)
        {
            dirD = -30f;
        }
        else
        {
            dirD = 210f;
            dir = -1;
        }
        GetDamaged(MaxHealth * 0.03);
        GameObject[] prefab = new GameObject[45];
        for (int i = 0; i < 45; i++)
        {
            prefab[i] = Instantiate(Blood, transform.position, Quaternion.identity) as GameObject;
            float radian = (dirD + 2 * i * dir) * Mathf.Deg2Rad;
            prefab[i].GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * bloodSpeed;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 45; i++)
        {
            Destroy(prefab[i]);
        }

        ani.SetBool("BloodVomit", false);
    }

    private IEnumerator SwordDance(float waitTime, float time)
    {
        bool bDir = BossDir();

        ani.SetBool("SwordDance", true);
        yield return new WaitForSeconds(waitTime);
        ani.SetTrigger("SwordDance_1");
        for(int i = 0;i<16;i++)
        {
            GameObject prefab = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
            if(!bDir)
            {
                prefab.transform.localScale = new Vector3(-1, 1, 1);
            }
            yield return new WaitForSeconds(0.1f);
            Destroy(prefab);
        }
        ani.SetBool("SwordDance", false);
    }

    private IEnumerator Uppercut(float dashSpeed)
    {
        bool bDir = BossDir();
        ani.SetBool("UpperCut", true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(XDist(), 0).normalized * dashSpeed;

        while (XDist() > NearDist || XDist() < -1 * NearDist)
        {
            yield return null;
        }
        rb.velocity = Vector2.zero;

        ani.SetTrigger("UpperCut_1");

        GameObject prefab = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
        if (!bDir)
        {
            prefab.transform.localScale = new Vector3(-1, 1, 1);
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(prefab);

        ani.SetBool("UpperCut", false);
    }

    private IEnumerator WallThump(float dashSpeed)
    {
        bool bDir = BossDir();
        ani.SetBool("WallThump", true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(XDist(), 0);
        rb.velocity = direction.normalized * dashSpeed;

        GameObject prefab = Instantiate(BurangBossCol, transform.position, Quaternion.identity) as GameObject;
        prefab.transform.parent = Parent;

        while (XDist() > NearDist || XDist() < -1 * NearDist)
        {
            yield return null;
        }
        ani.SetTrigger("WallThump_1");

        LayerMask WPLayerMask = 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Player");
        LayerMask PLayerMask = 1 << LayerMask.NameToLayer("Player");
        LayerMask WLayerMask = 1 << LayerMask.NameToLayer("Wall");
        var hit = Physics2D.Raycast(transform.position, direction, float.MaxValue, WPLayerMask.value);
        var hit1 = Physics2D.Raycast(transform.position, direction, float.MaxValue, PLayerMask.value);
        Collider2D col = prefab.GetComponent<Collider2D>();

        while (!col.IsTouching(hit.collider))
        {
            yield return null;
        }
        rb.velocity = Vector2.zero;
        if (col.IsTouching(hit1.collider))//플레이어에 닿았으면
        {
            BurangBossStunDebuff stunDebuff = new BurangBossStunDebuff();
            stunDebuff.Init(3);
            Game.inst.player.AddBuffable(stunDebuff);//플레이어 스턴

            FindObjectOfType<Player>().GetComponent<Rigidbody2D>().velocity = direction.normalized * dashSpeed;
            var hit2 = Physics2D.Raycast(FindObjectOfType<Player>().transform.position, direction, float.MaxValue, WLayerMask.value);
            Collider2D col2 = FindObjectOfType<Player>().GetComponent<Collider2D>();

            while (!col2.IsTouching(hit2.collider))
            {
                yield return null;
            }
            FindObjectOfType<Player>().GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            ani.SetTrigger("WallThump_2");

            rb.velocity = direction.normalized * dashSpeed;
            while (!col.IsTouching(hit.collider))
            {
                yield return null;
            }
            rb.velocity = Vector2.zero;

            Destroy(prefab);

            ani.SetTrigger("WallThump_3");

            yield return new WaitForSeconds(0.1f);

            BossDamage += 0.5f;

            GameObject child = Instantiate(NearAttack, transform.position, Quaternion.identity) as GameObject;
            if (!bDir)
            {
                child.transform.localScale = new Vector3(-1, 1, 1);
            }

            yield return new WaitForSeconds(0.2f);

            if(child.GetComponent<Knife>().IsKnifeHit)
            {
                BurangBossBleedDebuff bleedDebuff = new BurangBossBleedDebuff();
                bleedDebuff.Init(4);
                Game.inst.player.AddBuffable(bleedDebuff);
            }

            Destroy(child);

            BossDamage -= 0.5f;
        }
        ani.SetBool("WallThump", false);
    }

    private IEnumerator IdleRoutine(float time, float speed)
    {
        ani.SetBool("Phase1Walk",true);
        for(float interval = 0.1f;time>0;time-=interval)
        {
            Vector3 direction = new Vector3(XDist(),0,0).normalized;

            BossDir();

            yield return MoveRoutine (transform.position + new Vector3(direction.x * interval * speed,0,0), interval);
        }
        ani.SetBool("Phase1Walk", false);
    }

    protected override void OnStunned()
    {

    }
}

public class BurangBossStunDebuff : Buffable
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

public class BurangBossBleedDebuff : Buffable
{
    private float timer;
    private float nextTime;

    public override void StartDebuff(Player player)
    {
        nextTime = 1;
        timer = 0;
    }

    public override void Apply(Player player)
    {
        timer += Time.deltaTime;
        if(timer >= nextTime)
        {
            nextTime++;
            Game.inst.player.GetDamaged(0.25f);
        }
    }

    public override void EndDebuff(Player player) { }
}
