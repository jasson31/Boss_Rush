﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ListWrapper
{
    public List<AnimatorOverrideController> attackPatterns;
}

[System.Serializable]
struct DropInfo
{
	public int weaponId;
	public float probability;
}

public abstract class Boss : MonoBehaviour, IDamagable
{
    public int Phase { get; protected set; }
    protected Coroutine CurrentRoutine { get; private set; }
    private Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

    public float Health { get; protected set; }
    public float MaxHealth { get; protected set; }

    protected Animator animator;

    [SerializeField]
    private GameObject damageTextPrefab;

    private List<DamageText> pooledDamageTexts = new List<DamageText>();
    protected Player player;
    protected Collider2D col;
    protected Rigidbody2D rb;

	[SerializeField]
	private List<DropInfo> dropInfos;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        Init();
    }

    private void Update()
    {
        if (CurrentRoutine == null)
        {
            NextRoutine();
        }
    }

    private DamageText GetPooledDamageText()
    {
        DamageText damageText = null;
        if (pooledDamageTexts.Count > 0)
        {
            damageText = pooledDamageTexts[pooledDamageTexts.Count - 1];
            damageText.gameObject.SetActive(true);
            damageText.boss = this;
            pooledDamageTexts.RemoveAt(pooledDamageTexts.Count - 1);
        }
        else
        {
            damageText = Instantiate(damageTextPrefab).GetComponent<DamageText>();
            damageText.boss = this;          
        }
        damageText.transform.position = transform.position + 2 * Vector3.up + Vector3.back;
        return damageText;
    }

    public void RetrieveDamageText(DamageText text)
    {
        pooledDamageTexts.Add(text);
    }

    public virtual void GetDamaged(float damage)
    {
        damage += UnityEngine.Random.Range(-1, 2);
        GetPooledDamageText().SetText(damage.ToString());
        StartCoroutine(DamageRoutine());
        Health -= damage;
        IngameUIManager.inst.SetBossHealthBar(Health, MaxHealth);
        if(Health < 0)
        {
			DropItem();
            FindObjectOfType<ClearChecker>().isClear = true;
            SceneManager.LoadScene("SelectScreen");
        }
    }

    private IEnumerator DamageRoutine()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            foreach (var renderer in renderers)
            {
                if(renderer)
                {
                    renderer.color = Color.Lerp(Color.red, Color.white, t * 4);
                }
        }
            yield return null;
        }
        
    }

    protected Vector3 GetPlayerPos()
    {
        // FIXME: Get rid of FindObjectOfType
        return FindObjectOfType<Player>().transform.position;
    }

    protected virtual void Init()
    {

    }

	private void DropItem()
	{
		float rand = UnityEngine.Random.value;
		float sum = 0;
		foreach (var dropInfo in dropInfos)
		{
			sum += dropInfo.probability;
			if (sum > rand)
			{
				FindObjectOfType<WeaponBehaviour>().AddWeapon(dropInfo.weaponId);
				break;
			}
		}
	}

    private void NextRoutine()
    {
        if (nextRoutines.Count <= 0)
        {
            nextRoutines = DecideNextRoutine();
        }
        StartCoroutineBoss(nextRoutines.Dequeue());
    }

    /// <summary>
    /// 다음 행동 루틴들을 queue로 만들어 리턴
    /// </summary>
    protected abstract Queue<IEnumerator> DecideNextRoutine();

    protected abstract void OnStunned();

    public virtual void Stun(float time)
    {
        nextRoutines.Clear();
        StartCoroutineBoss(NewActionRoutine(StunRoutine(time)));
    }

    private void StartCoroutineBoss(IEnumerator coroutine)
    {
        if (CurrentRoutine != null)
        {
            StopCoroutine(CurrentRoutine);
        }
        CurrentRoutine = StartCoroutine(coroutine);
    }

    protected virtual IEnumerator StunRoutine(float time)
    {
        animator.SetTrigger("Stunned");
		OnStunned();
		yield return new WaitForSeconds(time);
        animator.SetTrigger("StunEnd");
    }

    protected IEnumerator NewActionRoutine(IEnumerator action)
    {
        yield return action;
        CurrentRoutine = null;
    }


    protected IEnumerator MoveRoutine(Vector3 destination, float time)
    {
        yield return MoveRoutine(destination, time, AnimationCurve.Linear(0, 0, 1, 1));
    }
    protected IEnumerator MoveRoutine(Vector3 destination, float time, AnimationCurve curve)
    {
        Vector3 startPosition = transform.position;
		Debug.Log(startPosition);
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            transform.position =
                Vector3.Lerp(startPosition, destination, curve.Evaluate(t / time));
            yield return null;
        }
        transform.position = destination;
    }

    protected IEnumerator WaitRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

	protected void StartPhaseTransition(float time, int nextPhase)
	{
		nextRoutines.Clear();
		StartCoroutineBoss(NewActionRoutine(PhaseTransitionRoutine(time, nextPhase)));
	}

	protected virtual IEnumerator PhaseTransitionRoutine(float time, int phase)
	{
		col.enabled = false;
		yield return new WaitForSeconds(time);
		col.enabled = true;
		Phase = phase;
	}

	protected void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
