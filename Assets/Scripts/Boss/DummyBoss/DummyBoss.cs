using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class DummyBoss : Boss
{
    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        nextRoutines.Enqueue(NewActionRoutine(TeleportRoutine(2)));
        nextRoutines.Enqueue(NewActionRoutine(AttackRoutine()));

        return nextRoutines;
    }

    protected override IEnumerator StunRoutine(float time)
    {
        animator.SetTrigger("Stunned");
        yield return base.StunRoutine(time);
        animator.SetTrigger("StunEnd");
    }

    private IEnumerator TeleportRoutine(float waitTime)
    {
        TeleportTo(FindObjectOfType<Player>().transform.position);
        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
    }

    protected override void OnStunned()
    {
        throw new NotImplementedException();
    }
}
