using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocoonLine : MonoBehaviour, IDamagable
{
    public SpiderBoss spiderBoss;
    public float Health { get; protected set; }

    public void GetDamaged(float damage)
    {
        //StartCoroutine(spiderBoss.CocoonBreakRoutine());
        spiderBoss.Stun(0.5f);
        spiderBoss.GetComponent<Animator>().SetTrigger("CocoonExit");
    }
}
