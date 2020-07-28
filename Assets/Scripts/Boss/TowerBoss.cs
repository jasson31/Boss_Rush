using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TowerBoss : Boss
{
    [SerializeField]
    private GameObject normalBullet;
    [SerializeField]
    private GameObject counterBullet;
    [SerializeField]
    private GameObject pillar;
    [SerializeField]
    private Vector3 shootPos;
    [SerializeField]
    private Bounds map;

    private LineRenderer lr;
    const float laserReadyWidth = 0.1f;
    const float laserShootWidth = 2.0f;

    const float bulletSpeed = 20;
    const int bulletChangeTime = 5;
    private float returnBulletShootTime = 0;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(shootPos, 1);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(map.center, map.size);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        switch (Phase)
        {
            case 0:
                nextRoutines.Enqueue(NewActionRoutine(LaserRoutine(2.0f)));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                nextRoutines.Enqueue(NewActionRoutine(LaserRoutine(2.0f)));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                nextRoutines.Enqueue(NewActionRoutine(LaserRoutine(2.0f, true)));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                break;
            case 1:
                nextRoutines.Enqueue(NewActionRoutine(ShotRoutine()));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                break;
        }
        return nextRoutines;
    }

    protected override void Init()
    {
        base.Init();
        returnBulletShootTime = Time.time;
        lr = GetComponent<LineRenderer>();
    }

    private IEnumerator ShotRoutine()
    {
        bool isNormal = (Time.time - returnBulletShootTime < bulletChangeTime);

        if (Time.time - returnBulletShootTime > bulletChangeTime)
        {
            returnBulletShootTime = Time.time;
        }

        GameObject bullet = Instantiate(isNormal ? normalBullet : counterBullet, shootPos, Quaternion.identity);
        if (isNormal) bullet.AddComponent<NormalBullet>();
        else bullet.AddComponent<ReturnBullet>();
        bullet.GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - shootPos).normalized * bulletSpeed;
        Destroy(bullet, 3.0f);
        yield return null;
    }
    private IEnumerator LaserRoutine(float waitTime, bool makePillar = false)
    {
        if(makePillar)
        {
            StartCoroutine(PillarRoutine(2.0f));
        }

        lr.enabled = true;

        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, GetPlayerPos());
        yield return new WaitForSeconds(waitTime);

        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;
        yield return new WaitForSeconds(0.5f);
        for(float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;
    }

    private IEnumerator PillarRoutine(float waitTime)
    {
        float x = Random.Range(map.min.x, map.max.x);
        float y = map.min.y;

        GameObject newPillar = Instantiate(pillar, new Vector2(x, y), Quaternion.identity);
        newPillar.AddComponent<Pillar>();

        Vector3 startPos = new Vector3(x, y - newPillar.GetComponent<Collider2D>().bounds.size.y);
        Vector3 destPos = new Vector3(x, y);

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            newPillar.transform.position = Vector3.Lerp(startPos, destPos, t / 0.1f);
            yield return null;
        }

        yield return new WaitForSeconds(waitTime);
        if(newPillar != null)
        {
            newPillar.GetComponent<Pillar>().PillarFall();
        }
    }

    protected override IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        returnBulletShootTime = Time.time;
    }

    protected override void OnStunned()
    {
        lr.enabled = false;
    }
}

public class NormalBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.GetDamaged(0);
        }
    }
}
public class ReturnBullet : MonoBehaviour, IDamagable
{
    public int Health { get; private set; }

    public void GetDamaged(int damage)
    {
        if (Health == 1)
        {
            GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            collision.GetComponent<Player>().GetDamaged(0);
        }
        else if (collision.GetComponent<Boss>() != null)
        {
            collision.GetComponent<Boss>().Stun(5);
        }
    }

    private void Start()
    {
        Health = 1;
    }
}

public class Pillar : MonoBehaviour, IDamagable
{
    public Coroutine fallRoutine;

    private bool isBossAttack = false;
    private bool isPlayerAttack = false;
    private bool isPillarFall = true;
    const float pillarSpeed = 5;

    public int Health { get; private set; }

    public void SetPlayerAttack(bool active)
    {
        isPlayerAttack = active;
    }

    public void GetDamaged(int damage)
    {
        if(Health > 0)
        {
            Health--;
            if (Health == 0)
            {
                // FIXME: Get rid of FindObjectOfType
                Vector3 playerPosition = FindObjectOfType<Player>().transform.position;

                GetComponent<Rigidbody2D>().velocity = new Vector3(transform.position.x - playerPosition.x, 0, 0).normalized * pillarSpeed;
                isBossAttack = true;
                SetPlayerAttack(false);
                isPillarFall = false;
                if (fallRoutine != null) StopCoroutine(fallRoutine);
            }
        }
    }

    public void PillarFall()
    {
        if(isPillarFall)
        {
            fallRoutine = StartCoroutine(PillarFallRoutine());
        }
    }

    public IEnumerator PillarFallRoutine()
    {
        // FIXME: Get rid of FindObjectOfType
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;

        bool isPlayerOnLeft = playerPosition.x - transform.position.x < 0;
        SetPlayerAttack(true);

        for (float t = 0; t < 1.0f; t += Time.deltaTime)
        {
            transform.Rotate(new Vector3(0, 0, 1), 90 * Time.deltaTime * (isPlayerOnLeft ? 1 : -1), Space.Self);
            yield return null;
        }

        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerAttack && collision.GetComponent<Player>() != null)
        {
            collision.GetComponent<Player>().GetDamaged(0);
        }
        else if (isBossAttack && collision.GetComponent<Boss>() != null)
        {
            collision.GetComponent<Boss>().Stun(5);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Health = 1;
    }
}


