using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private List<Weapon> weapons = new List<Weapon>();
    private int weaponIndex = 0;
    private Weapon Weapon { get { return weapons.Count > weaponIndex ? weapons[weaponIndex] : null; } }

    public GameObject testBullet;
    public GameObject testLaser;

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
        Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
        //weapon.attackBehaviour = new LaserAttack(testLaser, 2, 2);
        weapon.attackBehaviour = new StabAttack();
        weapon.damage = 3;
        weapon.range = 1;
        weapon.moveSpeed = 7;
        weapons.Add(weapon);
        Debug.Log(weapon.attackBehaviour);

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
        Weapon.Update();
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

    public void GetDamaged(float damage)
    {
        /*Weapon.health -= damage;
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
        }*/
        
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
