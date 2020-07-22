using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public struct SkillInfo
{
    public int skillID;
    public string skill;
}
public struct WeaponInfo
{
    public int weaponID;
    public int skillID;
    public string attack;
    public int health;
    public int damage;
}

[Serializable]
public class StaticData : SingletonBehaviour<StaticData>
{
    private Dictionary<int, WeaponInfo> weaponDict;
    private Dictionary<int, SkillInfo> skillDict;
    private void Start()
    {

    }

    private void LoadWeaponData()
    {
        //Read Json?
    }

    private void LoadSkillData()
    {

    }

    public Weapon CreateWeapon(int weaponID)
    {
        WeaponInfo weaponInfo = weaponDict[weaponID];
        Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
        weapon.attackBehaviour = (IWeaponAttack)Activator.CreateInstance(Type.GetType(weaponInfo.attack));
        weapon.health = weaponInfo.health;
        weapon.damage = weaponInfo.damage;    
        weapon.skill = CreateSkill(weaponInfo.skillID);
        
        return weapon;
    }

    public Skill CreateSkill(int skillID)
    {
        SkillInfo skillInfo = skillDict[skillID];
        Skill skill = ScriptableObject.CreateInstance<Skill>();
        skill.skillBehaviour = (IWeaponSkill)Activator.CreateInstance(Type.GetType(skillInfo.skill));
        return skill;
    }
}
