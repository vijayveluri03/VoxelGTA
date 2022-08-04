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

            switch (step)
            {

                case 0:
                    {
                        if (sharedObjects.TryFetch(Constants.SOKeys.UIManager) == null)
                        {
                            CreateUIManager();
                            return;
                        }
                        step++;
                    }
                    break;
                case 1:
                    {
                        if (sharedObjects.TryFetch(Constants.SOKeys.CameraSystem) == null)
                        {
                            sharedObjects.Add(Constants.SOKeys.CameraSystem, new Core.CameraSystem<eCameraType>());
                            return;
                        }
                        step++;
                    }
                    break;
                case 2:
                    {
                        var sceneInput = RegisterInputs.GetInputsFromScene();
                        sceneInput.RegisterCameraInputs(sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem));
                        sceneInput.Destroy(true);

                        sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem).SwitchCamera(eCameraType.MAIN_CAMERA);

                        step++;
                    }
                    break;
                case 3:
                    {
                        LoadLobbyAndGoToNextState();
                        step++;
                        break;
                    }
                case 4:

                    if ( isLevelLoaded )
                    {
                        step++;
                    }
                    break;
                case 5:
                    {
                        var sceneInput = RegisterInputs.GetInputsFromScene();
                        sceneInput.RegisterCameraInputs(sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem));
                        sceneInput.Destroy(true);
                        step++;
                        break;
                    }

                case 6:
                    {
#if DEBUG
                        statemachine.PushNextState(GStateManager.eState.Game, sharedObjects);
#else
                        statemachine.PushNextState(GStateManager.eState.Frontend, sharedObjects);
#endif
                        step++;
                        break;
                    }
                case 7:
                    Core.QLogger.Assert(false);
                    break;

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
                    isLevelLoaded = true;
                }, true
                );
        }

        private Core.SharedObjects<System.Object> sharedObjects = null;
        private bool isLevelLoaded = false;
        private int step = 0;
        
    }
}
