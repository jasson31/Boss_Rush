using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoss : Boss
{
    float phaseEnterTimer = 0;
    IEnumerator IdleWait(float waitTime, Action callback)
    {
        float startTime = Time.time;
        while (Time.time - startTime < waitTime)
        {
            yield return null;
        }
        callback();
    }
    protected override void Init()
    {
        StateMachine phase1 = new StateMachine();

        phase1.StateEnter += delegate
        {
            phase1.Transition("phasae1Enter");
        };

        State phase1Enter = new State("phasae1Enter",
            delegate
            {
                phaseEnterTimer = Time.time;
                StartCoroutine(CameraZoomIn(2));
            },
            delegate
            {

            },
            delegate
            {
                if (Time.time - phaseEnterTimer > 2)
                {
                    phase1.Transition("idle");
                }
            });

        State idle = new State("idle",
            delegate
            {

            },
            delegate
            {

            },
            delegate
            {
                FollowPlayer();
                if (DistanceFromPlayer() < 1)
                {
                    Debug.Log("Attack");
                }
            });




        phase1.AddState(phase1Enter);
        phase1.AddState(idle);





        stateMachines.Add(phase1);

        ChangePhase();

    }

    protected override void OnDead()
    {

    }

}
