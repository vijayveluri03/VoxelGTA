using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.FSMController;
using Core;

public class WeaponCommon 
{
    public WeaponCommon ( WeaponController owner )
    {
        this.owner = owner;
    }

    #region Animation

    public void SetAnimation ( WeaponController.eStates state )
    {
        if ( previousState != state )
            animator.SetInteger( "AnimationState", (int)state);

        previousState = state;
    }

    public void SetAnimation ( WeaponController.eStates state, float time )
    {
        if ( previousState != state )
        {
            animator.SetInteger( "AnimationState", (int)state);
            animator.time
        }

        previousState = state;
    }

    private WeaponController.eStates previousState = 0;
    public Animator animator { get { return owner.Animator; }}

    #endregion

    public void FireBullet( float accuracy )
    {
        QLogger.Assert ( bulletsRemainingInMag > 0 );

        GameObject bullet = GameObject.Instantiate ( inputs.bulletReference);
        bullet.transform.position = inputs.muzzlePositionAndDirection.position;
        bullet.transform.forward = inputs.muzzlePositionAndDirection.forward;
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        QLogger.Assert ( bulletScript != null );
        bulletScript.Init ( inputs.muzzlePositionAndDirection.forward, inputs.damage, inputs.range );

        bulletsRemainingInMag --;
    }

    public bool CanAutoShootNextBullet ()
    {
        return ( inputs.shotType == eShotType.FULLY_AUTO && bulletsRemainingInMag > 0 );
    }

    public bool IsMagazineEmpty ()
    {
        Core.QLogger.Assert ( bulletsRemainingInMag >= 0 );
        return bulletsRemainingInMag <= 0;
    }
    public bool IsMagazineFull ()
    {
        return ( bulletsRemainingInMag == inputs.magCapacity );
    }
    public void SetBulletRemainingcount ( int count )
    {
        bulletsRemainingInMag = count;
    }

    public void Init()
    {
        SetBulletRemainingcount ( inputs.magCapacity );
    }
    public int bulletsRemainingInMag { get; private set; }
    private WeaponController owner;
    public WeaponInputs inputs { get { return owner.Inputs; }}
}

public class WeaponCore : FSMController<WeaponController, WeaponController.eStates>.FSMState
{
	public Transform transform { get { return Owner.GameObject.transform; }}
    public GameObject gameObject { get { return Owner.GameObject; }}
    public WeaponInputs inputs { get { return Owner.Inputs; }}
    public WeaponCommon common { get { return Owner.Common; }}
    public Animator animator { get { return Owner.Animator; }}
}

public class WeaponBaseState : WeaponCore
{
    private AnimationClip runAnimationClip = null;
    private string previousPlayedAnimationName = "";

	// Start is called before the first frame update
	public override void OnEnter( params object[] arguments )
	{
        base.OnEnter ( arguments );
	}

    public override void Update ()
    {
        base.Update();
    }
}

public class WeaponIdle : WeaponBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        common.SetAnimation( WeaponController.eStates.Idle);
        Debug.LogWarning("WeaponIdle");
    }

    public override void Update()
    {
        base.Update();
        
        // temp hardcode 
        if ( Input.GetKeyDown( KeyCode.Mouse0))
        {
            SetState(  WeaponController.eStates.Shoot );
        }
        //temp hardcode
        if ( Input.GetKeyDown( KeyCode.R))
        {
            if ( !common.IsMagazineFull() )
                SetState(  WeaponController.eStates.Reload );
        }

    }
    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
}

public class WeaponShoot : WeaponBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        common.SetAnimation( WeaponController.eStates.Shoot);
        Debug.LogWarning("WeaponIdleWalk");

        // Fire one bullet and go to recoil
        common.FireBullet( 1 /* accuracy */);
        SetState ( WeaponController.eStates.Recoil );
    }

    
    public override void Update()
    {
        base.Update();


    }

    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }

}

public class WeaponReload : WeaponBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        common.SetAnimation( WeaponController.eStates.Reload);
        time = 0;
    }

    public override void Update()
    {
        base.Update();
        time += Time.deltaTime;
        if ( time >= inputs.reloadTime )
        {
            if ( Input.GetMouseButton(0) && common.CanAutoShootNextBullet() && !common.IsMagazineEmpty() )
                SetState ( WeaponController.eStates.Shoot);
            else 
                SetState ( WeaponController.eStates.Idle);
        }
    }

    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
    float time;
}

public class WeaponRecoil : WeaponBaseState 
{
    
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        common.SetAnimation( WeaponController.eStates.Recoil);
        Debug.LogWarning("WeaponRecoil");
        recoilTime = 0;
    }

    public override void Update()
    {
        base.Update();
        recoilTime += Time.deltaTime;
        if ( recoilTime >= inputs.recoilTime )    
        {
            //todo hardcode 
            if ( Input.GetMouseButton(0) && common.CanAutoShootNextBullet() && !common.IsMagazineEmpty() )
                SetState ( WeaponController.eStates.Shoot );
            else if ( common.IsMagazineEmpty() )
                SetState ( WeaponController.eStates.Reload );
            else 
                SetState ( WeaponController.eStates.Idle);
        }
    } 

    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
    float recoilTime;
}