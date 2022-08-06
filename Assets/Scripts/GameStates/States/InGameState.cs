﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{

    public class InGameState : Core.FSMBaseState<GStateManager.eState>
    {

        public InGameState(Core.FSMStateMachine<GStateManager.eState> machine) : base(machine) { }

        public override void OnEnter(System.Object context)
        {
            base.OnEnter(context);
            ExtractContext(context);

            BuildPlayer();
            SwitchCameraToThirdPersonView();

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


        #region PRIVATE 

        private void BuildPlayer()
        {
            collisionProcessor = new CollisionProcessor();
            Core.CollisionDispatcher.Instance.CollsionHandlers += collisionProcessor.ProcessCollision;

            GameObject charaterModelGO = GameObject.Instantiate(Core.ResourceManager.Instance.LoadAsset<UnityEngine.Object>("Characters/VoxelGirl/MainCharacter")) as GameObject;
            if (charaterModelGO == null) Core.QLogger.LogErrorAndThrowException("VoxelGirl is not instantiated");
           
            
            CharacterController controller = new CharacterController();
            controller.Init(charaterModelGO);
            WeaponController weaponController = new WeaponController();

            player = new Player();
            player.Init(controller, weaponController);

            //assign player collsion context
            var CollidableComponent = charaterModelGO.GetComponent<Core.Collidable>();
            Core.QLogger.Assert(CollidableComponent != null, "Collidable componet not present on" + charaterModelGO.name);
            if (CollidableComponent)
            {
                (CollidableComponent.CollisionContext as PlayerCollisionContext).Player = player;
            }
            


            cameraMonoScript = GameObject.Find("ThirdPersonCamera").GetComponent<ThirdPersonCamera>();
            cameraMonoScript.SetCharacterToFollow(charaterModelGO.transform);   // todo: Instead of directly providing the transform, provide an interface through which the trasform could be fetched. The player can extend the interface to provide the data

            //player.EquipWeapon(eInventoryItem.Pistol);
        }

        private void ExtractContext(object context)
        {
            Core.QLogger.Assert(context != null && context is Core.SharedObjects<System.Object>);
            sharedObjects = context as Core.SharedObjects<System.Object>;
        }

        private void SwitchCameraToThirdPersonView()
        {
            sharedObjects.TryFetch<Core.CameraSystem<eCameraType>>(Constants.SOKeys.CameraSystem).SwitchCamera(eCameraType.PLAYER_CAMERA);
        }

        iPlayer player = null;
        ThirdPersonCamera cameraMonoScript = null;
        CollisionProcessor collisionProcessor = null;
        private Core.SharedObjects<System.Object> sharedObjects = null;

        #endregion
    }
}