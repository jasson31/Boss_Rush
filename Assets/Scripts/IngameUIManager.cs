using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class UIManager : SingletonBehaviour<UIManager>
{
    private void OnDisable()
    {
        OnUnmount();
    }

    private void OnEnable()
    {
        OnMount();
    }

    protected abstract void OnMount();
    protected abstract void OnUnmount();
}

public class IngameUIManager : SingletonBehaviour<IngameUIManager>
{
    private float health;
    private float maxHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    public Image[] weaponImage;
    private WeaponBehaviour weapon;

    public GameObject panel;
    public Button yes, no;

    private Boss boss;
    public GameObject gameCursor;



    [SerializeField]
    private Slider bossHealthBar;

    private Player player;

    public void SetBossHealthBar(float health, float maxHealth)
    {
        bossHealthBar.value = health / (float)maxHealth;
    }

    public void SetHealthBar(float health, float maxHealth)
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < health)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void SetWeaponSprites()
    {
        int a = weapon.weaponIndex;
        int numWeapons = weapon.weapons.Count;
        weaponImage[0].sprite = weapon.weapons[weapon.weaponIndex].sprite;
        int index = 1;
        if (numWeapons < 3)
        {
            for(int i = numWeapons; i < 3; i++)
            {
                weaponImage[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < numWeapons; i++)
        {
            if (i == weapon.weaponIndex) continue;
            weaponImage[index++].sprite = weapon.weapons[i].sprite;
        }
 
        
    }

    public void OpenMenuUI()
    {
        
    }

    private void Start()
    {
        gameCursor = GameObject.Find("IngameCursor");
        boss = FindObjectOfType<Boss>();
        weapon = GameObject.Find("Weapon").GetComponent<WeaponBehaviour>();
        player = FindObjectOfType<Player>();

        SetWeaponSprites();

        health = weapon.weapons[weapon.weaponIndex].health;
        maxHealth = weapon.weapons[weapon.weaponIndex].maxHealth;

        SetHealthBar(health, maxHealth);

    }
    private void Update()
    {

        SetWeaponSprites();

        health = weapon.weapons[weapon.weaponIndex].health;
        maxHealth = weapon.weapons[weapon.weaponIndex].maxHealth;

        SetHealthBar(health, maxHealth);

        if(!weapon.check)
        {
            gameCursor.SetActive(false);
            boss.gameObject.SetActive(false);
            player.SetPlayerControllable(false);
            panel.SetActive(true);
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(true);
        }


    }

    public void continueButton()
    {
        Debug.Log("이어서 전투");
    }
    public void abandonButton()
    {
        Debug.Log("포기하기");
    }
}
