using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GuardBoss : Boss
{
    [SerializeField]
    private List<Vector3> targetPositions = new List<Vector3>();

    [SerializeField]
    private AnimationCurve moveCurve;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Bounds map;

    private bool isCollidePlayer = false;

    const float bulletSpeed = 20;
    const float moveSpeed = 3;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var position in targetPositions)
        {
            Gizmos.DrawWireSphere(position, 1);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(map.center, map.size);
    }

    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();
        float rand = Random.value;


        switch (Phase)
        {
            case 0:
                if (rand < 0.25f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(MoveRoutine(targetPositions[Random.Range(0, 2)], 2, moveCurve)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1)));
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(3, 0.2f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1)));
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(3, 0.2f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1)));
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(3, 0.2f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1)));
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(3, 0.2f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1)));
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(3, 0.2f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(3)));
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3)));
                }
                //else if (rand < 0.4f)
                //{

                //}
                else if (rand < 0.9f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(5, 0.1f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(3)));
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3)));
                }
                else
                {
                    Vector3 startPosition = transform.position;
                    Vector3 destination = new Vector3(2 * targetPositions[4].x - startPosition.x, startPosition.y, startPosition.z);
                    nextRoutines.Enqueue(NewActionRoutine(SetCollide(true)));
                    nextRoutines.Enqueue(NewActionRoutine(MoveRoutine(targetPositions[4], 2, moveCurve)));
                    nextRoutines.Enqueue(NewActionRoutine(MoveRoutine(destination, 2, moveCurve)));
                    nextRoutines.Enqueue(NewActionRoutine(SetCollide(false)));
                    nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3)));
                }
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
            Debug.Log(playerPosition);
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

    private IEnumerator SetCollide(bool value)
    {
        isCollidePlayer = value;
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
        isCollidePlayer = false;
    }
}
