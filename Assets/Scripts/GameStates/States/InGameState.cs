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

            GameObject charaterModelGO = GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("Characters/VoxelGirl/MainCharacter")) as GameObject;
            if (charaterModelGO == null) Core.QLogger.LogErrorAndThrowException("VoxelGirl is not instantiated");

            CharacterController controller = new CharacterController();
            controller.Init(charaterModelGO);
            WeaponController weaponController = new WeaponController();

            player = new Player();
            player.Init(controller, weaponController);

            CollisionListener playerCollisionListener = charaterModelGO.GetComponentInChildren<CollisionListener>();
            Core.QLogger.Assert(playerCollisionListener != null);
            playerCollisionListener.Init(collisionProcessor.ProcessCollision, player);

            cameraMonoScript = GameObject.Find("ThirdPersonCamera").GetComponent<ThirdPersonCamera>();
            cameraMonoScript.SetCharacterToFollow(charaterModelGO.transform);   // todo: Instead of directly providing the transform, provide an interface through which the trasform could be fetched. The player can extend the interface to provide the data

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
        ThirdPersonCamera cameraMonoScript = null;
        CollisionProcessor collisionProcessor = null;
    }
}