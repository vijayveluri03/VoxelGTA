using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    public class LoadingState : Core.FSMBaseState<GStateManager.eState>
    {
        public LoadingState(Core.FSMStateMachine<GStateManager.eState> machine) : base(machine) { }

        public override void Update()
        {
            if (sharedObjects == null)
                return;

            if (sharedObjects.TryFetch( Constants.SOKeys.UIManager) == null)
            {
                CreateUIManager();
                return;
            }

            if ( !isLevelBeingLoaded )
            {
                isLevelBeingLoaded = true;
                LoadLobbyAndGoToNextState();
            }
        }

        public override void OnEnter() { base.OnEnter(); }
        public override void OnExit() { base.OnExit(); }

        public override void OnContext(object context)
        {
            base.OnContext(context);
            Core.QLogger.Assert(context != null && context is Core.SharedObjects<System.Object>);
            sharedObjects = context as Core.SharedObjects<System.Object>;
        }

        private void CreateUIManager()
        {
            GameObject uiManagerGo =
                    GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("UI/UiManager"))
                    as GameObject; // @todo - this convertion will fail if loading fails. 

            Core.QLogger.Assert(uiManagerGo != null);

            sharedObjects.Add(Constants.SOKeys.UIManager, uiManagerGo.GetComponent<Core.UIManager>());

            Core.QLogger.LogInfo("UI manager was instantiated");
        }
        private void LoadLobbyAndGoToNextState()
        {
            Core.ResourceManager.Instance.LoadLevel("Lobby", true,
                delegate ()
                {
#if DEBUG
                    statemachine.PushNextState(GStateManager.eState.Game, null);
#else
                    statemachine.PushNextState(GStateManager.eState.Frontend, null);
#endif
                }, true
                );
        }

        private Core.SharedObjects<System.Object> sharedObjects = null;
        private bool isLevelBeingLoaded = false;
        
    }
}
