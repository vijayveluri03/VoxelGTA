using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;
using Core.FSMStateMachine;

public class LoadingState : FSMBaseState
{
    
    public override void Update()
    {
        if ( Main.Instance.uIManager == null )
        {
            GameObject uiManagerGo = GameObject.Instantiate ( ResourceManager.Instance.LoadAsset<UnityEngine.Object> ( "UI/UiManager") ) as GameObject;
            if ( uiManagerGo == null ) QLogger.LogErrorAndThrowException ( "UiManager was not instantiated");

            Main.Instance.uIManager = uiManagerGo.GetComponent<UIManager>();
            if ( Main.Instance.uIManager == null ) QLogger.LogErrorAndThrowException ( "UiManager script was not instantiated");

            if ( QLogger.CanLogInfo ) 
            
            QLogger.LogInfo("UI manager was instantiated");

			ResourceManager.Instance.LoadLevel("Lobby", true,
			delegate ()
			{
				Main.Instance.gameStateManager.SetGameState(GameStateManager.GameStates.Frontend, null);
			}, true
			);
        }
    }

    #region FSMState
    public override void OnEnter() { base.OnEnter(); }
    public override void OnExit() { base.OnExit(); }
    #endregion

    
}
