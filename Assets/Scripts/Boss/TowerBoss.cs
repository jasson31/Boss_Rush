using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBoss : Boss
{
    [SerializeField]
    private GameObject normalBullet;
    [SerializeField]
    private Vector3 shootPos;

    const float bulletSpeed = 5;




    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        float rand = Random.value;

        switch (0)
        {
            case 0:
                if (rand < 0.33f)
                {

                    nextRoutines.Enqueue(NewActionRoutine(CircularShotRoutine(5, 12, 1f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                }
                else if (rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(StraightShotRoutine(6, 0.1f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine(FanShotRoutine(5, 5, 0.7f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }

                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(0.8f)));
                break;

            case 1:
                break;
        }

        return nextRoutines;
    }


    private IEnumerator CircularShotRoutine(int numWave, int bulletCount, float interval)
    {
        for(int i=0; i<numWave; i++)
        {
            for (int j = 0; j < bulletCount; j++)
            {
                Instantiate(normalBullet, shootPos + new Vector3(Mathf.Cos(j * 30 * Mathf.PI / 180), Mathf.Sin(j * 30 * Mathf.PI / 180), 0), Quaternion.identity).GetComponent<Rigidbody2D>().velocity
                    = (new Vector3(Mathf.Cos(j * 30 * Mathf.PI / 180), Mathf.Sin(j * 30 * Mathf.PI / 180), 0)).normalized * bulletSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(interval);

        }

    }
    private IEnumerator StraightShotRoutine(int bulletCount, float interval)
    {

        for(int i=0; i < bulletCount; i++)
        {
            Instantiate(normalBullet, shootPos, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - shootPos).normalized * bulletSpeed * 2;
            yield return new WaitForSeconds(interval);
        }

    }

    private IEnumerator FanShotRoutine(int numWave, int bulletCount, float interval)
    {
        for(int i=0; i<numWave; i++)
        {
            for (int j = 0; j < bulletCount; j++)
            {
                Instantiate(normalBullet, shootPos, Quaternion.identity).GetComponent<Rigidbody2D>().velocity 
                    = (GetPlayerPos() + new Vector3(0, j-2, 0) / 0.5f * Mathf.Pow(-1, i) - shootPos).normalized * bulletSpeed * 1.5f;
                yield return new WaitForSeconds(0.15f);
            }
        yield return new WaitForSeconds(interval);
        }

    }


    private IEnumerator IdleRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

    protected override void OnStunned()
    {

    }

}



