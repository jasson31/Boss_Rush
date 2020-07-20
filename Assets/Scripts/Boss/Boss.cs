using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;

public struct State
{
    public State(string name, Action enter, Action exit, Action update)
    {
        this.name = name;
        this.Enter = enter;
        this.Exit = exit;
        this.Update = update;
    }

    public string name;
    public Action Enter;
    public Action Exit;
    public Action Update;
}

public class StateMachine
{
    public int minHealth;
    public int AttackCount { get; private set; }
    private Dictionary<string, State> states = new Dictionary<string, State>();


    public State GetState(string state)
    {
        return states[state];
    }

    public void AddState(State runState, List<State> attackState)
    {
        states.Add("Run", runState);
        AttackCount = attackState.Count;
        for (int i = 0; i < attackState.Count; i++)
        {
            Debug.Log("Attack" + i.ToString());
            states.Add("Attack" + i.ToString(), attackState[i]);
        }
    }

    public void AddState(State runState, List<State> attackState, State additionalState, bool isDeath)
    {
        AddState(runState, attackState);
        states.Add(isDeath ? "Death" : "Stunned", additionalState);
    }

    public void AddState(State runState, List<State> attackState, State deathState, State stunnedState)
    {
        AddState(runState, attackState, deathState, true);
        states.Add("Stunned", stunnedState);
    }

}

public abstract class Boss : MonoBehaviour, IDamagable
{
    public bool isInvincible = false;
    protected Animator animator;
    protected int currentAttack;

    [SerializeField]
    protected List<AnimatorController> phaseController;

    public int Health { get; set; }
    protected List<StateMachine> stateMachines = new List<StateMachine>();
    [SerializeField]
    protected int phase = -1;
    protected StateMachine StateMachine { get { return stateMachines[phase]; } }

    private void Start()
    {
        animator = GetComponent<Animator>();
        Init();
    }

    private void Update()
    {
        //StateMachine.Update();
    }

    public void StateEnter(string state)
    {
        if(state == "Attack") StateMachine.GetState(state + currentAttack.ToString()).Enter();
        else StateMachine.GetState(state).Enter();
    }

    public void StateUpdate(string state)
    {
        if (state == "Attack") StateMachine.GetState(state + currentAttack.ToString()).Update();
        else StateMachine.GetState(state).Update();
    }

    public void StateExit(string state)
    {
        if (state == "Attack") StateMachine.GetState(state + currentAttack.ToString()).Exit();
        else StateMachine.GetState(state).Exit();
    }


    protected void ChangePhase()
    {
        /*if(phase != -1)
        {
            StateMachine.StateExit();
        }
        phase += 1;
        StateMachine.StateEnter();*/
        currentAttack = 0;
        phase += 1;
        animator.runtimeAnimatorController = phaseController[phase];
    }

    protected abstract void Init();

    public virtual void GetDamaged(int damage)
    {
        if(!isInvincible)
        {
            Health -= damage;

            if (StateMachine.minHealth == 0 && Health <= 0)
            {
                animator.SetTrigger("Death");
            }
            else if (Health <= StateMachine.minHealth)
            {
                Health = StateMachine.minHealth;
                ChangePhase();
            }
            Debug.Log("Ouch" + Health);
        }
    }

    public IEnumerator CameraZoomIn(float resetTime)
    {
        float zoomInTime = 1;
        float zoomOutTime = 0.5f;

        Vector3 originalPos = Camera.main.transform.position;
        float originalSize = Camera.main.orthographicSize;

        Vector3 destPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        float destSize = 3;

        for (float timer = 0; timer <= zoomInTime; timer += Time.deltaTime)
        {
            yield return null;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, destPos, timer / zoomInTime);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, destSize, timer / zoomInTime);
        }

        yield return new WaitForSeconds(resetTime - zoomInTime - zoomOutTime);


        for (float timer = 0; timer <= zoomOutTime; timer += Time.deltaTime)
        {
            yield return null;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, originalPos, timer / zoomOutTime);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originalSize, timer / zoomOutTime);
        }
    }

    public void FollowPlayer(float minDistance)
    {
        //TODO
        if(DistanceFromPlayer() > minDistance)
        {
            Vector3 direction = (GameObject.Find("Player").transform.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime;
            LookPlayer();
        }
    }

    public void LookPlayer()
    {

    }

    public float DistanceFromPlayer()
    {
        //TODO
        return (GameObject.Find("Player").transform.position - transform.position).magnitude;
    }

    protected void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
