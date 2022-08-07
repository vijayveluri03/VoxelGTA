using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GTA
{
    // STATE TO LOAD ALL THE STUFF NEEDED FOR THE GAME 

    public class LoadingState : Core.FSMBaseState<GStateManager.eState>
    {
        public LoadingState(Core.FSMStateMachine<GStateManager.eState> machine) : base(machine) { }

        public override void Update()
        {
            if (sharedObjects == null)
                return;

            switch (step)
            {
                // SET UP UI MANAGER WHICH CAN CONTROL UI PANELS 
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
                    // SET UP CAMERA SYSTEM ( WHICH CAN HANDLE MULTIPLE CAMERAS )
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
                    // SET UP MAIN CAMERA 
                case 2:
                    {
                        var sceneInput = RegisterInputs.GetInputsFromScene();
                        sceneInput.RegisterCameraInputs(sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem));
                        sceneInput.Destroy(true);

                        sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem).SwitchCamera(eCameraType.MAIN_CAMERA);

                        step++;
                    }
                    break;
                    // LOAD LEVEL 
                case 3:
                    {
                        LoadLobbyAndGoToNextState();
                        step++;
                        break;
                    }
                    // WAIT FOR THE LEVEL TO BE LOADED
                case 4:

                    if ( isLevelLoaded )
                    {
                        step++;
                    }
                    break;
                    // SET UP SCENE CAMERA 
                case 5:
                    {
                        var sceneInput = RegisterInputs.GetInputsFromScene();
                        sceneInput.RegisterCameraInputs(sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem));
                        sceneInput.Destroy(true);
                        step++;
                        break;
                    }
                    // SET UP INPUTS - ACTION TO KEY MAPPING 
                case 6:
                    {
                        Core.UnityInputSystem<GTA.eInputAction> inputSystem = new Core.UnityInputSystem<eInputAction>();
                        inputSystem.Init(FetchKeyMapping());
                        sharedObjects.Add(Constants.SOKeys.InputSystem, inputSystem);
                        step++;
                    }
                    break;
                    // ALL THE LOADING IS DONE - CHANGE GAME STATE 
                case 7:
                    {
#if DEBUG
                        statemachine.PushNextState(GStateManager.eState.Game, sharedObjects);
#else
                        statemachine.PushNextState(GStateManager.eState.Frontend, sharedObjects);
#endif
                        step++;
                        break;
                    }
                case 8:
                    Core.QLogger.Assert(false);
                    break;

            }
        }


        public override void OnEnter(object context)
        {
            base.OnEnter(context);
            ExtractContext(context);
        }

        public override void OnExit()
        {
            base.OnExit();
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

        private void ExtractContext(object context)
        {
            Core.QLogger.Assert(context != null && context is Core.SharedObjects<System.Object>);
            sharedObjects = context as Core.SharedObjects<System.Object>;
        }

        private Dictionary<eInputAction, List<KeyCode>> FetchKeyMapping()
        {
            // Hacked to directly add inputs here. Later, these have to be setup based on the inputs from the user in options.
            Dictionary<eInputAction, List<KeyCode>> actionKeycodeMapping = new Dictionary<eInputAction, List<KeyCode>>();

            actionKeycodeMapping[eInputAction.MOVE_FORWARD] = new List<KeyCode>{ KeyCode.W, KeyCode.UpArrow };
            actionKeycodeMapping[eInputAction.MOVE_BACKWARD] = new List<KeyCode> { KeyCode.D, KeyCode.DownArrow };

            actionKeycodeMapping[eInputAction.STRAFE_LEFT] = new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow };
            actionKeycodeMapping[eInputAction.STRAFE_RIGHT] = new List<KeyCode> { KeyCode.D, KeyCode.RightArrow };

            actionKeycodeMapping[eInputAction.SWITCH_PLAYER_CAMERA] = new List<KeyCode> { KeyCode.Tab };

            actionKeycodeMapping[eInputAction.ACTION_1] = new List<KeyCode> { KeyCode.E, KeyCode.Mouse0 };
            actionKeycodeMapping[eInputAction.ACTION_2] = new List<KeyCode> { KeyCode.Mouse1 };

            return actionKeycodeMapping;
        }

        private Core.SharedObjects<System.Object> sharedObjects = null;
        private bool isLevelLoaded = false;
        private int step = 0;
        
    }
}
