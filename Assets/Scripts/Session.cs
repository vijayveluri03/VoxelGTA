using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public class Session
    {
        public Player player { get; private set; }

        public Session(Core.SharedObjects<System.Object> sharedObjects)
        {
            this.sharedObjects = sharedObjects;

            inputSystem = sharedObjects.Fetch<Core.UnityInputSystem<eInputAction>>(Constants.SOKeys.InputSystem);
            BuildPlayer();
            BuildPlayerCamera();
            BuildWeaponCameraMediatorAndAttachIt();
            SwitchCameraToThirdPersonView();
            ListenToUserActions();

            // CHEATS
            player.OnWeaponCollected(eInventoryItem.Pistol);
        }

        ~Session()
        {
            if (collisionProcessor != null)
            {
                Core.CollisionDispatcher.Instance.UnRegister(collisionProcessor.ProcessCollision);
                collisionProcessor = null;
            }
        }

        public void FixedUpdate()
        {
            if (player != null)
            {
                player.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            if (player != null)
            {
                player.LateUpdate();
            }
        }

        public void Update()
        {
            if (player != null)
            {
                player.Update();
            }
        }

        #region PRIVATE 

        private void BuildPlayer()
        {
            collisionProcessor = new CollisionProcessor();
            collisionProcessor.Init(this);
            Core.CollisionDispatcher.Instance.Register(collisionProcessor.ProcessCollision);

            GameObject charaterModelGO = GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("Characters/VoxelGirl/MainCharacter")) as GameObject;
            Core.QLogger.Assert(charaterModelGO != null, "VoxelGirl is not instantiated");

            CharacterController controller = new CharacterController();
            controller.Init(charaterModelGO, inputSystem);

            player = new Player();
            player.Init(inputSystem, controller);

            //assign player collsion context
            var CollidableComponent = charaterModelGO.GetComponent<Core.Collidable>();
            Core.QLogger.Assert(CollidableComponent != null, "Collidable componet not present on" + charaterModelGO.name);
        }

        // Dependency - Player has to be build first 
        private void BuildPlayerCamera()
        {
            {
                GameObject thirdPersonCamera = GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("Prefabs/ThirdPersonCamera")) as GameObject;
                Camera camera = thirdPersonCamera.GetComponent<Camera>();
                sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem).AddCamera(eCameraType.PLAYER_CAMERA, camera);

                cameraScript = thirdPersonCamera.GetComponent<ThirdPersonCamera>();
                cameraScript.Init(player.GetSharedModel_ReadOnly());
                
                cameraScript.SnapToMode(ThirdPersonCamera.eMode.THIRD_PERSON_CENTER);
            }
        }

        private void BuildWeaponCameraMediatorAndAttachIt()
        {
            var mediator = new WeaponCameraMediator(cameraScript);
            player.PostInit(mediator);
        }

        // Dependency - Camera has to be set up first 
        private void SwitchCameraToThirdPersonView()
        {
            sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem).SwitchCamera(eCameraType.PLAYER_CAMERA);
        }

        private void ListenToUserActions()
        {
            inputSystem.RegisterPressEvent(eInputAction.SWITCH_PLAYER_CAMERA, OnCameraSwitchAction);
#if DEBUG
            inputSystem.RegisterPressEvent(eInputAction.TOGGLE_DEBUG_PANEL, OnDebugConsoleAction);
#endif
        }

        private void OnCameraSwitchAction()
        {
            Core.QLogger.Assert(cameraScript != null);

            ThirdPersonCamera.eMode nextMode =  ThirdPersonCamera.eMode.UNDEFINED;
            ThirdPersonCamera.eMode currentMode = cameraScript.IsTransisioning ? cameraScript.NextMode : cameraScript.Mode;

            switch (currentMode)
            {
                case ThirdPersonCamera.eMode.UNDEFINED:
                    nextMode = ThirdPersonCamera.eMode.THIRD_PERSON_CENTER;
                    break;
                case ThirdPersonCamera.eMode.THIRD_PERSON_CENTER:
                    nextMode = ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_LEFT;
                    break;
                case ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_LEFT:
                    nextMode = ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_RIGHT;
                    break;
                case ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_RIGHT:
                    nextMode = ThirdPersonCamera.eMode.FIRST_PERSON;
                    break;
                case ThirdPersonCamera.eMode.FIRST_PERSON:
                    nextMode = ThirdPersonCamera.eMode.THIRD_PERSON_CENTER;
                    break;
            }
            cameraScript.TransitionToMode(nextMode);

            // Adjusting the weapon slot accordingly
            switch(nextMode)
            {
                case ThirdPersonCamera.eMode.THIRD_PERSON_CENTER:
                    player.TrySwitchPrimaryWeaponHand(Player.eWeaponSlot.RIGHT);
                    break;
                case ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_LEFT:
                    player.TrySwitchPrimaryWeaponHand(Player.eWeaponSlot.RIGHT);
                    break;
                case ThirdPersonCamera.eMode.THIRD_PERSON_OFFSET_RIGHT:
                    player.TrySwitchPrimaryWeaponHand(Player.eWeaponSlot.LEFT);
                    break;
                case ThirdPersonCamera.eMode.FIRST_PERSON:
                    player.TrySwitchPrimaryWeaponHand(Player.eWeaponSlot.RIGHT);
                    break;
                default:
                    Core.QLogger.Assert(false);
                    break;
            }
        }

#if DEBUG
        private void OnDebugConsoleAction()
        {
            Core.QLogger.ShowOrHideGUI();
        }
#endif

        CollisionProcessor collisionProcessor = null;
        private Core.SharedObjects<System.Object> sharedObjects = null;
        private Core.UnityInputSystem<GTA.eInputAction> inputSystem = null;

        ThirdPersonCamera cameraScript = null;

        #endregion
    }
}