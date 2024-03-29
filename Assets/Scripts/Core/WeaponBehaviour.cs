﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    private Animator anim;

    private float prevWeaponChangedTime = -100;
    private float weaponChangeCoolTime = 2;


    public static List<Weapon> weapons = new List<Weapon>();
    public int weaponIndex = 0;
    private Weapon Weapon { get { return weapons.Count > weaponIndex ? weapons[weaponIndex] : null; } }

    public bool check = true;

    public GameObject testBullet;
    public GameObject testLaser;

    public Sprite testImage1, testImage2;
    public AnimatorOverrideController testController1, testController2;

    private void OnDrawGizmos()
    {
        if (Weapon)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Weapon.range);
        }
    }
    private void Awake()
    {
        check = true;
		if (weapons.Count == 0)
		{
			weapons.Add(WeaponFactory.CreateWeapon(101));
			weapons.Add(WeaponFactory.CreateWeapon(201));
		}
		anim = GetComponent<Animator>();
    }

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

    private void Update()
    {
        Weapon?.Update();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ChangeWeapon(2);
		}
	}

    public void ChangeWeapon(int index)
    {
        if(index != weaponIndex && index < weapons.Count)
        {
            if(Time.time - prevWeaponChangedTime > weaponChangeCoolTime)
            {
                Weapon.OnUnmountWeapon();
                weaponIndex = index;
                Weapon.OnMountWeapon();
                prevWeaponChangedTime = Time.time;
            }
        }
    }

    public void AddWeapon(int weaponID)
    {
        weapons.Add(WeaponFactory.CreateWeapon(weaponID));
    }

    public void GetDamaged(float damage)
    {
        Weapon.health -= damage;
        if (Weapon.health <= 0)
        {
            Weapon weapon = Weapon;
            check = false;
            weapons.Remove(weapon);
            Destroy(weapon);
            Debug.Log(weapons.Count);
            if (weapons.Count <= 0)
            {
                //Game Over Routine
            }
            else
            {
                weaponIndex = 0;
                Weapon.OnMountWeapon();
                //Weapon Break Routine
            }
        }
    }

    private void UseWeaponAttack(Vector2 mousePosition)
    {
        anim.SetTrigger("Attack");

        Weapon?.WeaponAttack(transform.position, mousePosition);
    }

    private void UseWeaponSkill(Vector2 mousePosition)
    {
        Weapon?.WeaponSkill(transform.position, mousePosition);
    }
}
