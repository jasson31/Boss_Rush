using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoss : Boss
{
    protected override void Init()
    {
        phaseCount = 1;
        patternCount = new List<int>();

        patternCount.Add(1);
        minWaitTime = 2;
        maxWaitTime = 4;
    }

    protected override void OnDead()
    {

    }

    public void Phase1Pattern1()
    {

    }

}
