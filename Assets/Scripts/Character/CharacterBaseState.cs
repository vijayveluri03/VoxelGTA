using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.FSMController;
using Core;

public class CharacterCommon 
{
    public CharacterCommon ( CharacterController owner )
    {
        this.owner = owner;
    }

    #region Movement 

    public Vector3  MoveWithForce_WithFixedUpdate ( Vector3 pos, Vector3 force )
    {
        pos += ( force * Time.fixedDeltaTime );
        return pos;
    }
    public Vector3 MoveBy ( Vector3 pos, Vector3 delta )
    {
        pos += delta;
        return pos;
    }
    public float verticalVelocity = 0;
    public Vector3 horizontalVelocity = Vector3.zero;

    public Vector3 UpdateAndGetPositionBasedOnVelocities_WithFixedUpdate( Vector3 position )
    {
        if ( !isGrounded )
            verticalVelocity += inputs.gravity * Time.fixedDeltaTime;

        position = MoveWithForce_WithFixedUpdate ( position, horizontalVelocity );
        position = MoveWithForce_WithFixedUpdate ( position, Vector3.up * verticalVelocity );

        if ( reAdjustYBy != 0 )
        {
           position = MoveBy ( position, new Vector3 ( 0, reAdjustYBy, 0 ));
           Debug.Log("Readjustment : " + reAdjustYBy);
        }
        return position;
    }
    public bool IsFalling ()
    {
        if ( isGrounded ) 
            return false;
        return verticalVelocity < 0;
    }

    public bool AreVelocitiesZero ()
    {
        if ( verticalVelocity > 0.1f || horizontalVelocity.sqrMagnitude > 0.2f )
            return false;
        return true;
    }

    #endregion

    #region Animation

    public void SetAnimation ( eAnimationStates state )
    {
        return; // not all animations are present at this moment 
        if ( previousState != state )
            animator.SetInteger( "AnimationState", (int)state);

        previousState = state;
    }

    private eAnimationStates previousState = 0;
    public Animator animator { get { return owner.Animator; }}

    #endregion

    #region Rotation and mouse 

    public void ProcessMouseMovement_WithFixedUpdate ()
    {
        // Hardcoded for now 
        float mouseInputX = Input.GetAxis ("Mouse X") * inputs.mouseIntensity;
        //Debug.Log("Mouse input" + mouseInputX);
        Quaternion a = Quaternion.AngleAxis ( mouseInputX * 360 * Time.fixedDeltaTime, Vector3.up);

        float mouseInputY = Input.GetAxis ("Mouse Y") * inputs.mouseIntensity;
        //Debug.Log("Mouse input" + mouseInputY);
        Quaternion b = Quaternion.AngleAxis ( mouseInputY * 360 * Time.fixedDeltaTime, Vector3.right);
        /// todo : temp.!-- impliment y axis as well 
        b = Quaternion.identity;

        slerpToPseudo = slerpToPseudo * a * b;

        // Quaternion q = transform.rotation;
        // Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
        // slerpToPseudo = q * deltaAngle;

    }

    public Quaternion UpdateAndGetRotationBasedOnVelocities_WithFixedUpdate( Quaternion rot )
    {
        return Quaternion.Slerp ( rot, slerpToPseudo, inputs.rotationSlerpSpeed * Time.fixedDeltaTime );
    }
    

    protected Quaternion slerpToPseudo = Quaternion.identity;


    #endregion

    #region Falling and Grounding 

    public void DoGroundedCheck ()
    {
        isGrounded = false;
        reAdjustYBy = 0;

        if ( verticalVelocity > 0 )
        {

        }
        else 
        {
            if ( Physics.Raycast( inputs.centerTransform.position, -transform.up, out hitInfo, inputs.exactDistanceToBottom * 3, inputs.ground.value ) )
            {
                float hitDistanceFromCenter = Vector3.Distance ( hitInfo.point, inputs.centerTransform.position);
                if ( hitDistanceFromCenter <= inputs.acceptableDistanceToBottom )
                {
                    isGrounded = true;
                    verticalVelocity = 0;
                }
                if ( hitDistanceFromCenter < inputs.exactDistanceToBottom )
                {
                    // pushing it up
                    reAdjustYBy = inputs.acceptableDistanceToBottom - inputs.exactDistanceToBottom;
                }
            }
        }
    }

