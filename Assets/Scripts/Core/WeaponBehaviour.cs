using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    private Animator anim;

    private float prevWeaponChangedTime = -100;
    private float weaponChangeCoolTime = 2;


    [SerializeField]
    public List<Weapon> weapons = new List<Weapon>();
    public int weaponIndex = 0;
    private Weapon Weapon { get { return weapons.Count > weaponIndex ? weapons[weaponIndex] : null; } }

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
        Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
        //weapon.attackBehaviour = new LaserAttack(testLaser, 2, 2);
        weapon.attackBehaviour = new StabAttack();
        weapon.damage = 3;
        weapon.range = 1;
        weapon.moveSpeed = 7;
        weapon.sprite = testImage1;
        weapon.controller = testController1;
        weapons.Add(weapon);
        Debug.Log(weapon.attackBehaviour);

        Weapon weapon2 = ScriptableObject.CreateInstance<Weapon>();
        weapon2.attackBehaviour = new SwordAttack();
        weapon2.damage = 3;
        weapon2.range = 1.2f;
        weapon2.moveSpeed = 7;
        weapon2.sprite = testImage2;
        weapon2.controller = testController2;
        weapons.Add(weapon2);
        Debug.Log(weapon2.attackBehaviour);

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
    }

    public void ChangeWeapon(int index)
    {
        if(index != weaponIndex)
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
