using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ShootAttackBullet : MonoBehaviour
{
    private int damage;
    private Vector2 startPos;
    private float range;

    public void Init(int _damage, Vector2 _startPos, float _range, Vector2 velocity)
    {
        damage = _damage;
        startPos = _startPos;
        range = _range;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<IDamagable>()?.GetDamaged(damage);
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, startPos) > range)
        {
            Destroy(gameObject);
        }
    }
}