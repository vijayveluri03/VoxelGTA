using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    public class CharacterCommonBehaviour
    {
        public CharacterCommonBehaviour(CharacterController owner)
        {
            this.owner = owner;
            movement = new Movement(owner);
            animation = new Animation(owner);
            orientation = new Orientation(owner);
            sharedData = new SharedData();
        }

        public class Movement
        {
            public Movement (CharacterController owner ) { this.owner = owner; }

            #region movement
            public static Vector3 MoveWithForce_WithFixedUpdate(Vector3 pos, Vector3 force)
            {
                pos += (force * Time.fixedDeltaTime);
                return pos;
            }

            public static Vector3 MoveBy(Vector3 pos, Vector3 delta)
            {
                return pos + delta;
            }

            public void FixedUpdate()
            {
                DoGroundedCheck();
            }

            public Vector3 GetPositionBasedOnVelocities_WithFixedUpdate(Vector3 position)
            {
                if (!IsGrounded)
                    VerticalVelocity += inputs.gravity * Time.fixedDeltaTime;

                position = MoveWithForce_WithFixedUpdate(position, HorizontalVelocity);
                position = MoveWithForce_WithFixedUpdate(position, Vector3.up * VerticalVelocity);

                if (verticalCorrectionDelta != 0)
                {
                    position = MoveBy(position, new Vector3(0, verticalCorrectionDelta, 0));
                    Core.QLogger.LogInfo("Readjustment : " + verticalCorrectionDelta);
                }
                return position;
            }

            public void SetVerticalVelocity(float velocity)
            {
                VerticalVelocity = velocity;
            }
            public void SetHorizontalVelocity(Vector3 velocity)
            {
                HorizontalVelocity = velocity;
            }

            public bool IsFalling()
            {
                if (IsGrounded)
                    return false;
                return VerticalVelocity < 0;
            }

            public float GetHorizontalVelocity()
            {
                return HorizontalVelocity.magnitude;
            }
            public float GetVerticalVelocity()
            {
                return VerticalVelocity;
            }

            public bool AreVelocitiesZero()
            {
                if ( Mathf.Abs( VerticalVelocity ) > 0.01 || HorizontalVelocity.sqrMagnitude > 0.01)
                    return false;
                return true;
            }

            public float VerticalVelocity { get; private set; }
            public Vector3 HorizontalVelocity { get; private set; }

            #endregion

            #region Falling and Grounding 

            private void DoGroundedCheck()
            {
                IsGrounded = false;
                verticalCorrectionDelta = 0;

                // Is going up 
                // todo - Do a roof check 
                if (VerticalVelocity > 0)
                {

                }
                // Is going down
                else
                {
                    if (Physics.Raycast(inputs.centerTransform.position, -transform.up, out hitInfo, inputs.exactDistanceToBottom * 3, inputs.ground.value))
                    {
                        float hitDistanceFromCenter = Vector3.Distance(hitInfo.point, inputs.centerTransform.position);
                        if (hitDistanceFromCenter <= inputs.acceptableDistanceToBottom)
                        {
                            IsGrounded = true;
                            VerticalVelocity = 0;
                        }
                        if (hitDistanceFromCenter < inputs.exactDistanceToBottom)
                        {
                            // pushing it up
                            verticalCorrectionDelta = inputs.acceptableDistanceToBottom - inputs.exactDistanceToBottom;
                        }
                    }
                }
            }

            public bool IsGrounded { get; private set; }

            private Transform transform { get { return owner.GameObject.transform; } }
            private RaycastHit hitInfo = new RaycastHit();
            private float verticalCorrectionDelta = 0;

            #endregion

            private CharacterInputs inputs { get { return owner.Inputs; } }

            private CharacterController owner;
        }

        public class Animation
        {
            public Animation( CharacterController owner) { this.owner = owner; }

            public void SetAnimation(eAnimationStates state)
            {
                return; // not all animations are present at this moment 
                if (previousState != state)
                    animator.SetInteger("AnimationState", (int)state);

                previousState = state;
            }

            public Animator animator { get { return owner.Animator; } }
            private eAnimationStates previousState = 0;
            private CharacterController owner = null;
        }

        public class Orientation
        {
            public Orientation( CharacterController owner ) { this.owner = owner; }

            public void FixedUpdate()
            {
                ProcessMouseMovement_WithFixedUpdate();
            }


            // @TODO - mouse is hardcoded here. 
            private void ProcessMouseMovement_WithFixedUpdate()
            {
                // Hardcoded for now 
                float mouseInputX = Input.GetAxis("Mouse X") * inputs.mouseIntensity;
                //Core.QLogger.LogInfo("Mouse input" + mouseInputX);
                Quaternion a = Quaternion.AngleAxis(mouseInputX * 360 * Time.fixedDeltaTime, Vector3.up);

                float mouseInputY = Input.GetAxis("Mouse Y") * inputs.mouseIntensity;
                //Core.QLogger.LogInfo("Mouse input" + mouseInputY);
                Quaternion b = Quaternion.AngleAxis(mouseInputY * 360 * Time.fixedDeltaTime, Vector3.right);
                /// todo : temp.!-- impliment y axis as well 
                b = Quaternion.identity;

                orientationDelta = a * b;

                // Quaternion q = transform.rotation;
                // Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
                // slerpToPseudo = q * deltaAngle;

            }

            public Quaternion GetRotationBasedOnInputs_WithFixedUpdate(Quaternion rot)
            {
                return Quaternion.Slerp(rot, rot * orientationDelta, inputs.rotationSlerpSpeed * Time.fixedDeltaTime);
            }


            private Quaternion orientationDelta = Quaternion.identity;

            private CharacterController owner;
            private CharacterInputs inputs { get { return owner.Inputs; } }
        }


        public class SharedData
        {
            public float forwardRawVelocity = 0;
            public float strafeRawVelocity = 0;

            public Vector3 PositionPseudo;
            public Quaternion RotationPseudo;
        }

        public Movement movement = null;
        public Animation animation = null;
        public Orientation orientation = null;
        public SharedData sharedData = null;

        private CharacterController owner;
        public CharacterInputs inputs { get { return owner.Inputs; } }
    }

    public class FSMCStateWithCharacterSharedContext : Core.FSMCState<CharacterController, CharacterController.eStates>
    {
        public Transform Transform { get { return Owner.GameObject.transform; } }
        public GameObject GameObject { get { return Owner.GameObject; } }
        public CharacterInputs Inputs { get { return Owner.Inputs; } }
        public CharacterCommonBehaviour CommonBehaviour { get { return Owner.CommonState; } }
        public Animator Animator { get { return Owner.Animator; } }
        public CharacterCommonBehaviour.SharedData SharedData {  get { return CommonBehaviour.sharedData; } }
    }

    public class CharacterBaseState : FSMCStateWithCharacterSharedContext
    {


        private AnimationClip runAnimationClip = null;
        private string previousPlayedAnimationName = "";

        // private int groundLayerMask;

        // Start is called before the first frame update
        public override void OnEnter(params object[] arguments)
        {
            base.OnEnter(arguments);

            Core.QLogger.Assert(Inputs.acceptableDistanceToBottom > Inputs.exactDistanceToBottom && Inputs.exactDistanceToBottom > 0);
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Core.QLogger.LogToGUI(3, "Velocities : " + CommonBehaviour.movement.GetHorizontalVelocity() + " , " + CommonBehaviour.movement.GetVerticalVelocity());
        }

        public override void Update()
        {
            base.Update();
        }

        protected void CachePosition()
        {
            SharedData.PositionPseudo = Transform.position;
            SharedData.RotationPseudo = Transform.rotation;

        }
        protected void ApplyPosition()
        {
            Transform.position = SharedData.PositionPseudo;
            Transform.rotation = SharedData.RotationPseudo;
        }
    }

    public class CharacterIdle : CharacterBaseState
    {
        public override void OnEnter(params object[] arguments)
        {
            base.OnEnter(arguments);
            CommonBehaviour.movement.SetHorizontalVelocity(Vector3.zero);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            CachePosition();
            CommonBehaviour.movement.FixedUpdate();
            CommonBehaviour.orientation.FixedUpdate();

            if (!CommonBehaviour.movement.IsGrounded)
            {
                SetState(CharacterController.eStates.Falling);
                return;
            }

            // Hardcoded for now 
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (v != 0 || h != 0 || !CommonBehaviour.movement.AreVelocitiesZero())
            {
                SetState(CharacterController.eStates.Walk, v, h);
                return;
            }

            SharedData.PositionPseudo = CommonBehaviour.movement.GetPositionBasedOnVelocities_WithFixedUpdate(SharedData.PositionPseudo);
            SharedData.RotationPseudo = CommonBehaviour.orientation.GetRotationBasedOnInputs_WithFixedUpdate(SharedData.RotationPseudo);
            ApplyPosition();


            if (Input.GetKey(KeyCode.Space))
            {
                SetState(CharacterController.eStates.Jump);
            }
        }
        public override void Notify(params object[] arguments)
        {
            base.Notify(arguments);
        }
    }

    public class CharacterWalk : CharacterBaseState
    {
        public override void OnEnter(params object[] arguments)
        {
            base.OnEnter(arguments);
            
            if (arguments != null && arguments.Length == 2)
            {
                // Move this based on previous values in previous frame 
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            CachePosition();

            CommonBehaviour.movement.FixedUpdate();
            CommonBehaviour.orientation.FixedUpdate();

            if (!CommonBehaviour.movement.IsGrounded)
            {
                SetState(CharacterController.eStates.Falling);
                return;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                SetState(CharacterController.eStates.Jump);
            }

            // Hardcoded for now 
            float forwardIntensity = Input.GetAxisRaw("Horizontal");
            float sideIntensity = Input.GetAxisRaw("Vertical");

            Move_WithFixedUpdate(sideIntensity, forwardIntensity);

            SharedData.PositionPseudo = CommonBehaviour.movement.GetPositionBasedOnVelocities_WithFixedUpdate(SharedData.PositionPseudo);
            SharedData.RotationPseudo = CommonBehaviour.orientation.GetRotationBasedOnInputs_WithFixedUpdate(SharedData.RotationPseudo);

            ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );

            {
                if (CommonBehaviour.movement.AreVelocitiesZero())
                {
                    SetState(CharacterController.eStates.Idle);
                }
            }

            //transform.rotation = Quaternion.Slerp ( transform.rotation, slerpToPseudo, inputs.rotationSpeed * Time.fixedDeltaTime );

        }

        private void Move_WithFixedUpdate(float forwardIntensity, float sideIntensity)
        {
            //CommonBehaviour.movement.SetHorizontalVelocity( Vector3.zero );

            // FORWARD FORCE
            if (forwardIntensity != 0)
            {
                SharedData.forwardRawVelocity += Inputs.acceleration * Time.fixedDeltaTime * Mathf.Sign(forwardIntensity);
                SharedData.forwardRawVelocity = Mathf.Min(SharedData.forwardRawVelocity, Inputs.movementSpeedMax);
            }
            if (sideIntensity != 0)
            {
                SharedData.strafeRawVelocity += Inputs.strafeAcceleration * Time.fixedDeltaTime * Mathf.Sign(sideIntensity);
                SharedData.strafeRawVelocity = Mathf.Min(SharedData.strafeRawVelocity, Inputs.strafeSpeedMax);
            }

            // BACKWARD DRAG
            if (Inputs.dragHorizontal != 0)
            {
                SharedData.forwardRawVelocity /= 1 + Inputs.dragHorizontal * Time.fixedDeltaTime;
            }
            if (Inputs.strafeDragHorizontal != 0)
            {
                SharedData.strafeRawVelocity /= 1 + Inputs.strafeDragHorizontal * Time.fixedDeltaTime;
            }

            CommonBehaviour.movement.SetHorizontalVelocity( (Transform.forward * SharedData.forwardRawVelocity) + (Transform.right * SharedData.strafeRawVelocity));

            if (sideIntensity != 0)
            {
                // Quaternion q = transform.rotation;
                // Quaternion deltaAngle = Quaternion.Euler ( 0, Mathf.Sign(h) * 30, 0 );
                // slerpToPseudo = q * deltaAngle;
            }
        }

        public override void Notify(params object[] arguments)
        {
            base.Notify(arguments);
        }

        
    }

    public class CharacterJump : CharacterBaseState
    {
        public override void OnEnter(params object[] arguments)
        {
            base.OnEnter(arguments);

            jumpCountFromGround = 0;

            CommonBehaviour.animation.SetAnimation(eAnimationStates.Jump);
            CommonBehaviour.movement.SetVerticalVelocity(CommonBehaviour.movement.VerticalVelocity + JumpIfPossibleAndReturnDetaForce(Inputs.jumpForceValue));
        }

        public override void OnExit()
        {
            base.OnExit();
            jumpCountFromGround = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            CachePosition();

            CommonBehaviour.movement.FixedUpdate();
            CommonBehaviour.orientation.FixedUpdate();

            if (CommonBehaviour.movement.IsGrounded)
            {
                if (CommonBehaviour.movement.AreVelocitiesZero())
                    SetState(CharacterController.eStates.Idle);
                else
                    SetState(CharacterController.eStates.Walk);
            }

            // double jump 
            {
                //if (Input.GetKey(KeyCode.Space))
                //{
                //    CommonBehaviour.movement.SetVerticalVelocity(CommonBehaviour.movement.VerticalVelocity + JumpIfPossibleAndReturnDetaForce(Inputs.jumpForceValue));
                //    CommonBehaviour.animation.SetAnimation(eAnimationStates.Jump);
                //}
            }

            if (CommonBehaviour.movement.IsFalling())
                SetState(CharacterController.eStates.Falling);

            SharedData.PositionPseudo = CommonBehaviour.movement.GetPositionBasedOnVelocities_WithFixedUpdate(SharedData.PositionPseudo);
            SharedData.RotationPseudo = CommonBehaviour.orientation.GetRotationBasedOnInputs_WithFixedUpdate(SharedData.RotationPseudo);

            ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );
        }


        public override void Notify(params object[] arguments)
        {
            base.Notify(arguments);
        }

        float JumpIfPossibleAndReturnDetaForce(float force)
        {
            if (jumpCountFromGround > 2)
                return 0;

            jumpCountFromGround++;
            return jumpCountFromGround == 1 ? force : force * 1.3f;// hack, but its fine for now
        }

        private int jumpCountFromGround = 0;
    }

    public class CharacterFalling : CharacterBaseState
    {
        public override void OnEnter(params object[] arguments)
        {
            base.OnEnter(arguments);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            CachePosition();

            CommonBehaviour.movement.FixedUpdate();
            CommonBehaviour.orientation.FixedUpdate();

            if (CommonBehaviour.movement.IsGrounded)
            {
                if (CommonBehaviour.movement.AreVelocitiesZero())
                    SetState(CharacterController.eStates.Idle);
                else
                    SetState(CharacterController.eStates.Walk);
            }

            SharedData.PositionPseudo = CommonBehaviour.movement.GetPositionBasedOnVelocities_WithFixedUpdate(SharedData.PositionPseudo);
            SharedData.RotationPseudo = CommonBehaviour.orientation.GetRotationBasedOnInputs_WithFixedUpdate(SharedData.RotationPseudo);

            ApplyPosition();// Vector3.Lerp ( transform.position, PositionPseudo, 0.75f  );

        }

        public override void Notify(params object[] arguments)
        {
            base.Notify(arguments);
        }
    }
}