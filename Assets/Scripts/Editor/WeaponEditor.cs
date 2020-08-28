using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct WeaponSpecsContainer
{
	public WeaponSpecsContainer(List<WeaponSpec> weapons)
	{
		this.weapons = weapons;
	}

	public List<WeaponSpec> weapons;
}

public class WeaponEditorWindow : EditorWindow
{
	static List<WeaponSpec> weapons = new List<WeaponSpec>();

	private int id;
	private string weaponName = "";
	private int damage;
	private float coolTime;
	private float range;
	private int health;
	private WeaponAttack attack;
	private WeaponMoveSpeed speed;
	
	//For Projectile
	private string projectileName;
	private float projectileSpeed;

	//For Laser
	private string laserName;
	private float chargeTime;
	private float shotTime;

	private Vector2 scrollPosition;

	[MenuItem("Editor/Weapon Editor")]
	public static void ShowWindow()
	{
		WeaponEditorWindow window = (WeaponEditorWindow)GetWindow(typeof(WeaponEditorWindow));

		List<Weapon> newWeapons = new List<Weapon>();
		var json = Resources.Load<TextAsset>("Weapons");
		Debug.Log(json);
		if (json != null)
		{
			weapons = JsonParser<WeaponSpecsContainer>.FromJson(json.text).weapons;
		}
		window.Show();
	}

	private void OnGUI()
	{
		BeginWindows();

		GUILayout.Label("Weapon List", EditorStyles.boldLabel);
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		for(int i = 0; i < weapons.Count; i++)
		{
			var weapon = weapons[i];
			EditorGUILayout.LabelField(weapons.IndexOf(weapon).ToString(), EditorStyles.boldLabel);
			weapon.id = EditorGUILayout.IntField("ID", weapon.id);
			weapon.weaponName = EditorGUILayout.TextField("Name", weapon.weaponName);
			weapon.damage = EditorGUILayout.IntField("Damage", weapon.damage);
			weapon.coolTime = EditorGUILayout.FloatField("Cooltime", weapon.coolTime);
			weapon.range = EditorGUILayout.FloatField("Range", weapon.range);
			weapon.health = EditorGUILayout.IntField("Health", weapon.health);
			weapon.attack = (WeaponAttack)EditorGUILayout.EnumPopup("Attack Type", weapon.attack);
			if (weapon.attack == WeaponAttack.PROJECTILE)
			{
				weapon.projectileName = EditorGUILayout.TextField("Projectile name", weapon.projectileName);
				weapon.projectileSpeed = EditorGUILayout.FloatField("Projectile speed", weapon.projectileSpeed);
			}
			else if (weapon.attack == WeaponAttack.LASER)
			{
				weapon.laserName = EditorGUILayout.TextField("Projectile name", weapon.laserName);
				weapon.chargeTime = EditorGUILayout.FloatField("Charge time", weapon.chargeTime);
				weapon.shotTime = EditorGUILayout.FloatField("Shot time", weapon.shotTime);
			}
			weapon.speed = (WeaponMoveSpeed)EditorGUILayout.EnumPopup("Move Speed", weapon.speed);
			if (GUILayout.Button("Remove"))
            {
				weapons.Remove(weapon);
            }
		}
		EditorGUILayout.EndScrollView();

		GUILayout.Label("New Weapon", EditorStyles.boldLabel);
		id = EditorGUILayout.IntField("ID", id);
		weaponName = EditorGUILayout.TextField("Name", weaponName);
		damage = EditorGUILayout.IntField("Damage", damage);
		coolTime = EditorGUILayout.FloatField("Cooltime", coolTime);
		range = EditorGUILayout.FloatField("Range", range);
		health = EditorGUILayout.IntField("Health", health);
		attack = (WeaponAttack)EditorGUILayout.EnumPopup("Attack Type", attack);
		if (attack == WeaponAttack.PROJECTILE)
		{
			projectileName = EditorGUILayout.TextField("Projectile name", projectileName);
			projectileSpeed = EditorGUILayout.FloatField("Projectile speed", projectileSpeed);
		}
		else if (attack == WeaponAttack.LASER)
		{
			laserName = EditorGUILayout.TextField("Projectile name", laserName);
			chargeTime = EditorGUILayout.FloatField("Charge time", chargeTime);
			shotTime = EditorGUILayout.FloatField("Shot time", shotTime);
		}
		speed = (WeaponMoveSpeed)EditorGUILayout.EnumPopup("Move Speed", speed);

		if (GUILayout.Button("Add new weapon"))
		{
			WeaponSpec weapon = new WeaponSpec();
			weapon.weaponName = weaponName;
			weapon.damage = damage;
			weapon.coolTime = coolTime;
			weapon.range = range;
			weapon.health = health;
			weapon.attack = attack;

			weapons.Add(weapon);
		}
		if (GUILayout.Button("Save current settings"))
		{
			WeaponSpecsContainer weaponList = new WeaponSpecsContainer(weapons);
			if (!System.IO.File.Exists(Application.dataPath + "/Resources/Weapons.json"))
				System.IO.File.Create(Application.dataPath + "/Resources/Weapons.json");
			System.IO.File.WriteAllText(Application.dataPath + "/Resources/Weapons.json", JsonParser<WeaponSpecsContainer>.ToJson(weaponList));
			Debug.Log(Application.dataPath + "/Resources/Weapons.json");
			Debug.Log(JsonParser<WeaponSpecsContainer>.ToJson(weaponList));
		}
		EndWindows();
	}

	void WeaponAdderWindow(int id)
	{
		GUI.DragWindow();
	}

	void WeaponEditWindow(int id)
	{

	}
}
