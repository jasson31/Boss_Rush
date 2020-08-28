using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranBoss : Boss
{
    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>;
        float rand = Random.value;

        switch (Phase)
        {
            case 0:
                if(rand < 0.2f)
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }
                else if(rand < 0.4f)
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }
                else if(rand < 0.6f)
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }
                else if(rand < 0.8f)
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }
                else if(rand < 0.9f)
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine());
                }

        }

        return nextRoutines;
    }
}
