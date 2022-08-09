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

    // todo - Move weapon in hand management to a different class. Single reponsibility principle. 
    // because weapon variables have too many states and its polluting this class 
    public class Player
    {
        public enum eWeaponSlot
        {
            UNDEFINED,
            LEFT,
            RIGHT
        };

        public Weapon PrimaryWeapon
        {
            get
            {
                if (weaponsInHand.ContainsKey(primaryWeaponSlot))
                    return weaponsInHand[primaryWeaponSlot];
                return null;
            }
        }

        public void Init( Core.UnityInputSystem<eInputAction> inputSystem, CharacterController controller)
        {
            WeaponInventory = new Inventory();
            this.inputSystem = inputSystem;
            this.playerController = controller;
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
            if (PrimaryWeapon != null)
            {
                PrimaryWeapon.Update();
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


        public PlayerAndCameraSharedModel GetSharedModel_ReadOnly()
        {
            return sharedData;
        }

        public Transform GetTransformForCameraToFollow()
        {
            Core.QLogger.Assert(playerController != null && playerController.GameObject != null);
            return playerController.GameObject.transform;
        }


        #region WEAPON PUBLIC APIS
        public void OnWeaponCollected(eInventoryItem weapon)
        {
            // todo - this is temp logic which is swap the weapon everytime a new weapn is collected. 
            eWeaponSlot cachedSlot = GetPrimaryWeaponSlot();
            if(cachedSlot == eWeaponSlot.UNDEFINED)
                cachedSlot = eWeaponSlot.LEFT;  // todo - this is a bad assumption

            if (PrimaryWeapon != null)
                UnEquipAndDestroyPrimaryWeapon();

            ConstructAndEquipPrimaryWeapon(cachedSlot, weapon);
        }

        // Order of Sets to follow
        // 1. Create weapon 
        // 2. Attach it to player. 
        // 3. Set the slot attached as the primary weapon

        public void ConstructAndEquipPrimaryWeapon( eWeaponSlot slot, eInventoryItem item)
        {
            Core.QLogger.Assert(GetWeaponInSlot(slot) == null);

            // #1
            Weapon weapon = new Weapon(inputSystem);
            GameObject gun = PropFactory.Spawn(item);
            weapon.Initialize(gun);

            //#2
            AttachWeaponInSlot(weapon, slot);

            //#3
            SetPrimaryWeaponSlot(slot);
        }

        // Order of Sets to follow
        // 1. detach weapon - This will make the weapon disconnected from player. doesnt destroy it 
        // 2. reset player's weapon in hand cached params 
        // 3. destroy weapon ( which is already detached ) 

        public void UnEquipAndDestroyPrimaryWeapon()
        {
            if (PrimaryWeapon == null)
                return;

            Weapon weapon = PrimaryWeapon;

            //#1
            DetachWeaponFromSlot(primaryWeaponSlot);    // this will make PrimaryWeapon Null, as the player is not equipped with it. 

            //#2
            UnSetPrimaryWeaponSlot();

            //#3
            weapon.UnInitialize();
            weapon = null;                              // not really necessary 
        }

        public bool HasPrimaryWeapon()
        {
            return PrimaryWeapon != null;
        }

        public void TrySwitchPrimaryWeaponHand(eWeaponSlot newSlot)
        {
            if (primaryWeaponSlot != newSlot && PrimaryWeapon != null)
                SwitchPrimaryWeaponHand(newSlot);
        }

        // Steps in order 
        // #1 Switch weapon from primary weapon slot to new slot
        // #2 switch the primary weapon slot to the new one
        public void SwitchPrimaryWeaponHand(eWeaponSlot newSlot)
        {
            Core.QLogger.Assert(GetWeaponInSlot(newSlot) == null);
            Core.QLogger.Assert(primaryWeaponSlot != newSlot);
            Core.QLogger.Assert(PrimaryWeapon != null);

            //#1
            Weapon weapon = PrimaryWeapon;
            DetachWeaponFromSlot(primaryWeaponSlot);
            AttachWeaponInSlot(weapon, newSlot);

            //#2
            UnSetPrimaryWeaponSlot();
            SetPrimaryWeaponSlot(newSlot);
        }

        public eWeaponSlot GetPrimaryWeaponSlot()
        {
            return primaryWeaponSlot;
        }

        #endregion

        #region WEAPONS private APIs
        // Just attach to slot. Doesnt create it
        private void AttachWeaponInSlot( Weapon weapon, eWeaponSlot slot)
        {
            // The slot shouldnt have a weapon loaded already
            Core.QLogger.Assert(GetWeaponInSlot(slot) == null);

            Transform handTransform = null;
            if (slot == eWeaponSlot.LEFT)
                handTransform  = playerController.Inputs.leftHandWeaponSlot;
            else
                handTransform=  playerController.Inputs.rightHandWeaponSlot;

            Core.QLogger.Assert(handTransform != null);
            Core.QLogger.Assert(weapon != null && weapon.GameObject != null, "Weapon is not initialized" );

            weapon.GameObject.transform.parent = handTransform;
            weapon.GameObject.transform.localPosition = Vector3.zero;
            weapon.GameObject.transform.localRotation = Quaternion.identity;

            weaponsInHand[slot] = weapon;
        }

        // Just detach weapon. Doesnt destroy it 
        private void DetachWeaponFromSlot(eWeaponSlot slot)
        {
            Core.QLogger.Assert(GetWeaponInSlot(slot) != null);

            var weapon = weaponsInHand[slot];
            weapon.GameObject.transform.parent = null;

            weaponsInHand[slot] = null;
        }

        private void SetPrimaryWeaponSlot(eWeaponSlot slot)
        {
            primaryWeaponSlot = slot;
        }
        private void UnSetPrimaryWeaponSlot()
        {
            primaryWeaponSlot =  eWeaponSlot.UNDEFINED;
        }

        private Weapon GetWeaponInSlot( eWeaponSlot slot )
        {
            if (weaponsInHand.ContainsKey(slot))
                return weaponsInHand[slot];
            return null;
        }

        #endregion

        public CharacterController playerController { get; private set; }
        public Inventory WeaponInventory { get; private set; }

        private PlayerAndCameraSharedModel sharedData = new PlayerAndCameraSharedModel();
        private Core.UnityInputSystem<eInputAction> inputSystem = null;

        // WEAPONS
        private Dictionary<eWeaponSlot, Weapon> weaponsInHand = new Dictionary<eWeaponSlot, Weapon>();
        private eWeaponSlot primaryWeaponSlot = eWeaponSlot.UNDEFINED;  // This is slightly redundant state. This has to be in sync with weaponsinHand data. So be extra careful while maintaining states
    }

}