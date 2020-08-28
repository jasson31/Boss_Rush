using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAttack
{
	CUT,
	STAB,
	PROJECTILE,
	HIT,
	LASER,
}

public enum WeaponMoveSpeed
{
	VERYSLOW,
	SLOW,
	MEDIUM,
	FAST,
	VERYFAST
}

public interface IWeaponAttack
{
    void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range);
}

[System.Serializable]
public class WeaponSpec
{
	public int id;
	public string weaponName;
	public int health;
	public int damage;
	public float range;
	public float coolTime;
	public WeaponAttack attack;
	public WeaponMoveSpeed speed;

	//For Projectile
	public string projectileName;

	//For Laser
	public float chargeTime;
	public float shotTime;
}

public class Weapon : ScriptableObject
{
    public int weaponID;
    public float moveSpeed;
	public string weaponName;
    public int health;
    public int MaxHealth { get; private set; }
    public int damage;
    public float range;
    public float coolTime;
    private float timer;

    public IWeaponAttack attackBehaviour;
    public Skill skill;

    public Sprite sprite;
    public AnimatorOverrideController controller;

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
        Game.inst.player.originSpeed = moveSpeed;
        Game.inst.player.speed = moveSpeed;
        GameObject.Find("Weapon").GetComponent<Animator>().runtimeAnimatorController = controller;
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

public class ShootAttack : IWeaponAttack
{
    private GameObject bullet;
    private float speed;

    public ShootAttack(GameObject _bullet, float _speed)
    {
        bullet = _bullet;
        speed = _speed;
    }

    public void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range)
    {
        Vector2 dir = (mousePosition - handPosition).normalized;
        GameObject.Instantiate(bullet, handPosition, Quaternion.identity).GetComponent<ShootAttackBullet>().Init(damage, handPosition, range, dir * speed);    
    }
}

public class LaserAttack : IWeaponAttack
{
    private GameObject laser;
    private float chargeTime;
    private float shootTime;

    public LaserAttack(GameObject _laser, float _chargeTime, float _shootTime)
    {
        laser = _laser;
        chargeTime = _chargeTime;
        shootTime = _shootTime;

    }

    public void Attack(Vector2 handPosition, Vector2 mousePosition, int damage, float range)
    {
        Vector2 dir = (mousePosition - handPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x);
        Vector2 endPos = handPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * range;
        Vector2 centerPos = (handPosition + endPos) / 2;
        LaserAttackBullet newLaser = GameObject.Instantiate(laser, centerPos, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Game.inst.player.transform).GetComponent<LaserAttackBullet>();
        Game.inst.StartCoroutine(newLaser.LaserBulletRoutine(damage, new Vector2(-0.5f, 0) * range, new Vector2(0.5f, 0) * range, chargeTime, shootTime));
    }
}
