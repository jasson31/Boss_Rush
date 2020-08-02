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
    private Slider healthBar;

    [SerializeField]
    private Slider bossHealthBar;

    public void SetBossHealthBar(int health, int maxHealth)
    {
        bossHealthBar.value = health / (float)maxHealth;
    }

    public void SetHealthBar(int health, int maxHealth)
    {
        healthBar.value = health / maxHealth;
    }

    public void OpenMenuUI()
    {

    }
}
