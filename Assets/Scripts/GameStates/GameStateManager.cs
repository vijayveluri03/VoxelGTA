using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;
using Core.FSMStateMachine;

public class GameStateManager 
{
    public enum GameStates
    {
        none = 0,
        Loading,
        Frontend,
        Game
    }

    // listeners
    public Action<GameStates> OnStateChanged;

    #region constructor and destructor.
        public GameStateManager()
        {
            gameStateMachine = new FSMStateMachine<GameStates>();
            gameStateMachine.RegisterState(GameStates.Loading, new LoadingState() );
            gameStateMachine.RegisterState(GameStates.Frontend, new FrontendState() );
            //gameStateMachine.RegisterState(CGameStates.Game, new GameplayState() );
			gameStateMachine.AddMapping ( GameStates.Loading, GameStates.Frontend );
			gameStateMachine.AddMapping ( GameStates.Frontend, GameStates.Game );
			gameStateMachine.AddMapping ( GameStates.Game, GameStates.Frontend );
        }
        ~GameStateManager()
        {
        }
    #endregion


    public void Update () 
    {
        gameStateMachine.Update();
    }

    public FSMState GetState( GameStates state )   {        return gameStateMachine.GetState(state);    }
    public void SetGameState(GameStates newGameState, System.Object context )
    {
        gameStateMachine.QueueState( newGameState, context );
    }

    // Private members 
    FSMStateMachine<GameStates> gameStateMachine;


}
