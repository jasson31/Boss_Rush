using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponSpecsContainer
{
	public List<WeaponSpec> weapons;
}

public class WeaponFactory
{
	private static readonly float[] MoveSpeed = { 4, 5.5f, 7, 9.5f, 12 };

	private static Dictionary<int, WeaponSpec> weaponSpecs = null;
	public static Weapon CreateWeapon(int id)
	{
		if (weaponSpecs == null)
		{
			weaponSpecs = new Dictionary<int, WeaponSpec>();
			var json = Resources.Load<TextAsset>("Weapons");
			foreach(var newWeaponSpec in JsonParser<WeaponSpecsContainer>.FromJson(json.text).weapons)
			{
				weaponSpecs.Add(newWeaponSpec.id, newWeaponSpec);
			}
		}

		Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
		WeaponSpec weaponSpec = weaponSpecs[id];
		weapon.weaponID = weaponSpec.id;
		weapon.weaponName = weaponSpec.weaponName;
		weapon.health = weaponSpec.health;
		weapon.maxHealth = weaponSpec.health;
		weapon.damage = weaponSpec.damage;
		weapon.range = weaponSpec.range;
		weapon.moveSpeed = MoveSpeed[(int)weaponSpec.speed];

		switch (weaponSpec.attack)
		{
			case WeaponAttack.CUT:
				weapon.attackBehaviour = new SwordAttack();
				break;
			case WeaponAttack.STAB:
			case WeaponAttack.HIT:
				weapon.attackBehaviour = new StabAttack();
				break;
			case WeaponAttack.PROJECTILE:
				GameObject projectilePrefab = Resources.Load<GameObject>("Prefabs/" + weaponSpec.projectileName);
				weapon.attackBehaviour = new ShootAttack(projectilePrefab, weaponSpec.projectileSpeed);
				break;
			case WeaponAttack.LASER:
				GameObject laserPrefab = Resources.Load<GameObject>("Prefabs/" + weaponSpec.laserName);
				weapon.attackBehaviour = new LaserAttack(laserPrefab, weaponSpec.chargeTime, weaponSpec.shotTime);
				break;
		}

		weapon.controller = Resources.Load<AnimatorOverrideController>("Anims/" + weaponSpec.weaponName);
		weapon.sprite = Resources.Load<Sprite>("Sprites/" + weaponSpec.weaponName);

		return weapon;
	}
}
