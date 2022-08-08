using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    // @todo - is there a better way to do this ?
    public class PlayerAndCameraSharedModel
    {
        public Transform cameraTarget;
        public Quaternion verticalDelta;
    };
    public class Player
    {
        public void Init(CharacterController controller, WeaponController weaponController)
        {
            WeaponInventory = new Inventory();
            this.playerController = controller;
            this.weaponController = weaponController;
        }

        public void FixedUpdate()
        {
            if (playerController != null)
            {
                playerController.FixedUpdate();
            }
        }

        public void Update()
        {
            if (playerController != null)
            {
                playerController.Update();
            }
            if (weaponController != null)
            {
                weaponController.Update();
            }
            UpdateSharedData();
        }

        public void LateUpdate()
        {
            if (playerController != null)
            {
                playerController.LateUpdate();
            }
        }

        public void UpdateSharedData()
        {
            sharedData.cameraTarget = GetTransformForCameraToFollow();
            sharedData.verticalDelta = playerController.CommonState.orientation.VerticalOrientationDelta;
        }

        public bool IsWeaponEquipped()
        {
            return false;
        }

        public void EquipWeapon(eInventoryItem item)
        {
            GameObject gun = ItemFactory.Spawn(item);
            gun.transform.parent = playerController.Inputs.weaponRoot;
            gun.transform.localPosition = Vector3.zero;
            gun.transform.localRotation = Quaternion.identity;
            weaponController.EquipWeapon(gun);
        }

        public void UnEquipWeapon()
        {

        }

        public PlayerAndCameraSharedModel GetSharedModel_ReadOnly()
        {
            return sharedData;
        }

        public Transform GetTransformForCameraToFollow()
        {
            Core.QLogger.Assert(playerController != null && playerController.GameObject != null);
            return playerController.GameObject.transform;
        }

        public WeaponController weaponController { get; private set; }
        public CharacterController playerController { get; private set; }
        public Inventory WeaponInventory { get; private set; }

        private PlayerAndCameraSharedModel sharedData = new PlayerAndCameraSharedModel();
    }

}