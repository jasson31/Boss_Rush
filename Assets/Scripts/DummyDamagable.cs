using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDamagable : MonoBehaviour, IDamagable
{
    public int Health => throw new System.NotImplementedException();

    public void GetDamaged(int damage)
    {
        Debug.Log(name + " Attacked! " + damage + " Damage!");
    }
}
