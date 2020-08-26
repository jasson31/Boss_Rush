﻿using System;
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
    private GameObject knifePrefab;

    [SerializeField]
    private GameObject ThrowingknifePrefab;

    [SerializeField]
    private GameObject garbagePrefab;

    [SerializeField]
    private GameObject garbagePrefabV2;

    [SerializeField]
    private GameObject garbageAttachedPrefab;

    [SerializeField]
    private GameObject shadowPrefab;

    [SerializeField]
    private GameObject BurangBossCol;

    [SerializeField]
    private LayerMask wallfloorLayerMask;

    [SerializeField]
    private LayerMask WFPLayerMask;

    bool Invincible = false;

    bool IsHit = false;

    Animator ani;
    SpriteRenderer rend;

    Transform Parent;
    Transform PlayerParent;

    private int FirstPhase2;//페이즈 시작 특별 패턴
    private int FirstPhase3;

    protected override void Start()
    {
        base.Start();
        Phase = 0;
        MaxHealth = Health = 200;
        Parent = GameObject.Find("BurangBoss").GetComponent<Transform>();
        PlayerParent = GameObject.Find("Player").GetComponent<Transform>();
        ani = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rend.flipX = false;
        FirstPhase2 = 1;
        FirstPhase3 = 1;
    }

    private float XDist()
    {
        return GetPlayerPos().x - transform.position.x;
    }

    private float YDist()
    {
        return GetPlayerPos().y - transform.position.y;
    }

    private void BossDir()
    {
        if (XDist() >= 0) rend.flipX = false;
        else rend.flipX = true;
    }

    public override void GetDamaged(int damage)
    {
        if (!Invincible) { base.GetDamaged(damage); }
        else { IsHit = true; }
        if (MaxHealth * 0.1f >= Health && Phase == 0)
        {
            Phase = 1;
            Health = 200;
        }
        if (MaxHealth * 0.1f >= Health && Phase == 1)
        {
            Phase = 2;
            Health = 200;
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
                if(rand < 0.33f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DashStab(1f, 30f, 0.5f)));
                }
                else if(rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DiagonalStab(1f, 15f, 0.8f, 30f)));
                }
                else
                {
                    if(dist.magnitude < 5)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(ChargeTripleStab(1f, 0.2f)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(GarbageBomb(1f, 3, 4f, 0.1f, 2f, 1f)));
                    }
                }
                float idleTime1 = UnityEngine.Random.Range(1.2f, 1.7f);
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(idleTime1,3f)));
                break;
            case 1:
                {
                    if (FirstPhase2 == 1)//특별 패턴
                    {
                        nextRoutines.Enqueue(NewActionRoutine(Phase1to2()));
                        nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(0,1f)));
                    }
                    else
                    {
                        if(rand < 0.25f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(1f,0.5f)));
                        }
                        else if(rand < 0.5f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ThrowTeleportDashStab(0.5f,20f,30f,0.5f)));
                        }
                        else if(rand < 0.75f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(0.5f,0.5f)));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab(0.5f,0.5f)));
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
                {
                    if (FirstPhase3 == 1)//특별 패턴
                    {
                        nextRoutines.Enqueue(NewActionRoutine(Phase2to3()));
                    }
                    else
                    {
                        if(playerPos.x <= -13 && playerPos.x >= 13)
                        {
                            if(rand < 0.25f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb(1f,30f,50f,2f)));
                            }
                            else if(rand < 0.5f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit(30f)));
                            }
                            else if(rand < 0.75f)
                            {
                                if(dist.magnitude < 8)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance(1f, 2.5f)));
                                }
                                else
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(Uppercut(30f)));
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
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb(1f,30f,30f,2f)));
                            }
                            else if(rand < 0.66f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit(30f)));
                            }
                            else
                            {
                                if(dist.magnitude < 8)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance(1f, 2.5f)));
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
        ani.SetBool("Phase1to2", true);
        BossDir();
        yield return new WaitForSeconds(2f);
        ani.SetBool("Phase1to2", false);
        FirstPhase2 = 0;
    }

    private IEnumerator Phase2to3()
    {
        ani.SetTrigger("Phase2to3");
        BossDir();
        yield return new WaitForSeconds(3f);
        ani.SetTrigger("Phase3Idle");
        FirstPhase3 = 0;
    }

    private IEnumerator DashStab(float waitTime, float dashSpeed, float dashTime)
    {
        UnityEngine.Debug.Log("DashStab!");

        ani.SetBool("DashStab", true);

        BossDir();

        Vector2 direction = new Vector2(XDist(), 0);

        //찌르기 모션 수정 필요
        GameObject child = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child.transform.parent = Parent;
        yield return new WaitForSeconds(waitTime);//경고 동작

        direction.Normalize();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector2.zero;
        Destroy(child);

        ani.SetBool("DashStab", false);
    }

    private IEnumerator DiagonalStab(float waitTime, float jumpSpeed, float jumpTime, float dashSpeed)
    {
        UnityEngine.Debug.Log("DiagonalStab!");

        yield return new WaitForSeconds(waitTime);//점프 준비 동작

        ani.SetBool("DiagonalStab", true);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, jumpSpeed);

        yield return new WaitForSeconds(jumpTime);

        //찌르기 모션 수정 필요
        GameObject child = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child.transform.parent = Parent;

        BossDir();

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
    }

    private IEnumerator ChargeTripleStab(float waitTime, float interval)
    {
        UnityEngine.Debug.Log("ChargeTripleStab");

        BossDir();

        ani.SetBool("ChargeTripleStab", true);

        yield return new WaitForSeconds(waitTime);

        //찌르기 모션 수정 필요
        GameObject child1 = Instantiate(knifePrefab, transform.position, Quaternion.Euler(0,0,30)) as GameObject;
        child1.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child1);
        GameObject child2 = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child2.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child2);
        GameObject child3 = Instantiate(knifePrefab, transform.position, Quaternion.Euler(0,0,-30)) as GameObject;
        child3.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child3);

        ani.SetBool("ChargeTripleStab", false);
    }

    private IEnumerator GarbageBomb(float waitTime, int count, float ArriveTime, float intervalTime, float intervalDistance, float ExplodeTime)
    {
        UnityEngine.Debug.Log("GarbageBomb");

        BossDir();

        ani.SetBool("GarbageBomb", true);

        yield return new WaitForSeconds(waitTime);

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
            Destroy(prefab[i]);
            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator ShadowTeleportStab(float waitTime, float teleportTime)
    {
        UnityEngine.Debug.Log("ShadowTeleportStab");

        GetComponent<Rigidbody2D>().gravityScale = 0.0f;

        GameObject shadowEnter = Instantiate(shadowPrefab, transform.position - new Vector3(0, 1.3f, 0), Quaternion.identity);
        yield return new WaitForSeconds(waitTime);
        Vector3 teleportPos = GetPlayerPos() - new Vector3(0, 0.8f, 0);
        GameObject shadowExit = Instantiate(shadowPrefab, teleportPos, Quaternion.identity);

        ani.SetBool("ShadowTeleportStab", true);

        yield return new WaitForSeconds(teleportTime);
        transform.position = teleportPos + new Vector3(0,1.2f,0);
        Destroy(shadowEnter);
        yield return new WaitForSeconds(teleportTime);
        Destroy(shadowExit);

        BossDir();
        //찌르기 모션 수정 필요
        GameObject child1 = Instantiate(knifePrefab, transform.position, Quaternion.Euler(0, 0, 15)) as GameObject;
        child1.transform.parent = Parent;
        yield return new WaitForSeconds(0.2f);
        Destroy(child1);
        GameObject child2 = Instantiate(knifePrefab, transform.position, Quaternion.Euler(0,0,-15)) as GameObject;
        child2.transform.parent = Parent;
        yield return new WaitForSeconds(0.2f);
        Destroy(child2);

        ani.SetBool("ShadowTeleportStab", false);

        GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ThrowTeleportDashStab(float waitTime, float knifeSpeed, float dashSpeed, float dashTime)
    {
        UnityEngine.Debug.Log("ThrowTeleportDashStab");

        yield return new WaitForSeconds(waitTime);

        BossDir();
        ani.SetBool("ThrowTeleportDashStab", true);
        yield return new WaitForSeconds(0.1f);
        GameObject prefab = Instantiate(ThrowingknifePrefab, transform.position, Quaternion.identity);
        Vector2 direction = new Vector2(XDist(), YDist()).normalized;
        prefab.GetComponent<Rigidbody2D>().velocity = direction * knifeSpeed;

        var hit = Physics2D.Raycast(prefab.transform.position, direction, float.MaxValue, wallfloorLayerMask);
        Collider2D col = prefab.GetComponent<Collider2D>();

        while (!col.IsTouching(hit.collider))
        {
            hit = Physics2D.Raycast(prefab.transform.position, prefab.GetComponent<Rigidbody2D>().velocity, float.MaxValue, wallfloorLayerMask);
            yield return null;
        }

        prefab.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        ani.SetBool("ThrowTeleportDashStabCol", true);
        yield return new WaitForSeconds(0.1f);
        transform.position = prefab.transform.position + new Vector3(0, 1.5f, 0);
        Destroy(prefab);

        BossDir();
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        yield return new WaitForSeconds(waitTime);

        Vector2 dashDir = new Vector2(XDist(), YDist()).normalized;
        GetComponent<Rigidbody2D>().velocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        ani.SetBool("ThrowTeleportDashStabCol", false);
        ani.SetBool("ThrowTeleportDashStab", false);

        GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator GarbageBombArmor(float invincibleTime)
    {
        UnityEngine.Debug.Log("GarbageBombArmor");

        BossDir();

        ani.SetBool("GarbageBombArmor", true);
        yield return new WaitForSeconds(0.5f);
        Invincible = true;
        for(float i = 0.1f;invincibleTime > i;i+=0.1f)
        {
            if(IsHit)
            {
                //FindObjectOfType<Player>().GetComponent<Player>().GetDamaged(1);
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        Invincible = false;
        ani.SetBool("GarbageBombArmor", false);
        if (IsHit)
        {
            GetDamaged(10);
            Stun(3f);
        }
        IsHit = false;
    }

    private IEnumerator AttachGarbageBomb(float waitTime, float dashSpeed, float bombSpeed, float explodeTime)
    {
        UnityEngine.Debug.Log("AttachGarbageBomb");

        yield return new WaitForSeconds(waitTime);
        ani.SetBool("AttachGarbageBomb", true);
        yield return new WaitForSeconds(0.5f);
        BossDir();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(XDist(), 0).normalized * dashSpeed;

        while(XDist() > 5 || XDist() < -5)
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
                if (!FindObjectOfType<Player>().GetComponent<Player>().isControllable)
                {
                    //FindObjectOfType<Player>().GetComponent<Player>().GetDamaged(1);
                }
                else
                {
                    //FindObjectOfType<Player>().GetComponent<Player>().GetDamaged(1);
                    //플레이어 스턴 추가 필요
                }
                Destroy(prefab2);
            }
            else//구르는 중
            {
                GameObject prefab1 = Instantiate(garbagePrefab, prefab.transform.position, Quaternion.identity) as GameObject;
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
            Destroy(prefab);
        }
        ani.SetBool("AttachGarbageBomb", false);
    }

    private IEnumerator BloodVomit(float bloodSpeed)
    {
        UnityEngine.Debug.Log("BloodVomit");

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
        GameObject[] prefab = new GameObject[90];
        for (int i = 0; i < 90; i++)
        {
            prefab[i] = Instantiate(garbagePrefab, transform.position, Quaternion.identity) as GameObject;
            float radian = (dirD + i * dir) * Mathf.Deg2Rad;
            prefab[i].GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * bloodSpeed;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 90; i++)
        {
            Destroy(prefab[i]);
        }
        ani.SetBool("BloodVomit", false);
    }

    private IEnumerator SwordDance(float waitTime, float time)
    {
        UnityEngine.Debug.Log("SwordDance");

        BossDir();

        ani.SetBool("SwordDance", true);
        yield return new WaitForSeconds(waitTime);
        ani.SetTrigger("SwordDance_1");
        yield return new WaitForSeconds(time);
        ani.SetBool("SwordDance", false);
    }

    private IEnumerator Uppercut(float dashSpeed)
    {
        UnityEngine.Debug.Log("UpperCut");

        BossDir();
        ani.SetBool("UpperCut", true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(XDist(), 0).normalized * dashSpeed;

        while (XDist() > 3 || XDist() < -3)
        {
            yield return null;
        }
        rb.velocity = Vector2.zero;

        ani.SetTrigger("UpperCut_1");
        yield return new WaitForSeconds(0.5f);
        ani.SetBool("UpperCut", false);
    }

    private IEnumerator WallThump(float dashSpeed)
    {
        UnityEngine.Debug.Log("WallThump");

        BossDir();
        ani.SetBool("WallThump", true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(XDist(), 0);
        rb.velocity = direction.normalized * dashSpeed;

        GameObject prefab = Instantiate(BurangBossCol, transform.position, Quaternion.identity) as GameObject;
        prefab.transform.parent = Parent;

        while (XDist() > 2 || XDist() < -2)
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
            FindObjectOfType<Player>().GetComponent<Player>().isControllable = false;
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
            while (!col.IsTouching(hit1.collider))
            {
                yield return null;
            }
            rb.velocity = Vector2.zero;

            ani.SetTrigger("WallThump_3");

            yield return new WaitForSeconds(0.3f);
            FindObjectOfType<Player>().GetComponent<Player>().isControllable = true;
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        
    }

    protected override void OnStunned()
    {

    }
}
