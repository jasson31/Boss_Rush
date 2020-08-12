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
    private List<Vector3> targetPositions = new List<Vector3>();//지정된 위치로 이동시

    [SerializeField]
    private AnimationCurve jumpCurve;

    [SerializeField]
    private float MoveSpeed = 5;

    [SerializeField]
    private Bounds map;

    [SerializeField]
    private GameObject knifePrefab;

    [SerializeField]
    private GameObject garbagePrefab;

    private bool isCollidePlayer = false;

    Transform Parent;

    private int FirstPhase2 = 1;//페이즈 시작 특별 패턴
    private int FirstPhase3 = 1;

    protected override void Start()
    {
        base.Start();
        Phase = 0;
        MaxHealth = Health = 200;
        Parent = GameObject.Find("BurangBoss").GetComponent<Transform>();

    }

    public override void GetDamaged(int damage)
    {
        base.GetDamaged(damage);
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
        float rand = Random.value;

        Vector3 BossPos = transform.position;
        Vector3 playerPos = GetPlayerPos();
        Vector3 dist = BossPos - playerPos;

        switch (Phase)
        {
            case 0:
                if(rand < 0.33f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DashStab(1f, 30f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }
                else if(rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(DiagonalStab(1f, 12f, 30f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }
                else
                {
                    if(dist.magnitude < 5)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(ChargeTripleStab(1f, 0.1f)));
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(GarbageBomb(1f, 3, 12, 0.1f)));
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                }
                break;
            case 1:
                {
                    if(FirstPhase2 == 1)//특별 패턴
                    {
                        FirstPhase2 = 0;
                        nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab()));
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                    else
                    {
                        if(rand < 0.25f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab()));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                        }
                        else if(rand < 0.5f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ThrowTeleportStab()));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                        }
                        else if(rand < 0.75f)
                        {
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab()));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                            nextRoutines.Enqueue(NewActionRoutine(ShadowTeleportStab()));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                        }
                        else
                        {
                            nextRoutines.Enqueue(NewActionRoutine(GarbageBombAura()));
                            nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                        }
                    }
                }
                break;
            case 2:
                {
                    if(FirstPhase3 == 1)//특별 패턴
                    {
                        FirstPhase3 = 0;
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(2.0f)));
                    }
                    else
                    {
                        if(playerPos.x <= -13 && playerPos.x >= 13)
                        {
                            if(rand < 0.25f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb()));
                                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                            }
                            else if(rand < 0.5f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit()));
                                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                            }
                            else if(rand < 0.75f)
                            {
                                if(dist.magnitude < 8)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance()));
                                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                                }
                                else
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(Uppercut()));
                                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                                }
                            }
                            else
                            {
                                nextRoutines.Enqueue(NewActionRoutine(WallThump()));
                                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                            }
                        }
                        else
                        {
                            if(rand < 0.33f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(AttachGarbageBomb()));
                                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                            }
                            else if(rand < 0.66f)
                            {
                                nextRoutines.Enqueue(NewActionRoutine(BloodVomit()));
                                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                            }
                            else
                            {
                                if(dist.magnitude < 8)
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(SwordDance()));
                                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                                }
                                else
                                {
                                    nextRoutines.Enqueue(NewActionRoutine(Uppercut()));
                                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                                }
                            }
                        }
                    }
                }
                break;
        }
        return nextRoutines;
    }

    private IEnumerator DashStab(float waitTime, float dashSpeed)
    {
        UnityEngine.Debug.Log("DashStab!");

        GameObject child = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child.transform.parent = Parent;
        yield return new WaitForSeconds(waitTime);//경고 동작

        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
        Vector2 direction = new Vector2(playerPosition.x - transform.position.x,0);

        direction.Normalize();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * dashSpeed;
        yield return new WaitForSeconds(0.3f);

        rb.velocity = Vector2.zero;
        Destroy(child);
    }

    private IEnumerator DiagonalStab(float waitTime,float jumpSpeed, float dashSpeed)
    {
        UnityEngine.Debug.Log("DiagonalStab!");

        yield return new WaitForSeconds(waitTime);//점프 준비 동작
        Vector3 currentPos = transform.position;

        float jumpHeight = 6f;
        float dashDistance = 12f;
        yield return MoveRoutine(currentPos + new Vector3(0, jumpHeight, 0), jumpHeight / jumpSpeed, jumpCurve);

        GameObject child = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child.transform.parent = Parent;

        Vector3 playerPosition = GetPlayerPos();
        if((playerPosition.x - transform.position.x)>=0)
        {//충돌 체크 가능하면 velocity로? 교체
            yield return MoveRoutine(currentPos + new Vector3(dashDistance, 0, 0), new Vector3(dashDistance, jumpHeight, 0).magnitude / dashSpeed);
        }
        else
        {
            yield return MoveRoutine(currentPos + new Vector3(-1 * dashDistance, 0, 0), new Vector3(dashDistance, jumpHeight, 0).magnitude / dashSpeed);
        }

        Destroy(child);
    }

    private IEnumerator ChargeTripleStab(float waitTime, float interval)
    {
        UnityEngine.Debug.Log("ChargeTripleStab");

        yield return new WaitForSeconds(waitTime);

        GameObject child1 = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child1.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child1);
        GameObject child2 = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child2.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child2);
        GameObject child3 = Instantiate(knifePrefab, transform.position, Quaternion.identity) as GameObject;
        child3.transform.parent = Parent;
        yield return new WaitForSeconds(interval);
        Destroy(child3);

    }

    private IEnumerator GarbageBomb(float waitTime, int count, float garbageSpeed, float interval)
    {
        UnityEngine.Debug.Log("GarbageBomb");

        yield return new WaitForSeconds(waitTime);
        for(int i=0;i<count;i++)
        {
            Instantiate(garbagePrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - transform.position).normalized * garbageSpeed;
            yield return new WaitForSeconds(interval);
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
            float y = transform.position.y;
            float z = 0;
            yield return MoveRoutine(new Vector3(x, y, z), Vector2.Distance(transform.position, new Vector2(x, y)) / MoveSpeed);
        }
    }

    private IEnumerator ShadowTeleportStab()
    {
        yield return null;
    }

    private IEnumerator ThrowTeleportStab()
    {
        yield return null;
    }

    private IEnumerator GarbageBombAura()
    {
        yield return null;
    }

    private IEnumerator AttachGarbageBomb()
    {
        yield return null;
    }

    private IEnumerator BloodVomit()
    {
        yield return null;
    }

    private IEnumerator SwordDance()
    {
        yield return null;
    }

    private IEnumerator Uppercut()
    {
        yield return null;
    }

    private IEnumerator WallThump()
    {
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (isCollidePlayer && player != null)
        {
            player.GetDamaged(1);
        }
    }

    protected override void OnStunned()
    {

    }
}
