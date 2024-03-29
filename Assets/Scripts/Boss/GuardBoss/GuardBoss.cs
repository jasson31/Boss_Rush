﻿using System.Collections;
using System.Collections.Generic;
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

	[SerializeField]
	private LayerMask wallLayerMask;

    private bool isCollidePlayer = false;

    const float bulletSpeed = 20;
    const float moveSpeed = 5;

	const float meleeRange = 1.5f;

    private LineRenderer lr;

    // FIXME: Play particle with Observer e.g. OnShake += ... and move this to map related script
    [SerializeField]
    private ParticleSystem debrisParticle;

	protected override void Start()
	{
        base.Start();
		Phase = 0;
        MaxHealth = Health = 200;
        lr = GetComponent<LineRenderer>();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
        Vector2 direction = playerPosition - transform.position;
        direction.Normalize();
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y));
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

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
#endif

	public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);
        if (MaxHealth * 0.3f >= Health && Phase == 0)
        {
            animator.SetTrigger("Phase1To2");
            StartPhaseTransition(2f, 1);
		}
        if (MaxHealth * 0.05f >= Health && Phase == 1)
        {
            animator.SetTrigger("Phase2To3");
            StartPhaseTransition(2f, 2);
        }
        if (Health <= 0)
            gameObject.SetActive(false);
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
                    for (int i = 0; i < 5; i++)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(1f, 3, 0.2f)));
                    }
                }
                else if (rand < 0.4f)
                {
                    int idx = FindObjectOfType<Player>().transform.position.x > 0 ? 3 : 2;
                    nextRoutines.Enqueue(NewActionRoutine(LandRoutine(targetPositions[idx])));
                    nextRoutines.Enqueue(NewActionRoutine(ShotLaserRoutine(1.5f, 2f)));
                    nextRoutines.Enqueue(NewActionRoutine(StunRoutine(3)));
                }
                else if (rand < 0.9f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(ShotRoutine(1f, 5, 0.1f)));
                }
                else
                {
                    Vector3 startPosition = transform.position;
                    Vector3 destination = new Vector3(2 * targetPositions[4].x - startPosition.x, startPosition.y, startPosition.z);
                    nextRoutines.Enqueue(NewActionRoutine(SetCollide(true)));
                    nextRoutines.Enqueue(NewActionRoutine(JumpUpRoutine(targetPositions[4])));
                    nextRoutines.Enqueue(NewActionRoutine(JumpDownRoutine(destination)));
                    nextRoutines.Enqueue(NewActionRoutine(SetCollide(false)));
                }
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(3f)));
                break;
			case 1:
                if (rand < 0.45f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(MoveRoutine(targetPositions[4], 1)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                    nextRoutines.Enqueue(NewActionRoutine(ChargeToPlayerRoutine()));
                    nextRoutines.Enqueue(NewActionRoutine(StunRoutine(1)));
                }
                else if (rand < 0.65f)
                {
                    int idx = FindObjectOfType<Player>().transform.position.x > 0 ? 3 : 2;
					nextRoutines.Enqueue(NewActionRoutine(Land2Routine(targetPositions[idx])));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                    nextRoutines.Enqueue(NewActionRoutine(ChargeRoutine(idx == 2 ? Vector2.right : Vector2.left)));
                }
				else
				{
					nextRoutines.Enqueue(NewActionRoutine(MeleeAttackRoutine()));
					nextRoutines.Enqueue(NewActionRoutine(MeleeAttackRoutine()));
				}
                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(2.5f)));
                break;
            case 2:
                for (int i = 0; i < 9; i++)
                {
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                    nextRoutines.Enqueue(NewActionRoutine(ChargeToPlayerRoutine()));
                }
                nextRoutines.Enqueue(NewActionRoutine(StunRoutine(5)));
                break;
        }

        return nextRoutines;
    }

    private IEnumerator ShotRoutine(float waitTime, int bulletCount, float interval)
    {
        Player player = FindObjectOfType<Player>();
        Vector3 playerPosition;

        lr.enabled = true;
        lr.startWidth = 0.01f;
        animator.SetTrigger("BulletShoot");
        for (float t = 0; t < waitTime; t += Time.deltaTime)
        {
            playerPosition = player.transform.position;  
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, playerPosition);
            yield return null;
        }

        lr.enabled = false;
        playerPosition = player.transform.position;

        for (int i = 0; i < bulletCount; i++)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = (playerPosition - transform.position).normalized * bulletSpeed;
            yield return new WaitForSeconds(interval);
        }
        animator.SetTrigger("BulletShootEnd");
    }

    private IEnumerator LandRoutine(Vector3 dest)
    {
        animator.SetTrigger("Land");
        yield return MoveRoutine(dest, 0.16f);
    }

    private IEnumerator ShotLaserRoutine(float waitTime, float width)
    {
        lr.enabled = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + 100 * (FindObjectOfType<Player>().transform.position - transform.position).normalized);
        lr.startWidth = 0.01f;
        yield return new WaitForSeconds(waitTime - 0.35f);
        animator.SetTrigger("LaserShoot");
        yield return new WaitForSeconds(0.35f);
        CameraController.inst.ShakeCamera(0.5f, 0.5f);
        debrisParticle.Play();
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            lr.startWidth = width * (1-t) * (1-t);
            yield return null;
        }
        animator.SetTrigger("LaserShootEnd");
        lr.enabled = false;
    }

    private IEnumerator JumpUpRoutine(Vector3 dest)
    {
        animator.SetTrigger("Up");
        yield return MoveRoutine(dest, 2, moveCurve);
    }

    private IEnumerator JumpDownRoutine(Vector3 dest)
    {
        animator.SetTrigger("Down");
        yield return MoveRoutine(dest, 2, moveCurve);
        animator.SetTrigger("DownEnd");
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
            float y = Random.Range(map.min.y, map.max.y);
            float z = Random.Range(map.min.z, map.max.z);
            yield return MoveRoutine(new Vector3(x,y,z), Vector2.Distance(transform.position, new Vector2(x,y)) / moveSpeed);
        }
    }

    private IEnumerator Land2Routine(Vector3 dest)
    {
        animator.SetTrigger("Land2");
        yield return MoveRoutine(dest, 1);
    }

    private IEnumerator MeleeAttackRoutine()
    {
		Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
		Vector2 direction = (playerPosition - transform.position).normalized;

		Vector3 destination = transform.position + new Vector3(direction.x, direction.y) * Mathf.Min(Vector2.Distance(playerPosition, transform.position) - 0.2f, 5);

		yield return MoveRoutine(destination, 0.3f, moveCurve);
		animator.SetTrigger("Swing");
		playerPosition = FindObjectOfType<Player>().transform.position;
		Debug.Log(Vector2.Angle(direction, playerPosition - transform.position));
		if (Vector2.Distance(transform.position, playerPosition) <= meleeRange && Vector2.Angle(direction, playerPosition - transform.position) <= 60)
		{
			FindObjectOfType<Player>().GetDamaged(0.25f);
		}

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SetCollide(bool value)
    {
        isCollidePlayer = value;
        yield return null;
    }

    private IEnumerator ChargeToPlayerRoutine()
    {
        Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
        Vector2 direction = playerPosition - transform.position;
        yield return ChargeRoutine(direction);
    }

    private IEnumerator ChargeRoutine(Vector2 direction)
    {
        direction.Normalize();
        if(direction == Vector2.right || direction == Vector2.left)
        {
            animator.SetTrigger("Dash");
        }
        else
        {
            animator.SetTrigger("Down2");
        }

        var hit = Physics2D.Raycast(transform.position, direction, float.MaxValue, wallLayerMask);

		Collider2D col = GetComponent<Collider2D>();
		yield return SetCollide(true);
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.velocity = direction * 30;
        yield return new WaitForSeconds(0.1f);

		Player player = FindObjectOfType<Player>();
		Collider2D playerCol = player.GetComponent<Collider2D>();

		while (!col.IsTouching(hit.collider))
		{
			yield return null;
		}
        CameraController.inst.ShakeCamera(0.5f, 0.5f);
        debrisParticle.Play();
        rb.velocity = Vector2.zero;
		yield return SetCollide(false);
        if (direction == Vector2.right || direction == Vector2.left)
        {
            animator.SetTrigger("DashEnd");
        }
        else
        {
            animator.SetTrigger("Down2End");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (isCollidePlayer && player != null)
        {
            player.GetDamaged(0.5f);
        }
    }

	protected override void OnStunned()
    {
        isCollidePlayer = false;
    }
}
