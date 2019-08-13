using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;
using Core.FSMStateMachine;

public class FrontendState : FSMBaseState 
{
    public struct SessionStartData 
	{
	}

    public override void OnEnter()
    { 
        base.OnEnter(); 

    }

    public void CreateANewSessionAndProceedToGamePlay ( SessionStartData data )
    {
    }
    public override void Update()   {   base.Update();   }
    public override void OnExit(){ base.OnExit(); }
}
