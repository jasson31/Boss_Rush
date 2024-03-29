﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewCode
{
    MainScreen, DungeonSelect, Ingame
}

public class Game : SingletonBehaviour<Game>
{
    [SerializeField]
    private List<UIManager> view = new List<UIManager>();
    private int currentView;

    private UIManager CurrentView { get { return view[currentView]; } }

    public Player player;


    public void ChangeView(ViewCode viewName)
    {
        CurrentView.gameObject.SetActive(false);
        currentView = (int)viewName;
        CurrentView.gameObject.SetActive(true);
    }
}
