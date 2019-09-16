using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.FSMController;

public class CharacterController 
{
    public enum eStates 
    {
        Idle,
        Walk, 
        Jump
    }
    public GameObject GameObject { get; private set; }
    public Animator Animator { get; private set; }
    public CharacterInputs Inputs { get; private set; }

    public void Init ( GameObject characterObject )
    {
        GameObject = characterObject;
        Animator = GameObject.GetComponent<Animator>();
        Inputs = GameObject.GetComponent<CharacterInputs>();

        controller = new FSMController<CharacterController, eStates>( this );
        controller.RegisterState ( eStates.Idle, new CharacterIdle() );
        controller.RegisterState ( eStates.Walk, new CharacterIdleWalk() );
        controller.RegisterState ( eStates.Jump, new CharacterJump() );
        
        controller.AddMapping ( eStates.Idle, eStates.Jump );
        controller.AddMapping ( eStates.Idle, eStates.Walk );

        controller.AddMapping ( eStates.Walk, eStates.Jump );
        controller.AddMapping ( eStates.Walk, eStates.Idle );

        controller.AddMapping ( eStates.Jump, eStates.Idle );
        controller.AddMapping ( eStates.Jump, eStates.Walk );
        
        controller.SetState ( eStates.Idle );
    }

    public void FixedUpdate ()
    {
        if ( controller != null )
            controller.FixedUpdate();
    }
    public void Update ()
    {
        if ( controller != null )
            controller.Update();
    }

    public void LateUpdate () 
    {
        if ( controller != null )
            controller.LateUpdate();
    }

    public void Notify( params object[] arguments ) 
    { 
        if ( controller != null )
            controller.Notify ( arguments );
    }
    
    private FSMController< CharacterController, eStates> controller = null;

}
