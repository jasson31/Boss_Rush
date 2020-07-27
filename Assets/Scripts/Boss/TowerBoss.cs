using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBoss : Boss
{
    [SerializeField]
    private GameObject normalBullet;
    [SerializeField]
    private GameObject counterBullet;
    [SerializeField]
    private Vector3 bulletShootPos;


    const float bulletSpeed = 20;
    const int bulletChangeTime = 5;
    private float returnBulletShootTime = 0;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(bulletShootPos, 1);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        switch (Phase)
        {
            case 0:
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
    }

    private IEnumerator ShotRoutine()
    {
        bool isNormal = (Time.time - returnBulletShootTime < bulletChangeTime);

        if(Time.time - returnBulletShootTime > bulletChangeTime)
        {
            returnBulletShootTime = Time.time;
        }

        // FIXME: Get rid of FindObjectOfType
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;

        GameObject bullet = Instantiate(isNormal ? normalBullet : counterBullet, bulletShootPos, Quaternion.identity);
        if(isNormal) bullet.AddComponent<NormalBullet>();
        else bullet.AddComponent<ReturnBullet>();
        bullet.GetComponent<Rigidbody2D>().velocity = (playerPosition - bulletShootPos).normalized * bulletSpeed;
        Destroy(bullet, 3.0f);
        yield return null;
    }

    protected override IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        returnBulletShootTime = Time.time;
    }

    protected override void OnStunned()
    {

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
        if(Health == 1)
        {
            Health--;
            GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            collision.GetComponent<Player>().GetDamaged(0);
        }
        else if(collision.GetComponent<Boss>() != null)
        {
            collision.GetComponent<Boss>().Stun(5);
        }
    }

    private void Start()
    {
        Health = 1;
    }
}

