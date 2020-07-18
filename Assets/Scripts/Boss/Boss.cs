using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State
{
    public State(string name, Action enter, Action exit, Action update)
    {
        this.name = name;
        Enter = enter;
        Exit = exit;
        Update = update;
    }

    public string name;
    public Action Enter;
    public Action Exit;
    public Action Update;
}

public class StateMachine
{
    private Dictionary<string, State> states;
    private State CurState { get { return states[curState]; } }
    private string curState;

    private void Transtion(string nextState)
    {
        CurState.Exit();
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
    protected List<StateMachine> stateMachines;
    protected int phase;
    protected StateMachine StateMachine { get { return stateMachines[phase]; } }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        StateMachine.Update();
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

    protected void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
