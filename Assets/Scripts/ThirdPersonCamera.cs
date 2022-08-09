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

            [UnityEngine.SerializeField] public Vector3 positionOffset;
            [UnityEngine.SerializeField] public Vector3 lookatOffset;

            [UnityEngine.SerializeField] public float movementSpeed;
            [UnityEngine.SerializeField] public float rotationSpeed;

            [UnityEngine.SerializeField] public bool ignoreMovementSpeedAndSnap;
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

        public eMode NextMode
        {
            get
            {
                if ( !nextMode.HasValue)
                    return eMode.UNDEFINED;
                return nextMode.Value;
            }
        }

        public bool IsTransisioning
        {
            get;
            private set;
        }       // Is Transitioning from one mode to the next 


        public Vector3 LookAtDirection
        {
            get
            {
                return transform.forward;
            }
        }
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        void Awake()
        {
            foreach (var modeProperties in modePropertiesArr)
            {
                this.modesMap[modeProperties.mode] = modeProperties;
            }
        }

        public void Init(PlayerAndCameraSharedModel data_readonly)
        {
            playerSharedData_ro = data_readonly;
        }

        public void SnapToMode(eMode mode)
        {
            currentMode = mode;
            nextMode = null;
            IsTransisioning = false;
        }
        public void TransitionToMode(eMode mode)
        {
            Core.QLogger.Assert(currentMode.HasValue);  // you should have current mode to be able to transition to something else 
            if (currentMode == mode)
                return;

            Core.QLogger.Assert(modesMap.ContainsKey(mode));

            nextMode = mode;
            IsTransisioning = true;
            currentTransitionTime = 0;
        }


        // @todo move this to late update
        void LateUpdate()
        {
            UpdateVerticalRotation();
            if (IsTransisioning)
            {
                UpdateTransition();
            }
            else
                UpdateCurrentMode();
        }

        void UpdateTransition()
        {
            Vector3 lerpTargetPosition;
            Quaternion slerpTargetRotation;

            GetNextPositionAndRotation(nextMode.Value, out lerpTargetPosition, out slerpTargetRotation);

            currentTransitionTime += UnityEngine.Time.deltaTime;
            if(currentTransitionTime >= transitionProperties.transitionTime )
            {
                currentTransitionTime = transitionProperties.transitionTime;
                IsTransisioning = false;
                currentMode = nextMode;
            }

            transform.position = (Vector3.Slerp(transform.position, lerpTargetPosition, currentTransitionTime / transitionProperties.transitionTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, slerpTargetRotation, currentTransitionTime / transitionProperties.transitionTime);
        }

        void UpdateCurrentMode()
        {
            Vector3 lerpTargetPosition;
            Quaternion slerpTargetRotation;

            GetNextPositionAndRotation(currentMode.Value, out lerpTargetPosition, out slerpTargetRotation);

            if (currentModeProperties.ignoreMovementSpeedAndSnap)
            {
                transform.position = lerpTargetPosition;
                transform.rotation = slerpTargetRotation;
            }
            else
            {
                transform.position = (Vector3.Slerp(transform.position, lerpTargetPosition, currentModeProperties.movementSpeed * UnityEngine.Time.deltaTime));
                transform.rotation = Quaternion.Slerp(transform.rotation, slerpTargetRotation * verticalRotation , currentModeProperties.rotationSpeed * UnityEngine.Time.deltaTime);
            }
        }

        void GetNextPositionAndRotation(eMode mode, out Vector3 lerpTargetPosition, out Quaternion slerpTargetRotation)
        {
            Core.QLogger.Assert(modesMap.ContainsKey(mode) );
            Core.QLogger.Assert(playerSharedData_ro != null && playerSharedData_ro.cameraTarget != null);
            ModeProperties modeProperties = modesMap[mode];

            Vector3 transformedPositionOffsetBasedOnOrientation = playerSharedData_ro.cameraTarget.localToWorldMatrix.MultiplyVector(modeProperties.positionOffset);
            Vector3 transformedLookatOffsetBasedOnOrientation = playerSharedData_ro.cameraTarget.localToWorldMatrix.MultiplyVector(modeProperties.lookatOffset);
            lerpTargetPosition = playerSharedData_ro.cameraTarget.position + transformedPositionOffsetBasedOnOrientation;
            slerpTargetRotation = Quaternion.LookRotation((playerSharedData_ro.cameraTarget.position + transformedLookatOffsetBasedOnOrientation) - lerpTargetPosition, Vector3.up);
        }

        void UpdateVerticalRotation()
        {
            // TODO -  not a great solution!
            verticalRotation = Quaternion.Slerp(verticalRotation, verticalRotation * playerSharedData_ro.verticalDelta, 10 /* rotation intensity TODO - magic number */ * Time.fixedDeltaTime);
            //verticalRotation = verticalRotation * playerSharedData_ro.verticalDelta;
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

        // MODES AND PROPERTIES
        [UnityEngine.SerializeField] ModeProperties[] modePropertiesArr;
        [UnityEngine.SerializeField] TransitionProperties transitionProperties;
        Dictionary<eMode, ModeProperties> modesMap = new Dictionary<eMode, ModeProperties>();
        private eMode? currentMode = null;
        private eMode? nextMode = null;


        // TRANSITION
        float currentTransitionTime = 0;

        // SHARED MODEL
        private PlayerAndCameraSharedModel playerSharedData_ro = null;
        private Quaternion verticalRotation = Quaternion.identity;
        
    }
}