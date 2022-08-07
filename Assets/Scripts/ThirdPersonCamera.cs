using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public class ThirdPersonCamera : UnityEngine.MonoBehaviour
    {
        public enum eMode
        {
            UNDEFINED = -1,

            THIRD_PERSON_CENTER = 0,
            THIRD_PERSON_OFFSET_LEFT,
            THIRD_PERSON_OFFSET_RIGHT,

            FIRST_PERSON,
        }

        [System.Serializable]
        public class ModeProperties
        {
            [UnityEngine.SerializeField] public eMode mode;

            [UnityEngine.SerializeField] public Transform transformToFollow;
            [UnityEngine.SerializeField] public Vector3 positionOffset;
            [UnityEngine.SerializeField] public Vector3 lookatOffset;

            [UnityEngine.SerializeField] public float movementSpeed;
            [UnityEngine.SerializeField] public float rotationSpeed;
        }

        [System.Serializable]
        public class TransitionProperties
        {
            //[UnityEngine.SerializeField] public float movementSpeed;
            //[UnityEngine.SerializeField] public float rotationSpeed;
            [UnityEngine.SerializeField] public float transitionTime;
        }

        public eMode Mode
        {
            get
            {
                if (!currentMode.HasValue)
                    return eMode.UNDEFINED;
                return currentMode.Value;
            }
        }

        void Awake()
        {
            foreach (var modeProperties in modePropertiesArr)
            {
                this.modesMap[modeProperties.mode] = modeProperties;
            }
        }

        public void SetTransformToFollow(eMode mode, Transform transformToFollow)
        {
            Core.QLogger.Assert(modesMap.ContainsKey(mode));
            modesMap[mode].transformToFollow = transformToFollow;
        }

        public void SnapToMode(eMode mode)
        {
            currentMode = mode;
            nextMode = null;
            isTransisioning = false;
        }
        public void TransitionToMode(eMode mode)
        {
            Core.QLogger.Assert(currentMode.HasValue);  // you should have current mode to be able to transition to something else 
            if (currentMode == mode)
                return;

            Core.QLogger.Assert(modesMap.ContainsKey(mode) && modesMap[mode].transformToFollow != null);

            nextMode = mode;
            isTransisioning = true;
            currentTransitionTime = 0;
        }


        void FixedUpdate()
        {
            if (isTransisioning)
            {
                FixedUpdateTransition();
            }
            else
                FixedUpdateCurrentMode();
        }

        void FixedUpdateTransition()
        {
            Vector3 lerpTargetPosition;
            Quaternion slerpTargetRotation;

            GetNextPositionAndRotation(nextMode.Value, out lerpTargetPosition, out slerpTargetRotation);

            currentTransitionTime += UnityEngine.Time.fixedDeltaTime;
            if(currentTransitionTime >= transitionProperties.transitionTime )
            {
                currentTransitionTime = transitionProperties.transitionTime;
                isTransisioning = false;
                currentMode = nextMode;
            }

            transform.position = (Vector3.Slerp(transform.position, lerpTargetPosition, currentTransitionTime / transitionProperties.transitionTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, slerpTargetRotation, currentTransitionTime / transitionProperties.transitionTime);
        }

        void FixedUpdateCurrentMode()
        {
            Vector3 lerpTargetPosition;
            Quaternion slerpTargetRotation;

            GetNextPositionAndRotation(currentMode.Value, out lerpTargetPosition, out slerpTargetRotation);

            transform.position = (Vector3.Slerp(transform.position, lerpTargetPosition, currentModeProperties.movementSpeed * UnityEngine.Time.fixedDeltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, slerpTargetRotation, currentModeProperties.rotationSpeed * UnityEngine.Time.fixedDeltaTime);
        }

        void GetNextPositionAndRotation(eMode mode, out Vector3 lerpTargetPosition, out Quaternion slerpTargetRotation)
        {
            Core.QLogger.Assert(modesMap.ContainsKey(mode) && modesMap[mode].transformToFollow != null);
            ModeProperties modeProperties = modesMap[mode];

            Vector3 transformedPositionOffsetBasedOnOrientation = modeProperties.transformToFollow.transform.localToWorldMatrix.MultiplyVector(modeProperties.positionOffset);
            Vector3 transformedLookatOffsetBasedOnOrientation = modeProperties.transformToFollow.transform.localToWorldMatrix.MultiplyVector(modeProperties.lookatOffset);
            lerpTargetPosition = modeProperties.transformToFollow.position + transformedPositionOffsetBasedOnOrientation;
            slerpTargetRotation = Quaternion.LookRotation((modeProperties.transformToFollow.position + transformedLookatOffsetBasedOnOrientation) - lerpTargetPosition, Vector3.up);
        }

        Vector3 MultiplyVectors(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        private ModeProperties currentModeProperties
        {
            get
            {
                Core.QLogger.Assert(modesMap.ContainsKey(currentMode.Value));
                return modesMap[currentMode.Value];
            }
        }

        [UnityEngine.SerializeField] ModeProperties[] modePropertiesArr;
        [UnityEngine.SerializeField] TransitionProperties transitionProperties;


        private eMode? currentMode = null;
        private eMode? nextMode = null;

        // Transition states 
        bool isTransisioning = false;       // Is Transitioning from one mode to the next 
        float currentTransitionTime = 0;


        Dictionary<eMode, ModeProperties> modesMap = new Dictionary<eMode, ModeProperties>();
    }
}