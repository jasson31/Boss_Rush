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

public class IngameUIManager : UIManager
{
    [SerializeField]
    private Slider healthBar;

    public void SetHealthBar(int health, int maxHealth)
    {
        healthBar.value = health / maxHealth;
    }

    public void OpenMenuUI()
    {

    }

    protected override void OnMount()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnUnmount()
    {
        throw new System.NotImplementedException();
    }
}
