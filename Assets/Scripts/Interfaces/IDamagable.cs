using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    float Health { get; }
    void GetDamaged(float damage);
}