    private RaycastHit hitInfo = new RaycastHit();
    
    public float reAdjustYBy { get; private set; }
    public bool isGrounded { get; private set; }
    public Transform transform { get { return owner.GameObject.transform; }}
    #endregion

    private CharacterController owner;
    public CharacterInputs inputs { get { return owner.Inputs; }}
}

public class CharacterCore : FSMController<CharacterController, CharacterController.eStates>.FSMState
{
	public Transform transform { get { return Owner.GameObject.transform; }}
    public GameObject gameObject { get { return Owner.GameObject; }}
    public CharacterInputs inputs { get { return Owner.Inputs; }}
    public CharacterCommon common { get { return Owner.Common; }}
    public Animator animator { get { return Owner.Animator; }}
}

public class CharacterBaseState : CharacterCore
{
 
	//private Rigidbody rb;
    protected Vector3 PositionPseudo;
    protected Quaternion RotationPseudo;
    

    private AnimationClip runAnimationClip = null;
    private string previousPlayedAnimationName = "";

    // private int groundLayerMask;

	// Start is called before the first frame update
	public override void OnEnter( params object[] arguments )
	{
        base.OnEnter ( arguments );

        Core.QLogger.Assert ( inputs.acceptableDistanceToBottom > inputs.exactDistanceToBottom && inputs.exactDistanceToBottom > 0 );
	}

	// Update is called once per frame
	public override void FixedUpdate()
	{
        base.FixedUpdate();

	}

    public override void Update ()
    {
        base.Update();
    }

    protected void CachePosition ()
    {
        PositionPseudo = transform.position;
        RotationPseudo = transform.rotation;
        
    }
    protected void ApplyPosition ()
    {
        transform.position =  PositionPseudo;
        transform.rotation = RotationPseudo;
    }
}

public class CharacterIdle : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CachePosition();
        common.DoGroundedCheck();
        common.ProcessMouseMovement_WithFixedUpdate();

        if ( !common.isGrounded )
        {
            SetState ( CharacterController.eStates.Falling );
            return;
        }

        // Hardcoded for now 
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

        if ( v != 0 || h != 0 || !common.AreVelocitiesZero() )
        {
            SetState ( CharacterController.eStates.Walk, v, h );
            return;
        }

        PositionPseudo = common.UpdateAndGetPositionBasedOnVelocities_WithFixedUpdate( PositionPseudo );
        RotationPseudo = common.UpdateAndGetRotationBasedOnVelocities_WithFixedUpdate( RotationPseudo);
        ApplyPosition();

        
        if ( Input.GetKeyDown( KeyCode.Space))
        {
            SetState(  CharacterController.eStates.Jump );
        }
    }
    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
}

