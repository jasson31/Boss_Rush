using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public Sprite Heart0;
    public Sprite Heart1;
    public Sprite Heart2;
    public Sprite Heart3;
    public Sprite Heart4;
    
    public Image[] weaponImage;
    private WeaponBehaviour weapon;

    public GameObject panel;
    public Button yes, no;

    private Boss boss;
    private GameObject gameCursor;



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

        //int i = 0;
        //if (health % 1 != 0)
        //{
        //    for(i=0; i<Mathf.FloorToInt(health); i++)
        //    {
        //        hearts[i].sprite = Heart4;
        //        hearts[i].enabled = true;
        //    }
        //    if (health % 1 == 0.25f) hearts[i].sprite = Heart1;
        //    else if (health % 1 == 0.50f) hearts[i].sprite = Heart2;
        //    else if (health % 1 == 0.75f) hearts[i].sprite = Heart3;
        //}

        for (int i = 0; i < maxHealth; i++)
        {
            if (i < Mathf.FloorToInt(health))
            {
                hearts[i].sprite = Heart4;
            }
            else if(health - i == 0 || health - i < 0)
            {
                hearts[i].sprite = Heart0;
                
            }
            else
            {
                if (health % 1 == 0.25f) hearts[i].sprite = Heart1;
                else if (health % 1 == 0.50f) hearts[i].sprite = Heart2;
                else if (health % 1 == 0.75f) hearts[i].sprite = Heart3;
            }
            
        }

        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i].enabled = true;
        }
        for (int i = (int)maxHealth; i < hearts.Length; i++)
        {
            hearts[i].enabled = false;
        }

        //for(int i=0; i<maxHealth; i++)
        //{
        //    hearts[i].enabled = true;
        //}
        //for(int i=(int)maxHealth; i<hearts.Length; i++)
        //{
        //    hearts[i].enabled = false;
        //}
    }

    public void SetWeaponSprites()
    {
        int a = weapon.weaponIndex;
        int numWeapons = weapon.weapons.Count;
        if(numWeapons > 0)
        {
            weaponImage[0].sprite = weapon.weapons[weapon.weaponIndex].sprite;
        }
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


        if (weapon.weapons.Count > 0)
        {
            health = weapon.weapons[weapon.weaponIndex].health;
            maxHealth = weapon.weapons[weapon.weaponIndex].maxHealth;
        }

        SetHealthBar(health, maxHealth);

        if(!weapon.check)
        {
            gameCursor.SetActive(false);
            player.SetPlayerControllable(false);
            player.isInvincible = true;
            panel.SetActive(true);
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(true);
        }


    }

    public void continueButton()
    {
        gameCursor.SetActive(true);
        player.SetPlayerControllable(true);
        player.isInvincible = false;
        panel.SetActive(false);
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);

        health = weapon.weapons[weapon.weaponIndex].health;
        maxHealth = weapon.weapons[weapon.weaponIndex].maxHealth;
        SetHealthBar(health, maxHealth);

        weapon.check = true;
        Debug.Log("이어서 전투");
    }
    public void abandonButton()
    {
        Debug.Log("포기하기");
        SceneManager.LoadScene("SelectScreen");
    }
}
