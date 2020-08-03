using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponAttack
{
    void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range);
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
public class StabAttack : IWeaponAttack
{
    public void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range)
    {
        Vector2 dir = (mousePosition - handPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x);

        Vector2 boxCenter = handPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * range / 2;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, new Vector2(range, 0.3f), angle * Mathf.Rad2Deg);

        foreach (var hit in hits)
        {
            IDamagable target = hit.GetComponent<IDamagable>();
            target?.GetDamaged(damage);
        }
    }
}

