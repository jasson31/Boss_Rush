using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ListWrapper
{
    public List<AnimatorOverrideController> attackPatterns;
}

public abstract class Boss : MonoBehaviour, IDamagable
{
    public int Phase { get; protected set; }
    private Coroutine currentRoutine = null;
    private Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

    public int Health { get; private set; }
   
    protected Animator animator;

    [SerializeField]
    private GameObject damageTextPrefab;

    private List<DamageText> pooledDamageTexts = new List<DamageText>();

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

    public virtual void GetDamaged(int damage)
    {
        damage += UnityEngine.Random.Range(-1, 2);
        GetPooledDamageText().SetText(damage.ToString());
        StartCoroutine(DamageRoutine());
        //Debug.Log("Ouch");
    }

    private IEnumerator DamageRoutine()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            foreach (var renderer in renderers)
            {
                renderer.color = Color.Lerp(Color.red, Color.white, t * 4);
        }
            yield return null;
        }
        
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentRoutine == null)
        {
            NextRoutine();
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
        OnStunned();
        StartCoroutineBoss(NewActionRoutine(StunRoutine(time)));
    }

    private void StartCoroutineBoss(IEnumerator coroutine)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(coroutine);
    }

    protected virtual IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

    protected IEnumerator NewActionRoutine(IEnumerator action)
    {
        yield return action;
        currentRoutine = null;
    }


    protected IEnumerator MoveRoutine(Vector3 destination, float time)
    {
        yield return MoveRoutine(destination, time, AnimationCurve.Linear(0, 0, 1, 1));
    }
    protected IEnumerator MoveRoutine(Vector3 destination, float time, AnimationCurve curve)
    {
        Vector3 startPosition = transform.position;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            transform.position =
                Vector3.Lerp(startPosition, destination, curve.Evaluate(t / time));
            yield return null;
        }
    }

    protected IEnumerator WaitRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

    protected void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
