using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDamagable : MonoBehaviour, IDamagable
{
    public float Health => throw new System.NotImplementedException();

    public void GetDamaged(float damage)
    {
        Debug.Log(name + " Attacked! " + damage + " Damage!");
    }
}
