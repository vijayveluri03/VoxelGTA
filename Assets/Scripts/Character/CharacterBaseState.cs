using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.FSMController;


public class CharacterCore : FSMController<CharacterController, CharacterController.eStates>.FSMState
{
	public Transform transform { get { return Owner.GameObject.transform; }}
    public GameObject gameObject { get { return Owner.GameObject; }}
    public CharacterInputs inputs { get { return Owner.Inputs; }}
}

public class CharacterBaseState : CharacterCore
{
    // public float movementSpeedMax;
    // public float acceleration;
    // public float rotationSpeed;
    // public float jumpForceValue;
    // public float gravity = -9.8f;
    // public LayerMask ground;
    // public float dragHorizontal;
    // public float dragVertical;

    
    // // public Transform bottomTransform;
    // // public Transform frontTransform;
    // public Transform centerTransform;
    // public float exactDistanceToBottom;
    // public float acceptableDistanceToBottom;
    // public float exactDistanceToFront;


	//private Rigidbody rb;
    private Animator animator;
    private Vector3 PositionPseudo;
    private Quaternion slerpTo;

    private bool isGrounded = false;
    private int jumpCountFromGround = 0;
    private float VerticalForce  { get { return verticalForce;} set { verticalForce = value; Debug.Log("VerticalForce " + verticalForce); }}
    private float verticalForce = 0;

    private RaycastHit hitInfo = new RaycastHit();
    private float horizontalVelocityRaw = 0;
    private Vector3 verticalVelocity = Vector3.zero;
    private Vector3 horizontalVelocity = Vector3.zero;

    private float reAdjustYBy = 0;
    private AnimationClip runAnimationClip = null;
    private string previousPlayedAnimationName = "";

    // private int groundLayerMask;

	// Start is called before the first frame update
	public override void OnEnter( params object[] arguments )
	{
        base.OnEnter ( arguments );

		//rb = GetComponent<Rigidbody>();
        animator = Owner.Animator;
        isGrounded = false;

        Core.QLogger.Assert ( inputs.acceptableDistanceToBottom > inputs.exactDistanceToBottom && inputs.exactDistanceToBottom > 0 );

        // AnimationClip[] allClips = animator.runtimeAnimatorController.animationClips;
        // Core.QLogger.Assert ( allClips != null && allClips.Length > 0 );

        // foreach ( AnimationClip a in allClips )
        // {
        //     if ( a.name.Contains("run") )
        //     {
        //         runAnimationClip = a;
        //         QLogger.LogInfo("Animation picked for run :" + runAnimationClip.name );
        //         break;
        //     }
        // }
	}

	// Update is called once per frame
	public override void FixedUpdate()
	{
        base.FixedUpdate();

        isGrounded = false;
        reAdjustYBy = 0;
        PositionPseudo = transform.position;

        if ( Physics.Raycast( inputs.centerTransform.position, -transform.up, out hitInfo, inputs.exactDistanceToBottom * 3, inputs.ground.value ) )
        {
            float hitDistanceFromCenter = Vector3.Distance ( hitInfo.point, inputs.centerTransform.position);
            if ( hitDistanceFromCenter <= inputs.acceptableDistanceToBottom )
            {
                isGrounded = true;
                verticalVelocity.y = 0;
            }
            if ( hitDistanceFromCenter < inputs.exactDistanceToBottom )
            {
                // pushing it up
                reAdjustYBy = inputs.acceptableDistanceToBottom - inputs.exactDistanceToBottom;
            }
        }

		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
        horizontalVelocity = Vector3.zero;

		if (v != 0)
		{
            horizontalVelocityRaw += inputs.acceleration * Time.fixedDeltaTime * Mathf.Sign ( v );
            horizontalVelocityRaw = Mathf.Min ( horizontalVelocityRaw, inputs.movementSpeedMax );
		}

        if ( inputs.dragHorizontal != 0 )
        {
            horizontalVelocityRaw /= 1 + inputs.dragHorizontal * Time.fixedDeltaTime;
        }

        horizontalVelocity = transform.forward * horizontalVelocityRaw;

        
        if ( h != 0 )
        {
            Quaternion q = transform.rotation;
            Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
            slerpTo = q * deltaAngle;
        }
        
        if ( Input.GetKeyDown( KeyCode.Space))
        {
                JumpIfPossible ( inputs.jumpForceValue );
                verticalVelocity.y += VerticalForce;
                jumpCountFromGround ++ ;
        }

        if ( !isGrounded )
            verticalVelocity.y += inputs.gravity * Time.fixedDeltaTime;

        MoveWithTime ( horizontalVelocity );
        MoveWithTime ( verticalVelocity );

        if ( reAdjustYBy != 0 )
        {
           MoveBy ( new Vector3 ( 0, reAdjustYBy, 0 ));
           Debug.Log("Readjustment : " + reAdjustYBy);
        }

        transform.position =  PositionPseudo;// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );
        transform.rotation = Quaternion.Slerp ( transform.rotation, slerpTo, inputs.rotationSpeed * Time.deltaTime );

        if ( !isGrounded )
            SetAnimation ( eAnimationStates.Jump );
        else if ( Mathf.Abs( horizontalVelocityRaw ) > 2.5f/* minimal value. can be exposed  */  ) 
            SetAnimation ( eAnimationStates.Running );
        else 
            SetAnimation ( eAnimationStates.Idle );

        // animator.SetBool("IsJumping", !isGrounded );

	}

    public override void Update ()
    {
        base.Update();
    }

    void MoveWithTime ( Vector3 force )
    {
        PositionPseudo += ( force * Time.fixedDeltaTime );
    }
    void MoveBy ( Vector3 delta )
    {
        PositionPseudo += delta;
    }

    void JumpIfPossible ( float force )
    {
        if ( jumpCountFromGround > 2 )
            return;

        jumpCountFromGround ++ ;
        VerticalForce = jumpCountFromGround == 1 ? force : force * 1.3f;// hack, but its fine for now
        
        // animator.SetTrigger("IsJumpInitiated");
    }

    void SetAnimation ( eAnimationStates state )
    {
        if ( previousState != state )
            animator.SetInteger( "AnimationState", (int)state);

        previousState = state;
    }

    private eAnimationStates previousState = 0;
    // void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log( "Collided:" + collision.gameObject.layer );
    //     if ( collision != null && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //     {
    //         Debug.Log("Grounded");
    //         isGrounded = true;
    //         jumpCountFromGround = 0;
    //         VerticalForce = 0;
    //     }
    // }
}


public class CharacterIdle : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        Debug.LogWarning("Yuaay. we are here ");
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
        Debug.LogWarning("Yuaay. we are here ");
    }

    
    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
}

public class CharacterJump : CharacterBaseState 
{
    public override void OnEnter(params object[] arguments)
    {
        base.OnEnter( arguments );
        Debug.LogWarning("Yuaay. we are here ");
    }

    
    public override void Notify( params object[] arguments ) 
    { 
        base.Notify(arguments);
    }
}