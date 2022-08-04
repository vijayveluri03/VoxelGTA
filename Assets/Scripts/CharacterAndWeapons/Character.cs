// using System.Collections;
// using System.Collections.Generic;
// 

// public class Character : MonoBehaviour
// {

// 	public float movementSpeed;
//     public float rotationSpeed;
//     public float jumpForceValue;
//     public float gravity = 9.8f;

// 	private Rigidbody rb;
//     private Animator animator;
//     private Vector3 moveTo;
//     private Quaternion slerpTo;
//     private bool grounded = false;
//     private int jumpCount = 0;
//     private float VerticalForce  { get { return verticalForce;} set { verticalForce = value; Core.QLogger.LogInfo("VerticalForce " + verticalForce); }}
//     private float verticalForce = 0;
//     private RaycastHit hitInfo = new RaycastHit();
//     private int groundLayerMask;

// 	// Start is called before the first frame update
// 	void Start()
// 	{
// 		rb = GetComponent<Rigidbody>();
//         animator = GetComponent<Animator>();
//         groundLayerMask = 1 << LayerMask.NameToLayer ( "Ground");
//         grounded = false;
// 	}

// 	// Update is called once per frame
// 	void FixedUpdate()
// 	{
// 		float h = Input.GetAxisRaw("Horizontal");
// 		float v = Input.GetAxisRaw("Vertical");
//         moveTo = rb.transform.position;

// 		if (v != 0)
// 		{
// 			Vector3 directionVector = new Vector3(0, 0, v);
// 			directionVector = transform.localToWorldMatrix.MultiplyVector(directionVector);
// 			Vector3 movement = directionVector.normalized * movementSpeed * Time.deltaTime;
// 			moveTo = ( rb.transform.position + movement);
// 		}

//         animator.SetBool("IsRunning", v != 0 );
        
//         if ( h != 0 )
//         {
//             Quaternion q = transform.rotation;
//             Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
//             slerpTo = q * deltaAngle;
//         }

//         animator.SetBool("IsJumping", !grounded );
//         if ( grounded )
//         {
//             if ( Physics.Raycast( transform.position, -transform.up, out hitInfo, 1000, groundLayerMask ) )
//             {
//                 if ( Vector3.Distance ( hitInfo.point, transform.position) < 0.1f )
//                 {
//                     moveTo.y = hitInfo.point.y;
//                 }
//                 else if ( hitInfo.point.y > transform.position.y )
//                 {
//                     // push out 
//                     moveTo.y = hitInfo.point.y;
//                 }
//                 else 
//                 {
//                     grounded = false;
//                 }
//             }
//         }
//         else 
//         {
//             VerticalForce -= gravity * Time.deltaTime;
//             moveTo += Vector3.up * VerticalForce * Time.deltaTime;
//         }



//         if ( Input.GetKeyDown( KeyCode.Space))
//         {
//                 JumpIfPossible ( jumpForceValue );
//                 jumpCount ++ ;
//         }

//         //rb.transform.position = Vector3.MoveTowards ( rb.transform.position, moveTo, movementSpeed * Time.deltaTime );
//         //rb.MovePosition( Vector3.Slerp ( rb.transform.position, moveTo, movementSpeed * Time.deltaTime ) );
//         rb.transform.position = ( Vector3.Slerp ( rb.transform.position, moveTo, movementSpeed * Time.deltaTime ) );
//         transform.rotation = Quaternion.Slerp ( transform.rotation, slerpTo, rotationSpeed * Time.deltaTime );
// 	}


//     void JumpIfPossible ( float force )
//     {
//         if ( jumpCount > 2 )
//             return;

//         grounded = false;
//         jumpCount ++ ;
//         VerticalForce = jumpCount == 1 ? force : force * 1.3f;// hack, but its fine for now
        
//         animator.SetTrigger("IsJumpInitiated");
//     }


//     void OnCollisionEnter(Collision collision)
//     {
//         Core.QLogger.LogInfo( "Collided:" + collision.gameObject.layer );
//         if ( collision != null && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
//         {
//             Core.QLogger.LogInfo("Grounded");
//             grounded = true;
//             jumpCount = 0;
//             VerticalForce = 0;
//         }
//     }
// }
