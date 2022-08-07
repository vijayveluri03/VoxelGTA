using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{

    public class ThirdPersonCamera : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] Transform characterToFollow;
        [UnityEngine.SerializeField] Vector3 offsetDistance;

        public float movementSpeed;
        public float rotationSpeed;


        private Vector3 moveTo;
        private Quaternion slerpTo;


        void Start()
        {

        }

        public void SetCharacterToFollow(Transform characterToFollow)
        {
            this.characterToFollow = characterToFollow;
        }

        public void SwitchCameraPreset()
        {
        }

        void FixedUpdate()
        {
            if (characterToFollow == null)
                return;

            Vector3 currenctOffset = characterToFollow.transform.localToWorldMatrix.MultiplyVector(offsetDistance);

            moveTo = characterToFollow.position + currenctOffset;
            slerpTo = Quaternion.LookRotation(characterToFollow.position - transform.position, Vector3.up);
            transform.LookAt(characterToFollow, Vector3.up);



            transform.position = (Vector3.Slerp(transform.position, moveTo, movementSpeed * UnityEngine.Time.fixedDeltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, slerpTo, rotationSpeed * UnityEngine.Time.fixedDeltaTime);
        }

        Vector3 MultiplyVectors(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }
}