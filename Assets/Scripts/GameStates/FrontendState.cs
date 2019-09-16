using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;
using Core.FSMStateMachine;

public class FrontendState : FSMBaseState 
{
    public override void OnEnter()
    { 
        base.OnEnter(); 

    }

    public override void OnContext(System.Object context) 
    { 
        base.OnContext( context );

        GameObject voxelCharacter = GameObject.Instantiate ( ResourceManager.Instance.LoadAsset<UnityEngine.Object> ( "Characters/VoxelGirl/MainCharacter") ) as GameObject;
        if ( voxelCharacter == null ) QLogger.LogErrorAndThrowException ( "VoxelGirl is not instantiated");

        // Main.Instance.uIManager = uiManagerGo.GetComponent<UIManager>();
        // if ( Main.Instance.uIManager == null ) QLogger.LogErrorAndThrowException ( "UiManager script was not instantiated");

        controller = new CharacterController();
        controller.Init( voxelCharacter );

        Core.Updater.Instance.FixedUpdater += FixedUpdate;
        Core.Updater.Instance.LateUpdater += LateUpdate;
    }

    public void FixedUpdate ()
    {
        if ( controller != null )
        {
            controller.FixedUpdate();
        }
    }

	public override void Update()
	{
		base.Update();
        if ( controller != null )
        {
            controller.Update();
        }
	}

    public void LateUpdate ()
    {
        if ( controller != null )
        {
            controller.LateUpdate();
        }
    }

	public override void OnExit()
	{
		base.OnExit();
        Core.Updater.Instance.FixedUpdater -= FixedUpdate;
        Core.Updater.Instance.LateUpdater -= LateUpdate;
	}

	CharacterController controller = null;
}
