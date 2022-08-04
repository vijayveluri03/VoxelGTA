using System;
using System.Collections;
using System.Collections.Generic;



namespace GTA
{
    public class GStateManager
    {
        public enum eState
        {
            none = 0,
            Loading,
            Frontend,
            Game
        }

        // listeners
        public Action<eState> OnStateChanged;

        public GStateManager(Core.SharedObjects<System.Object> sharedObjects)
        {
            gameStateMachine = new Core.FSMStateMachine<eState>();
            this.sharedObjects = sharedObjects;
            RegisterStates();
            RegisterMappings();
        }

        ~GStateManager()
        {
        }

        public void Update()
        {
            gameStateMachine.Update();
        }

        public Core.FSMState<eState> GetState(eState state)
        {
            return gameStateMachine.GetState(state);
        }

        public void PushNextState(eState newGameState)
        {
            gameStateMachine.PushNextState(newGameState, sharedObjects);
        }

        #region PRIVATE
        private void RegisterStates()
        {
            gameStateMachine.RegisterState(eState.Loading, new LoadingState(gameStateMachine));
            gameStateMachine.RegisterState(eState.Game, new InGameState(gameStateMachine));
        }
        private void RegisterMappings()
        {
#if DEBUG
            gameStateMachine.AddMapping(eState.Loading, eState.Game);
#else
            gameStateMachine.AddMapping(eState.Loading, eState.Frontend);
            gameStateMachine.AddMapping(eState.Frontend, eState.Game);
            gameStateMachine.AddMapping(eState.Game, eState.Frontend);
#endif
        }

        // Private members 
        Core.FSMStateMachine<eState> gameStateMachine;
        Core.SharedObjects<System.Object> sharedObjects = null;
#endregion
    }
}