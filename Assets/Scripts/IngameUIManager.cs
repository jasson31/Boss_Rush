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
    private int numHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

   


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

    public void OpenMenuUI()
    {
        
    }

    private void Update()
    {

        if (health > numHearts)
        {
            health = numHearts;
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

            if (i < numHearts)
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
