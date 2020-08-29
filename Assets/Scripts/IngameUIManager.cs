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
    [SerializeField]
    private int health;
    [SerializeField]
    private int maxHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    public Image[] weaponImage;
    public WeaponBehaviour weapon;

   


    [SerializeField]
    private Slider bossHealthBar;

    public void SetBossHealthBar(int health, int maxHealth)
    {
        bossHealthBar.value = health / (float)maxHealth;
    }

    public void SetHealthBar(int health, int maxHealth)
    {
        //healthBar.value = health / maxHealth;
    }

    public void SetWeaponSprites()
    {
        int numWeapons = weapon.weapons.Count;
        for (int i = 0; i < 3; i++)
        {
            if (i < numWeapons)
            {
                weaponImage[i].sprite = weapon.weapons[i].sprite;
            }
            else
            {
                weaponImage[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMenuUI()
    {
        
    }

    private void Start()
    {
        SetWeaponSprites();

        health = weapon.weapons[weapon.weaponIndex].health;
        maxHealth = weapon.weapons[weapon.weaponIndex].MaxHealth;

    }
    private void Update()
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

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
