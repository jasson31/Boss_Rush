using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> weapons = new List<Weapon>();
    private int weaponIndex = 0;
    private Weapon Weapon { get { return weapons.Count > weaponIndex ? weapons[weaponIndex] : null; } }

    [SerializeField]
    private Transform handTransform;

    private void OnEnable()
    {
        if (weapons.Count > weaponIndex)
        {
            Weapon.OnMountWeapon();
        }
        InputHandler.inst.OnAttackKeyDown += UseWeaponAttack;
    }

    private void OnDisable()
    {
        InputHandler.inst.OnAttackKeyDown -= UseWeaponAttack;
    }

    public void ChangeWeapon(int index)
    {
        Weapon.OnUnmountWeapon();
        weaponIndex = index;
        Weapon.OnMountWeapon();
    }

    public void AddWeapon(int weaponID)
    {
        weapons.Add(StaticData.inst.CreateWeapon(weaponID));
    }

    public void GetDamaged(int damage)
    {
        Weapon.health -= damage;
        if (Weapon.health <= 0)
        {
            Weapon weapon = Weapon;
            weapons.Remove(weapon);
            Destroy(weapon);
            if (weapons.Count <= 0)
            {
                //Game Over Routine
            }
            else if(weaponIndex >= weapons.Count)
            {
                weaponIndex = 0;
                //Weapon Break Routine
            }
        }
        
    }

    private void UseWeaponAttack(Vector2 mousePosition)
    {
        Weapon?.WeaponAttack(handTransform.position, mousePosition);
    }

    private void UseWeaponSkill(Vector2 mousePosition)
    {
        Weapon?.WeaponSkill(handTransform.position, mousePosition);
    }
}
