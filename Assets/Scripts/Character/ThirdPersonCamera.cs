using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform characterToFollow;
    [SerializeField] Vector3 offsetDistance;
    
	public float movementSpeed;
    public float rotationSpeed;


        private Vector3 moveTo;
    private Quaternion slerpTo;


    void Start()
    {
        
    }


    void FixedUpdate ()
    {
        if ( characterToFollow == null )
            return;

        Vector3 currenctOffset = characterToFollow.transform.localToWorldMatrix.MultiplyVector( offsetDistance );
        
        moveTo =  characterToFollow.position + currenctOffset;
        slerpTo = Quaternion.LookRotation ( characterToFollow.position - transform.position, Vector3.up);
        transform.LookAt ( characterToFollow, Vector3.up );



        transform.position = ( Vector3.Slerp ( transform.position, moveTo, movementSpeed * Time.fixedDeltaTime ) );
        transform.rotation = Quaternion.Slerp ( transform.rotation, slerpTo, rotationSpeed * Time.fixedDeltaTime );
    }

    Vector3 MultiplyVectors ( Vector3 a, Vector3 b )
    {
        return new Vector3 ( a.x * b.x, a.y * b.y, a.z * b.z );
    }
}