public class CharacterIdleWalk : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        forwardRaw = 0;
        strafeRaw = 0;

        if ( arguments != null && arguments.Length == 2 )
            {
                // Move this based on previous values in previous frame 
            }
    }

    
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        CachePosition();

        common.DoGroundedCheck();
        common.ProcessMouseMovement_WithFixedUpdate();

        if ( !common.isGrounded )
        {
            SetState ( CharacterController.eStates.Falling );
            return;
        }

        if ( Input.GetKeyDown( KeyCode.Space))
        {
            SetState(  CharacterController.eStates.Jump );
        }

        // Hardcoded for now 
   		float forwardIntensity = Input.GetAxisRaw("Horizontal");
		float sideIntensity = Input.GetAxisRaw("Vertical");

        Move_WithFixedUpdate( sideIntensity, forwardIntensity );

        PositionPseudo = common.UpdateAndGetPositionBasedOnVelocities_WithFixedUpdate ( PositionPseudo );
        RotationPseudo = common.UpdateAndGetRotationBasedOnVelocities_WithFixedUpdate( RotationPseudo);

        ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );

        //transform.rotation = Quaternion.Slerp ( transform.rotation, slerpToPseudo, inputs.rotationSpeed * Time.fixedDeltaTime );

    }

    public void Move_WithFixedUpdate( float forwardIntensity, float sideIntensity )
    {
        common.horizontalVelocity = Vector3.zero;

		if (forwardIntensity != 0)
		{
            this.forwardRaw += inputs.acceleration * Time.fixedDeltaTime * Mathf.Sign ( forwardIntensity );
            this.forwardRaw = Mathf.Min ( this.forwardRaw, inputs.movementSpeedMax );
		}
      
        if ( inputs.dragHorizontal != 0 )
        {
            this.forwardRaw /= 1 + inputs.dragHorizontal * Time.fixedDeltaTime;
        }

        if (sideIntensity != 0)
		{
            this.strafeRaw += inputs.strafeAcceleration * Time.fixedDeltaTime * Mathf.Sign ( sideIntensity );
            this.strafeRaw = Mathf.Min ( this.strafeRaw, inputs.strafeSpeedMax );
		}

        if ( inputs.strafeDragHorizontal != 0 )
        {
            this.strafeRaw /= 1 + inputs.strafeDragHorizontal * Time.fixedDeltaTime;
        }


		common.horizontalVelocity = (transform.forward * this.forwardRaw) + ( transform.right * this.strafeRaw);

        if ( sideIntensity != 0 )
        {

            // Quaternion q = transform.rotation;
            // Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
            // slerpToPseudo = q * deltaAngle;
        }
    }

    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }

    private float forwardRaw = 0;
    private float strafeRaw = 0;

}

public class CharacterJump : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );

        jumpCountFromGround = 0;

        common.SetAnimation ( eAnimationStates.Jump );
        common.verticalVelocity += JumpIfPossibleAndReturnDetaForce ( inputs.jumpForceValue );;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        CachePosition();

        common.DoGroundedCheck();

        if ( common.isGrounded )
        {
            if ( common.AreVelocitiesZero() )
                SetState( CharacterController.eStates.Idle );
            else 
                SetState ( CharacterController.eStates.Walk );
        }

        if ( Input.GetKeyDown( KeyCode.Space))
        {
            common.verticalVelocity += JumpIfPossibleAndReturnDetaForce ( inputs.jumpForceValue );;
            common.SetAnimation ( eAnimationStates.Jump );
        }

        if ( common.IsFalling () ) 
            SetState ( CharacterController.eStates.Falling );

        PositionPseudo = common.UpdateAndGetPositionBasedOnVelocities_WithFixedUpdate ( PositionPseudo );
        RotationPseudo = common.UpdateAndGetRotationBasedOnVelocities_WithFixedUpdate( RotationPseudo);

        ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );
    }

    
    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }

    float JumpIfPossibleAndReturnDetaForce ( float force )
    {
        if ( jumpCountFromGround > 2 )
            return 0;

        jumpCountFromGround ++ ;
        return jumpCountFromGround == 1 ? force : force * 1.3f;// hack, but its fine for now
    }

    private int jumpCountFromGround = 0;
}

public class CharacterFalling : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
    }

     public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        CachePosition();

        common.DoGroundedCheck();

        if ( common.isGrounded )
        {
            if ( common.AreVelocitiesZero() )
                SetState( CharacterController.eStates.Idle );
            else 
                SetState ( CharacterController.eStates.Walk );
        }

        PositionPseudo = common.UpdateAndGetPositionBasedOnVelocities_WithFixedUpdate ( PositionPseudo );
        RotationPseudo = common.UpdateAndGetRotationBasedOnVelocities_WithFixedUpdate( RotationPseudo);

        ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );

    } 

    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
}