using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;
 
public class Main : SingletonMonoBehaviour<Main>
{
    [HideInInspector] public GameStateManager gameStateManager;
    [HideInInspector] public UIManager   uIManager;

    // Use this for initialization
    void Start()
    {
        QLogger.SetLoggingLevel( QLogger.Level.Info );

        gameStateManager = new GameStateManager();
        gameStateManager.SetGameState(GameStateManager.GameStates.Loading, null );
    }

    public void Update ()
    {
        //@todo find a better place for cheats 
        // @todo hard coded controls
        if ( Input.GetKeyDown(KeyCode.C))
            QLogger.ShowOrHideGUI();

        if ( gameStateManager != null ) gameStateManager.Update();
        if ( uIManager != null ) uIManager.DoUpdate ();
    }

    protected override bool DestroyOnSceneSwitch   {  get { return true; }   }
}
