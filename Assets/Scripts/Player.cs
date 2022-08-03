using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{

    public class Player : iPlayer
    {


    }
    public class iPlayer : iListener
    {
        public WeaponController weaponController { get; private set; }
        public CharacterController playerController { get; private set; }

        public Inventory WeaponInventory { get; private set; }

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
        }

        public void LateUpdate()
        {
            if (playerController != null)
            {
                playerController.LateUpdate();
            }
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
    }

    public class iListener
    {

    }
}