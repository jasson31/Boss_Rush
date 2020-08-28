using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranBoss : Boss
{

    [SerializeField]



    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();
        float rand = Random.value;

        switch (Phase)
        {
            case 0:
                if(rand < 0.2f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(AKMRoutine()));
                }
                else if(rand < 0.4f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(SniperRoutine()));
                }
                else if(rand < 0.6f)
                {
                    //nextRoutines.Enqueue(NewActionRoutine(BayonetRoutine()));
                }
                else if(rand < 0.8f)
                {
                    //nextRoutines.Enqueue(NewActionRoutine(GrenadeRoutine()));
                }
                else if(rand < 0.9f)
                {
                    //nextRoutines.Enqueue(NewActionRoutine(SmokeRoutine()));
                }
                else
                {
                    //nextRoutines.Enqueue(NewActionRoutine(FlareRoutine()));
                }
                break;

        }

        return nextRoutines;
    }

    private IEnumerator AKMRoutine()
    {

        yield return null;
    }

    private IEnumerator SniperRoutine()
    {

        yield return null;
    }
    
    public override void GetDamaged(int damage)
    {
        base.GetDamaged(damage);
        if (MaxHealth * 0.25f >= Health && Phase == 0)
        {
            Phase = 1;
        }
        if (Health <= 0)
            gameObject.SetActive(false);
    }

    protected override void OnStunned()
    {

    }
}
