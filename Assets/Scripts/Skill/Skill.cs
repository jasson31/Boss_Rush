using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponSkill
{
    int CoolTime { get; set; }
    void UseSkill(Vector2 handPosition, Vector2 mousePosition, int skillDamage);
}

public class Skill : ScriptableObject
{
    public int skillCooltime;
    public int skillDamage;

    public Sprite sprite;

    public IWeaponSkill skillBehaviour;

    public void WeaponSkill(Vector2 handPosition, Vector2 mousePosition)
    {
        skillBehaviour.UseSkill(handPosition, mousePosition, skillDamage);
    }
}
