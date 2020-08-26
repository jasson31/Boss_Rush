using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoonLine : MonoBehaviour, IDamagable
{
    public SpiderBoss spiderBoss;
    public int Health { get; protected set; }

    public void GetDamaged(int damage)
    {
        //StartCoroutine(spiderBoss.CocoonBreakRoutine());
        spiderBoss.Stun(0.5f);
    }
}
