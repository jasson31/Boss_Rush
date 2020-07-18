using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public interface IWeaponAttack
{
    void Attack(Vector2 handPosition, Vector2 mousePosition, int damage);
}



public class Weapon : ScriptableObject
{
    public int weaponID;
    public int health;
    public int damage;
    public int skillDamage;

    public IWeaponAttack attackBehaviour;
    public Skill skill;

    public Sprite sprite;

    private void OnDestroy()
    {
        OnUnmountWeapon();
    }

    public void OnMountWeapon()
    {

    }

    public void OnUnmountWeapon()
    {

    }

    public void WeaponAttack(Vector2 handPosition, Vector2 mousePosition)
    {
        attackBehaviour.Attack(handPosition, mousePosition, damage);
    }

    public void WeaponSkill(Vector2 handPosition, Vector2 mousePosition)
    {
        skill.WeaponSkill(handPosition, mousePosition);
    }
}
