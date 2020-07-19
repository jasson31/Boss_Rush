using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.PlayerLoop;

public interface IWeaponAttack
{
    void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range);
}

public class SwordAttack : IWeaponAttack
{
    public void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(handPosition, range);
        foreach (var hit in hits)
        {
            IDamagable target = hit.GetComponent<IDamagable>();
            target?.GetDamaged(damage);
        }
    }
}

public class Weapon : ScriptableObject
{
    public int weaponID;
    public int health;
    public int MaxHealth { get; private set; }
    public int damage;
    public float range;
    public float coolTime;
    private float timer;

    public IWeaponAttack attackBehaviour;
    public Skill skill;

    public Sprite sprite;

    private void OnDestroy()
    {
        OnUnmountWeapon();
    }

    public void Update()
    {
        timer = Mathf.Max(0, timer - Time.deltaTime);
    }

    public void OnMountWeapon()
    {

    }

    public void OnUnmountWeapon()
    {

    }

    public void WeaponAttack(Vector2 handPosition, Vector2 mousePosition)
    {
        if (timer <= 0)
        {
            attackBehaviour.Attack(handPosition, mousePosition, damage, range);
            timer = coolTime;
        }
    }

    public void WeaponSkill(Vector2 handPosition, Vector2 mousePosition)
    {
        skill.WeaponSkill(handPosition, mousePosition);
    }
}
