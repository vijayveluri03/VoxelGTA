using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{


    public class Weapon
    {
        public enum eStates
        {
            Idle = 0,
            Shoot,
            Reload,
            Recoil
        }


        public GameObject GameObject { get; private set; }
        public Animator Animator { get; private set; }
        public WeaponInputs Inputs { get; private set; }
        public WeaponCommonState CommonState { get; private set; }
        public Core.UnityInputSystem<eInputAction> InputSystem { get; private set; }

        public Weapon(Core.UnityInputSystem<eInputAction> inputSystem)
        {
            InputSystem = inputSystem;
        }

        public void Initialize(GameObject WeaponObject)
        {
            GameObject = WeaponObject;
            Animator = GameObject.GetComponent<Animator>();
            Inputs = GameObject.GetComponent<WeaponInputs>();
            CommonState = new WeaponCommonState(this);

            controller = new Core.FSMController<Weapon, eStates>(this);
            controller.RegisterState(eStates.Idle, new WeaponIdle());
            controller.RegisterState(eStates.Shoot, new WeaponShoot());
            controller.RegisterState(eStates.Reload, new WeaponReload());
            controller.RegisterState(eStates.Recoil, new WeaponRecoil());

            controller.AddMapping(eStates.Idle, eStates.Shoot, eStates.Reload);
            controller.AddMapping(eStates.Shoot, eStates.Recoil);
            controller.AddMapping(eStates.Reload, eStates.Idle, eStates.Shoot);
            controller.AddMapping(eStates.Recoil, eStates.Idle, eStates.Shoot, eStates.Reload);

            controller.SetLogToGUI(true, 2);

            controller.SetState(eStates.Idle);
            CommonState.Init();

            // sanity checks
            Core.QLogger.Assert(Inputs.magCapacity > 0);
            Core.QLogger.Assert(Inputs.reloadTime > 0 && Inputs.recoilTime > 0 && Inputs.accuracy > 0);
            // if ( Inputs.shotType == eShotType.BURST )
            //     Core.QLogger.Assert ( Inputs.burstBulletCount > 1 );
            Core.QLogger.Assert(Inputs.damage > 0 && Inputs.range > 0);
            Core.QLogger.Assert(Inputs.muzzlePositionAndDirection != null);
        }

        public void UnInitialize()
        {
            // do something
        }

        public void Update()
        {
            if (controller != null)
                controller.Update();
        }

        public void Notify(params object[] arguments)
        {
            if (controller != null)
                controller.Notify(arguments);
        }

        private Core.FSMController<Weapon, eStates> controller = null;

    }
}