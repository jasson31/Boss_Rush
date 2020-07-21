using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoss : Boss
{

    protected override void Init()
    {
        StateMachine phase1 = new StateMachine();
        StateMachine phase2 = new StateMachine();

        State p1Run = new State();
        State p1Stunned = new State();
        State p1Attack1 = new State();
        State p1Attack2 = new State();

        State p2Run = new State();
        State p2Stunned = new State();
        State p2Attack = new State();
        State p2Death = new State();


        p1Run.Enter += delegate
        {
            Debug.Log("Phase 1 Run Start");
        };
        p1Run.Update += delegate
        {
            FollowPlayer(1);
            if(DistanceFromPlayer() < 1)
            {
                currentAttack = UnityEngine.Random.Range(0, phase1.AttackCount);
                animator.SetTrigger("Attack");
            }
        };
        p1Run.Exit += delegate
        {
            animator.runtimeAnimatorController = phaseController[phase].attackPatterns[currentAttack];
            animator.ResetTrigger("Attack");
        };


        p1Stunned.Enter += delegate
        {
            Debug.Log("Boss Stunned");
        };
        p1Stunned.Update += delegate
        {

        };
        p1Stunned.Exit += delegate
        {
            Debug.Log("Boss Stun end");
        };

        p1Attack1.Enter += delegate
        {
            Debug.Log("Phase 1 Attack1 Start");
        };
        p1Attack1.Update += delegate
        {

        };
        p1Attack1.Exit += delegate
        {

        };


        p1Attack2.Enter += delegate
        {
            Debug.Log("Phase 1 Attack2 Start");
        };
        p1Attack2.Update += delegate
        {

        };
        p1Attack2.Exit += delegate
        {

        };

        List<State> p1AttackStates = new List<State>();
        p1AttackStates.Add(p1Attack1);
        p1AttackStates.Add(p1Attack2);
        phase1.AddState(p1Run, p1AttackStates, p1Stunned, false);
        phase1.minHealth = 5;




        p2Run.Enter += delegate
        {
            Debug.Log("Phase 2 Run Start");
        };
        p2Run.Update += delegate
        {
            FollowPlayer(1);
            if (DistanceFromPlayer() < 1)
            {
                animator.SetTrigger("Attack");
            }
        };
        p2Run.Exit += delegate
        {
            animator.runtimeAnimatorController = phaseController[phase].attackPatterns[currentAttack];
            animator.ResetTrigger("Attack");
        };

        p2Attack.Enter += delegate
        {
            Debug.Log("Phase 2 Attack Start");
        };
        p2Attack.Update += delegate
        {

        };
        p2Attack.Exit += delegate
        {

        };


        p2Stunned.Enter += delegate
        {
            Debug.Log("Boss Stunned");
        };
        p2Stunned.Update += delegate
        {

        };
        p2Stunned.Exit += delegate
        {
            Debug.Log("Boss Stun end");
        };

        p2Death.Enter += delegate
        {
            Debug.Log("Phase 2 Death Start");
            Destroy(gameObject);
        };
        p2Death.Update += delegate
        {

        };
        p2Death.Exit += delegate
        {

        };

        List<State> p2AttackStates = new List<State>();
        p2AttackStates.Add(p2Attack);
        phase2.AddState(p2Run, p2AttackStates, p2Death, p2Stunned);
        phase2.minHealth = 0;

        Health = 10;

        stateMachines.Add(phase1);
        stateMachines.Add(phase2);

        ChangePhase();

    }

}
