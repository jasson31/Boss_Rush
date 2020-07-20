using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoss : Boss
{
    float phaseEnterTimer = 0;
    float patternWaitStart = 0;
    float patternWaitTimer = 0;
    bool inPattern = false;

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
                Debug.Log("Idle");
                animator.SetTrigger("idle");
            },
            delegate
            {

            },
            delegate
            {
                FollowPlayer(1);
                if (DistanceFromPlayer() < 1)
                {
                    phase1.Transition("pattern1");
                }
            });

        State pattern1 = new State("pattern1",
            delegate
            {
                Debug.Log("Attack");
                patternWaitStart = Time.time;
                patternWaitTimer = 2;
                inPattern = false;
            },
            delegate
            {
                inPattern = false;
            },
            delegate
            {
                if(Time.time - patternWaitStart < patternWaitTimer)
                {
                    FollowPlayer(1);
                    if(DistanceFromPlayer() > 1)
                    {
                        patternWaitStart = Time.time;
                    }
                }
                else if(!inPattern)
                {
                    animator.SetTrigger("pattern");
                    inPattern = true;
                }

                if (animator.GetBool("patternEnd"))
                {
                    phase1.Transition("idle");
                    animator.SetBool("patternEnd", false);
                }
            });

        phase1.AddState(phase1Enter);
        phase1.AddState(idle);
        phase1.AddState(pattern1);





        stateMachines.Add(phase1);

        ChangePhase();

    }

    protected override void OnDead()
    {

    }

}
