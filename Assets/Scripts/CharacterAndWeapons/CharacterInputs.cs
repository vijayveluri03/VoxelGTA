using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    
    public enum eAnimationStates
    {
        Running = 1,
        Walk = 2,
        Jump = 3,
        Idle = 4
    }
    public class CharacterInputs : MonoBehaviour
    {

        [UnityEngine.Header("Forward Movement")]
        public float movementSpeedMax;
        public float acceleration;
        public float dragHorizontal;

        [UnityEngine.Header("Strafing")]
        public float strafeSpeedMax;
        public float strafeAcceleration;
        public float strafeDragHorizontal;


        [UnityEngine.Header("Rotation")]
        public float mouseIntensity;
        public float rotationSlerpSpeed;

        [UnityEngine.Header("Jump and gravity ")]
        public float jumpForceValue;
        public float gravity = -9.8f;
        public float dragVertical;
        public UnityEngine.LayerMask ground;


        [UnityEngine.Header("Character details")]
        public Transform centerTransform;           // Character's center 
        public float exactDistanceToBottom;         // to know the legs position
        public float acceptableDistanceToBottom;    // to know the legs position - accurately 
        public float exactDistanceToFront;          // To know the front position - for collisions 



        [UnityEngine.Header("Weapons")]
        public Transform weaponRoot;


        // //private Rigidbody rb;
        // private Animator animator;
        // private Vector3 PositionPseudo;
        // private Quaternion slerpTo;

        // private bool isGrounded = false;
        // private int jumpCountFromGround = 0;
        // private float VerticalForce  { get { return verticalForce;} set { verticalForce = value; Core.QLogger.LogInfo("VerticalForce " + verticalForce); }}
        // private float verticalForce = 0;

        // private RaycastHit hitInfo = new RaycastHit();
        // private float horizontalVelocityRaw = 0;
        // private Vector3 verticalVelocity = Vector3.zero;
        // private Vector3 horizontalVelocity = Vector3.zero;

        // private float reAdjustYBy = 0;
        // private AnimationClip runAnimationClip = null;
        // private string previousPlayedAnimationName = "";

        // // private int groundLayerMask;

        // // Start is called before the first frame update
        // void Start()
        // {
        // 	//rb = GetComponent<Rigidbody>();
        //     animator = GetComponent<Animator>();
        //     isGrounded = false;

        //     Core.QLogger.Assert ( acceptableDistanceToBottom > exactDistanceToBottom && exactDistanceToBottom > 0 );

        //     // AnimationClip[] allClips = animator.runtimeAnimatorController.animationClips;
        //     // Core.QLogger.Assert ( allClips != null && allClips.Length > 0 );

        //     // foreach ( AnimationClip a in allClips )
        //     // {
        //     //     if ( a.name.Contains("run") )
        //     //     {
        //     //         runAnimationClip = a;
        //     //         Core.QLogger.LogInfo("Animation picked for run :" + runAnimationClip.name );
        //     //         break;
        //     //     }
        //     // }
        // }

        // // Update is called once per frame
        // void FixedUpdate()
        // {
        //     isGrounded = false;
        //     reAdjustYBy = 0;
        //     PositionPseudo = transform.position;

        //     if ( Physics.Raycast( centerTransform.position, -transform.up, out hitInfo, exactDistanceToBottom * 3, ground.value ) )
        //     {
        //         float hitDistanceFromCenter = Vector3.Distance ( hitInfo.point, centerTransform.position);
        //         if ( hitDistanceFromCenter <= acceptableDistanceToBottom )
        //         {
        //             isGrounded = true;
        //             verticalVelocity.y = 0;
        //         }
        //         if ( hitDistanceFromCenter < exactDistanceToBottom )
        //         {
        //             // pushing it up
        //             reAdjustYBy = acceptableDistanceToBottom - exactDistanceToBottom;
        //         }
        //     }

        // 	float h = Input.GetAxisRaw("Horizontal");
        // 	float v = Input.GetAxisRaw("Vertical");
        //     horizontalVelocity = Vector3.zero;

        // 	if (v != 0)
        // 	{
        //         horizontalVelocityRaw += acceleration * Time.fixedDeltaTime * Mathf.Sign ( v );
        //         horizontalVelocityRaw = Mathf.Min ( horizontalVelocityRaw, movementSpeedMax );
        // 	}

        //     if ( dragHorizontal != 0 )
        //     {
        //         horizontalVelocityRaw /= 1 + dragHorizontal * Time.fixedDeltaTime;
        //     }

        //     horizontalVelocity = transform.forward * horizontalVelocityRaw;


        //     if ( h != 0 )
        //     {
        //         Quaternion q = transform.rotation;
        //         Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
        //         slerpTo = q * deltaAngle;
        //     }

        //     if ( Input.GetKeyDown( KeyCode.Space))
        //     {
        //             JumpIfPossible ( jumpForceValue );
        //             verticalVelocity.y += VerticalForce;
        //             jumpCountFromGround ++ ;
        //     }

        //     if ( !isGrounded )
        //         verticalVelocity.y += gravity * Time.fixedDeltaTime;

        //     MoveWithTime ( horizontalVelocity );
        //     MoveWithTime ( verticalVelocity );

        //     if ( reAdjustYBy != 0 )
        //     {
        //        MoveBy ( new Vector3 ( 0, reAdjustYBy, 0 ));
        //        Core.QLogger.LogInfo("Readjustment : " + reAdjustYBy);
        //     }

        //     transform.position =  PositionPseudo;// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );
        //     transform.rotation = Quaternion.Slerp ( transform.rotation, slerpTo, rotationSpeed * Time.deltaTime );

        //     if ( !isGrounded )
        //         SetAnimation ( eAnimationStates.Jump );
        //     else if ( Mathf.Abs( horizontalVelocityRaw ) > 2.5f/* minimal value. can be exposed  */  ) 
        //         SetAnimation ( eAnimationStates.Running );
        //     else 
        //         SetAnimation ( eAnimationStates.Idle );

        //     // animator.SetBool("IsJumping", !isGrounded );

        // }

        // void Update ()
        // {

        // }

        // void MoveWithTime ( Vector3 force )
        // {
        //     PositionPseudo += ( force * Time.fixedDeltaTime );
        // }
        // void MoveBy ( Vector3 delta )
        // {
        //     PositionPseudo += delta;
        // }

        // void JumpIfPossible ( float force )
        // {
        //     if ( jumpCountFromGround > 2 )
        //         return;

        //     jumpCountFromGround ++ ;
        //     VerticalForce = jumpCountFromGround == 1 ? force : force * 1.3f;// hack, but its fine for now

        //     // animator.SetTrigger("IsJumpInitiated");
        // }

        // void SetAnimation ( eAnimationStates state )
        // {
        //     if ( previousState != state )
        //         animator.SetInteger( "AnimationState", (int)state);

        //     previousState = state;
        // }

        // private eAnimationStates previousState = 0;
        // // void OnCollisionEnter(Collision collision)
        // // {
        // //     Core.QLogger.LogInfo( "Collided:" + collision.gameObject.layer );
        // //     if ( collision != null && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // //     {
        // //         Core.QLogger.LogInfo("Grounded");
        // //         isGrounded = true;
        // //         jumpCountFromGround = 0;
        // //         VerticalForce = 0;
        // //     }
        // // }
    }
}