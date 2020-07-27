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
                nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(true)));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                break;
        }



        return nextRoutines;
    }



    private IEnumerator ShotRoutine(bool isNormal)
    {
        // FIXME: Get rid of FindObjectOfType
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;

        GameObject bullet = Instantiate(isNormal ? normalBullet : counterBullet, bulletShootPos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = (playerPosition - bulletShootPos).normalized * bulletSpeed;
        Destroy(bullet, 3.0f);
        yield return null;
    }

    /*private IEnumerator IdleRoutine(float time)
    {
        float moveTime;
        for (float t = 0; t < time; t += moveTime)
        {
            moveTime = Random.Range(0.5f, 2f);
            if (t + moveTime > time)
            {
                moveTime = time - t + float.Epsilon;
            }

            Vector3 direction = Random.insideUnitCircle.normalized;
            while (!map.Contains(transform.position + direction * moveSpeed * moveTime))
            {
                direction = Random.insideUnitCircle.normalized;
            }



            float x = Random.Range(map.min.x, map.max.x);
            float y = Random.Range(map.min.y, map.max.y);
            float z = Random.Range(map.min.z, map.max.z);
            yield return MoveRoutine(transform.position + direction * moveSpeed * moveTime, moveTime);
        }
    }*/

    protected override void OnStunned()
    {

    }
}
