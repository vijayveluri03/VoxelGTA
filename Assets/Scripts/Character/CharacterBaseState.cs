using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.FSMController;


public class CharacterBaseState : FSMController<CharacterController, CharacterController.eStates>.FSMState
{
}


public class CharacterIdle : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        Debug.LogWarning("Yuaay. we are here ");
    }

    
    public override void Notify( params object[] arguments ) 
    { 

    }
}

public class CharacterIdleWalk : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        Debug.LogWarning("Yuaay. we are here ");
    }

    
    public override void Notify( params object[] arguments ) 
    { 

    }
}

public class CharacterJump : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        Debug.LogWarning("Yuaay. we are here ");
    }

    
    public override void Notify( params object[] arguments ) 
    { 

    }
}