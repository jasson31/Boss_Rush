using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    private Dictionary<string, State> states = new Dictionary<string, State>();
    private State CurState { get { return states[curState]; } }
    private string curState = null;

    public Action StateEnter;
    public Action StateExit;

    public void Transition(string nextState)
    {
        if(curState != null)
        {
            CurState.Exit();
        }
        curState = nextState;
        CurState.Enter();
    }

    public void Update()
    {
        CurState.Update();
    }

    public void AddState(State state)
    {
        states.Add(state.name, state);
    }
}

public abstract class Boss : MonoBehaviour, IDamagable
{
    public int Health { get; private set; }
    protected List<StateMachine> stateMachines = new List<StateMachine>();
    protected int phase = -1;
    protected StateMachine StateMachine { get { return stateMachines[phase]; } }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    protected void ChangePhase()
    {
        if(phase != -1)
        {
            StateMachine.StateExit();
        }
        phase += 1;
        StateMachine.StateEnter();
    }

    protected abstract void Init();
    protected abstract void OnDead();

    public virtual void GetDamaged(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnDead();
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

    public void FollowPlayer()
    {
        //TODO
        Vector3 direction = (GameObject.Find("Player").transform.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime;
        GetComponent<SpriteRenderer>().flipX = direction.x < 0;
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
