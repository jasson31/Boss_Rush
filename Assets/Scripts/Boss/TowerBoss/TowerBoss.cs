using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBoss : Boss
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject lightningOrb;
    [SerializeField]
    private Vector3 shootPos;
    [SerializeField]
    private GameObject laserCollider;
    [SerializeField]
    private GameObject laserCollider2;
    [SerializeField]
    private Bounds map;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private LayerMask penMask;
    [SerializeField]
    private LineRenderer lr, lr2;
    const float laserReadyWidth = 0.1f;
    const float laserShootWidth = 2.0f;
    
  
    RaycastHit2D shootHit;

    const float bulletSpeed = 6.5f;

    protected override void Start()
    {
        base.Start();
        bullet.AddComponent<Bullet>();
        laserCollider.AddComponent<Laser>();
        laserCollider2.AddComponent<Laser>();
        Phase = 1;
        MaxHealth = Health = 200;
    }

    public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);
        if (MaxHealth * 0.33f >= Health && Phase == 1)
        {
            Phase = 2;
        }
        if (Health <= 0 && Phase == 2)
        {
            Phase = 3;
        }
        if (Phase == 4)
            gameObject.SetActive(false);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        float rand = Random.value;
        switch (Phase)
        {
            case 1:

                if (rand < 0.33f)
                {

                    nextRoutines.Enqueue(NewActionRoutine(CircularShotRoutine(5, 12, 1f)));

                }
                else if (rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(StraightShotRoutine(3, 6, 0.7f)));
                    //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine(FanShotRoutine(5, 5, 0.7f)));
                    //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }

                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3f)));
                break;

            case 2:

                Vector3 distance = shootPos - GetPlayerPos();

                if (rand < 0.25f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(LaserRoutine(1.0f)));
                    //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }
                else if (rand < 0.5f)
                {
                    if (distance.magnitude > 9)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(LaserRoutine2(1.0f)));
                        //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(LaserRoutine2b(1.0f)));
                        //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                }
                else if (rand < 0.75f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(OrbRoutine(7)));
                    //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine(ThunderRoutine(0.7f)));
                    //nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }

                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3f)));
                break;

            case 3:
                nextRoutines.Enqueue(NewActionRoutine(FinalRoutine(15f)));
                Phase = -1;

                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(0.8f)));
                break;

        }

        return nextRoutines;
    }


    private IEnumerator CircularShotRoutine(int numWave, int bulletCount, float interval)
    {
        animator.SetTrigger("Attack");
        for(int j=0; j <numWave; j++)
        {
            Vector3 originalBulletPos = (GetPlayerPos() - shootPos).normalized;
            //float range = (shootPos - originalBulletPos).magnitude;
            Vector3 nextBulletPos = originalBulletPos;
            float currentAngle = 0f;


            for (int i = 0; i < bulletCount; i++)
            {

                nextBulletPos += shootPos;
                GameObject cur = Instantiate(bullet, nextBulletPos, Quaternion.Euler(0, 0, currentAngle));
                cur.GetComponent<Rigidbody2D>().velocity = (nextBulletPos - shootPos).normalized * bulletSpeed;
                currentAngle += 360 / bulletCount * Mathf.Deg2Rad;
                nextBulletPos = new Vector3(originalBulletPos.x * Mathf.Cos(currentAngle) - originalBulletPos.y * Mathf.Sin(currentAngle),
                                                originalBulletPos.x * Mathf.Sin(currentAngle) + originalBulletPos.y * Mathf.Cos(currentAngle), 0).normalized;
                Destroy(cur, 3f);
            }

            yield return new WaitForSeconds(interval);

        }

    }
    private IEnumerator StraightShotRoutine(int numWave, int bulletCount, float interval)
    {
        animator.SetTrigger("Attack");
        for (int n=0; n<numWave; n++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                GameObject cur = Instantiate(bullet, shootPos, Quaternion.identity);
                cur.GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - shootPos).normalized * bulletSpeed * 2;
                Destroy(cur, 1.5f);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(interval);

        }


    }

    private IEnumerator FanShotRoutine(int numWave, int bulletCount, float interval)
    {
        animator.SetTrigger("Attack");
        for (int i=0; i<numWave; i++)
        {
            Vector3 pos = GetPlayerPos();

            for (int j = 0; j < bulletCount; j++)
            {

                GameObject cur = Instantiate(bullet, shootPos, Quaternion.identity);
                cur.GetComponent<Rigidbody2D>().velocity
                    = (pos + new Vector3(0, j - 2, 0) / 0.5f * Mathf.Pow(-1, i) - shootPos).normalized * bulletSpeed * 1.5f;
                Destroy(cur, 2.5f);
                yield return new WaitForSeconds(0.15f);
            }
        yield return new WaitForSeconds(interval);
        }

    }

    private IEnumerator LaserRoutine(float waitTime)
    {
        animator.SetTrigger("Attack");
        for (int i=0; i<3; i++)
        {
            lr.enabled = true;

            Vector3 lineEndPos = GetPlayerPos();

            lr.SetPosition(0, shootPos);
            lr.SetPosition(1, lineEndPos);

            for (float t = 0; t < waitTime; t += Time.deltaTime)
            {

                lineEndPos = GetPlayerPos();
                lr.SetPosition(0, shootPos);
                lr.SetPosition(1, lineEndPos);
                yield return null;
            }


            shootHit = Physics2D.Raycast(shootPos, lineEndPos - shootPos, 100, penMask);

            lr.SetPosition(1, shootHit.point);

            laserCollider.transform.position = (new Vector3(shootHit.point.x, shootHit.point.y, 0) + shootPos) / 2;
            Vector3 diff = new Vector3(shootHit.point.x, shootHit.point.y, 0) - shootPos;
            laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
            laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);
            laserCollider.SetActive(true);

            lr.startWidth = laserShootWidth;
            lr.endWidth = laserShootWidth;

            yield return new WaitForSeconds(0.5f);

            laserCollider.SetActive(false);

            for (float t = 0; t < 0.2f; t += Time.deltaTime)
            {
                float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
                lr.startWidth = size;
                lr.endWidth = size;
                yield return null;
            }

            lr.startWidth = laserReadyWidth;
            lr.endWidth = laserReadyWidth;
            lr.enabled = false;

            yield return null;
        }
        
    }
    
    private IEnumerator LaserRoutine2(float waitTime)
    {
        animator.SetTrigger("Attack");
        lr.enabled = true;
        lr2.enabled = true;

        Vector3 end1 = new Vector3(map.min.x - 1, map.min.y, 0);
        Vector3 end2 = new Vector3(map.max.x + 1, map.min.y, 0);


        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, end1);

        lr2.SetPosition(0, shootPos);
        lr2.SetPosition(1, end2);

        yield return new WaitForSeconds(waitTime);


        Vector3 fin = new Vector3(0, map.min.y, 0);

        laserCollider.transform.position = (end1 + shootPos) / 2;
        Vector3 diff = end1 - shootPos;
        laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);
        laserCollider.SetActive(true);

        laserCollider2.transform.position = (end2 + shootPos) / 2;
        Vector3 diff2 = end2 - shootPos;
        laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg);
        laserCollider.transform.localScale = new Vector2(diff2.magnitude, laserShootWidth);
        laserCollider2.SetActive(true);

        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;

        
        lr2.startWidth = laserShootWidth;
        lr2.endWidth = laserShootWidth;


        while (end1.x < 0)
        {
            end1 = end1 + new Vector3(0.1f, 0, 0);
            end2 = end2 - new Vector3(0.1f, 0, 0);
            lr.SetPosition(1, end1);
            lr2.SetPosition(1, end2);

            laserCollider.transform.position = (end1 + shootPos) / 2;
            diff = end1 - shootPos;
            laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
            laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);

            laserCollider2.transform.position = (end2 + shootPos) / 2;
            diff2 = end2 - shootPos;
            laserCollider2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg);
            laserCollider2.transform.localScale = new Vector2(diff2.magnitude, laserShootWidth);

            yield return new WaitForSeconds(0.015f);
        }

        laserCollider.SetActive(false);
        laserCollider2.SetActive(false);


        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;
            
            lr2.startWidth = size;
            lr2.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;
        
        lr2.startWidth = laserReadyWidth;
        lr2.endWidth = laserReadyWidth;
        lr2.enabled = false;
    }

    private IEnumerator LaserRoutine2b(float waitTime)
    {
        animator.SetTrigger("Attack");
        lr.enabled = true;
        lr2.enabled = true;

        Vector3 start = new Vector3(0, map.min.y, 0);

        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, start);

        lr2.SetPosition(0, shootPos);
        lr2.SetPosition(1, start);

        yield return new WaitForSeconds(waitTime);

        Vector3 end1 = new Vector3(0, map.min.y, 0);
        Vector3 end2 = new Vector3(0, map.min.y, 0);

        laserCollider.transform.position = (end1 + shootPos) / 2;
        Vector3 diff = end1 - shootPos;
        laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);
        laserCollider.SetActive(true);

        laserCollider2.transform.position = (end2 + shootPos) / 2;
        Vector3 diff2 = end2 - shootPos;
        laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg);
        laserCollider.transform.localScale = new Vector2(diff2.magnitude, laserShootWidth);
        laserCollider2.SetActive(true);

        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;

        lr2.startWidth = laserShootWidth;
        lr2.endWidth = laserShootWidth;

        while (end1.x > map.min.x - 1)
        {
            end1 = end1 - new Vector3(0.1f, 0, 0);
            end2 = end2 + new Vector3(0.1f, 0, 0);
            lr.SetPosition(1, end1);
            lr2.SetPosition(1, end2);

            laserCollider.transform.position = (end1 + shootPos) / 2;
            diff = end1 - shootPos;
            laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
            laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);

            laserCollider2.transform.position = (end2 + shootPos) / 2;
            diff2 = end2 - shootPos;
            laserCollider2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg);
            laserCollider2.transform.localScale = new Vector2(diff2.magnitude, laserShootWidth);

            yield return new WaitForSeconds(0.015f);
        }

        laserCollider.SetActive(false);
        laserCollider2.SetActive(false);

        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;

            lr2.startWidth = size;
            lr2.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;

        lr2.startWidth = laserReadyWidth;
        lr2.endWidth = laserReadyWidth;
        lr2.enabled = false;
    }

    private IEnumerator OrbRoutine(int orbCount)
    {
        animator.SetTrigger("Attack");
        GameObject[] orbs = new GameObject[orbCount];
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < orbCount; i++)
        {
            Vector3 temp = new Vector3(UnityEngine.Random.Range(map.min.x, map.max.x), UnityEngine.Random.Range(map.min.y, map.max.y), 0);
            orbs[i] = Instantiate(lightningOrb, temp, Quaternion.identity);
            //yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(1.0f);

        for(int i=0; i<orbCount; i++)
        {
            //Vector3 temp = GetPlayerPos();
            //Vector3 slope = orbs[i].transform.position - temp;

            //while((orbs[i].transform.position - temp).magnitude > 0.5f)
            //{
            //    orbs[i].transform.position -= slope / 100;
            //    yield return new WaitForSeconds(0.005f);
            //}

            orbs[i].GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - orbs[i].transform.position).normalized * 13;
            Destroy(orbs[i], 3.5f);

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(1f); 


    }

    private IEnumerator ThunderRoutine(float waitTime)
    {
        animator.SetTrigger("Attack");
        for (int i=0; i<7; i++)
        {

            Vector3 playerXPos = new Vector3(GetPlayerPos().x, map.max.y - 0.01f, 0);

            lr.enabled = true;
            lr.SetPosition(0, playerXPos);

            shootHit = Physics2D.Raycast(playerXPos, Vector2.down, 100, penMask);

            if (shootHit.collider.gameObject.layer != 11)
            {
                lr.SetPosition(1, shootHit.point);
            }
            else
            {
                lr.SetPosition(1, GetPlayerPos());
            }

            yield return new WaitForSeconds(0.3f);

            laserCollider.transform.position = (playerXPos + new Vector3(shootHit.point.x, shootHit.point.y, 0)) / 2;
            Vector3 diff = playerXPos - new Vector3(shootHit.point.x, shootHit.point.y, 0);
            laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
            laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);
            laserCollider.SetActive(true);

            lr.startWidth = laserShootWidth;
            lr.endWidth = laserShootWidth;

            yield return new WaitForSeconds(0.3f);

            laserCollider.SetActive(false);


            for (float t = 0; t < 0.2f; t += Time.deltaTime)
            {
                float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
                lr.startWidth = size;
                lr.endWidth = size;

                lr2.startWidth = size;
                lr2.endWidth = size;
                yield return null;
            }

            lr.startWidth = laserReadyWidth;
            lr.endWidth = laserReadyWidth;
            lr.enabled = false;

            yield return new WaitForSeconds(waitTime);
        }

    }

    private IEnumerator FinalRoutine(float count)
    {
        animator.SetTrigger("Attack");
        for (float i = 0; i < count; i ++)
        {
            lr.enabled = true;

            Vector3 lineEndPos = new Vector3(UnityEngine.Random.Range(map.min.x, map.max.x), UnityEngine.Random.Range(map.min.y, shootPos.y), 0);

            lr.SetPosition(0, shootPos);

            shootHit = Physics2D.Raycast(shootPos, lineEndPos - shootPos, 100, penMask);

            if (shootHit)
            {
                lr.SetPosition(1, shootHit.point);
            }

            yield return new WaitForSeconds(0.2f);

            lineEndPos = new Vector3(shootHit.point.x, shootHit.point.y, 0);

            laserCollider.transform.position = (lineEndPos + shootPos) / 2;
            Vector3 diff = lineEndPos - shootPos;
            laserCollider.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
            laserCollider.transform.localScale = new Vector2(diff.magnitude, laserShootWidth);

            lr.startWidth = laserShootWidth;
            lr.endWidth = laserShootWidth;

            laserCollider.SetActive(true);

            yield return new WaitForSeconds(0.2f);

            laserCollider.SetActive(false);

            //laserCollider.SetActive(false);

            for (float t = 0; t < 0.2f; t += Time.deltaTime)
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
        
    }


    private IEnumerator IdleRoutine(float time)
    {
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(time);
    }

    protected override void OnStunned()
    {

    }

}

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            Game.inst.player.GetDamaged(0.25f);

        }
    }
}

public class Laser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Game.inst.player.GetDamaged(0.25f);
        }
    }
}



