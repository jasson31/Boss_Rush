using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBoss : Boss
{
    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        switch(Phase)
        {
            case 0:


                break;
        }



        return nextRoutines;
    }



    private IEnumerator ShotRoutine(int bulletCount, float interval)
    {
        // FIXME: Get rid of FindObjectOfType
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
        for (int i = 0; i < bulletCount; i++)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = (playerPosition - transform.position).normalized * bulletSpeed;
            yield return new WaitForSeconds(interval);
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
    }

    protected override void OnStunned()
    {

    }
}
