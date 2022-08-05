using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{

    public class CharacterController
    {
        public enum eStates
        {
            Idle,
            Walk,
            Jump,
            Falling
        }
        public GameObject GameObject { get; private set; }
        public Animator Animator { get; private set; }
        public CharacterInputs Inputs { get; private set; }
        public CharacterCommonBehaviour CommonState { get; private set; }

        public void Init(GameObject characterObject)
        {
            GameObject = characterObject;
            Animator = GameObject.GetComponent<Animator>();
            Inputs = GameObject.GetComponent<CharacterInputs>();
            CommonState = new CharacterCommonBehaviour(this);

            controller = new Core.FSMController<CharacterController, eStates>(this);
            controller.RegisterState(eStates.Idle, new CharacterIdle());
            controller.RegisterState(eStates.Walk, new CharacterWalk());
            controller.RegisterState(eStates.Jump, new CharacterJump());
            controller.RegisterState(eStates.Falling, new CharacterFalling());

            controller.AddMapping(eStates.Idle, eStates.Jump, eStates.Walk, eStates.Falling);
            controller.AddMapping(eStates.Walk, eStates.Jump, eStates.Idle, eStates.Falling);
            controller.AddMapping(eStates.Jump, eStates.Idle, eStates.Walk, eStates.Falling);
            controller.AddMapping(eStates.Falling, eStates.Idle, eStates.Walk, eStates.Jump);

            controller.SetLogToGUI(true, 1);

            controller.SetState(eStates.Idle);
        }

        public void FixedUpdate()
        {
            if (controller != null)
                controller.FixedUpdate();
        }
        public void Update()
        {
            if (controller != null)
                controller.Update();
        }

        public void LateUpdate()
        {
            if (controller != null)
                controller.LateUpdate();
        }

        public void Notify(params object[] arguments)
        {
            if (controller != null)
                controller.Notify(arguments);
        }

        private Core.FSMController<CharacterController, eStates> controller = null;

    }
}