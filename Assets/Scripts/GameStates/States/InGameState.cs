using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GTA
{

    public class InGameState : Core.FSMBaseState<GStateManager.eState>
    {

        public InGameState(Core.FSMStateMachine<GStateManager.eState> machine) : base(machine) { }

        public override void OnEnter()
        {
            base.OnEnter();

        }

        public override void OnContext(System.Object context)
        {
            base.OnContext(context);

            collisionProcessor = new CollisionProcessor();

            GameObject voxelCharacter = GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("Characters/VoxelGirl/MainCharacter")) as GameObject;
            if (voxelCharacter == null) Core.QLogger.LogErrorAndThrowException("VoxelGirl is not instantiated");

            // Main.Instance.uIManager = uiManagerGo.GetComponent<UIManager>();
            // if ( Main.Instance.uIManager == null ) Core.QLogger.LogErrorAndThrowException ( "UiManager script was not instantiated");

            player = new iPlayer();
            CharacterController controller = new CharacterController();
            controller.Init(voxelCharacter, player);
            WeaponController weaponController = new WeaponController();
            player.Init(controller, weaponController);

            CollisionListener playerCollisionListener = voxelCharacter.GetComponentInChildren<CollisionListener>();
            Core.QLogger.Assert(playerCollisionListener != null);
            playerCollisionListener.Init(collisionProcessor.ProcessCollision, player);


            cameraScript = GameObject.Find("ThirdPersonCamera").GetComponent<ThirdPersonCamera>();
            cameraScript.SetCharacterToFollow(voxelCharacter.transform);


            player.EquipWeapon(eInventoryItem.Pistol);

            Core.Updater.Instance.FixedUpdater += FixedUpdate;
            Core.Updater.Instance.LateUpdater += LateUpdate;
        }

        public void FixedUpdate()
        {
            if (player != null)
            {
                player.FixedUpdate();
            }
        }

        public override void Update()
        {
            base.Update();
            if (player != null)
            {
                player.Update();
            }
        }

        public void LateUpdate()
        {
            if (player != null)
            {
                player.LateUpdate();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Core.Updater.Instance.FixedUpdater -= FixedUpdate;
            Core.Updater.Instance.LateUpdater -= LateUpdate;
        }

        iPlayer player = null;
        ThirdPersonCamera cameraScript = null;
        CollisionProcessor collisionProcessor = null;
    }
}