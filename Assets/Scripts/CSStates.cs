using System;
using System.Collections.Generic;
using events;
using game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public static class CSStates
    {
        public static Dictionary<Type, AbstractState> statesRegistry = new Dictionary<Type, AbstractState>();
        
        public abstract class AbstractState
        {
            protected readonly StateContext context;

            public AbstractState(StateContext context)
            {
                this.context = context;
            }

            public abstract void EnterState();

            public abstract void HandleUpdate();

            public abstract void HandleFixedUpdate();

            public abstract AbstractState GetNextState();

            protected Vector3 GetGravity()
            {
                return context.gravity * context.config.gravityScaler;
            }
        }
        
        public class MovingState : AbstractState
        {
            private bool isJumping;
            private Vector3 movingDirection;
            private float movingSpeed;
            private float crouchBlendFactor = 0f;
            private float fwdSideMoveFactor = 0.5f;
            private float lastMoveTime;
            
            public MovingState(StateContext context) : base(context)
            {
            }

            public override void EnterState()
            {
                isJumping = false;
                crouchBlendFactor = 0f;
                fwdSideMoveFactor = 0.5f;
                lastMoveTime = Time.time;
            }

            public override void HandleUpdate()
            {
                var fwd = context.cachedTransform.forward;
                fwd.y = 0f;
        
                var side = context.cachedTransform.right;
                side.y = 0f;
                
                var targetFwdVelocity = Vector3.zero;
                var targetSideVelocity = Vector3.zero;
                
                var isMoving = false;
                var config = context.config;
                var isRunning = Input.GetKey(config.keyRun);
                var isCrouch = Input.GetKey(config.keyCrouch);
                
                if (Input.GetKey(config.keyForward))
                {
                    targetFwdVelocity += fwd;
                    isMoving = true;
                }

                if (Input.GetKey(config.keyBackward))
                {
                    targetFwdVelocity += -fwd;
                    isMoving = true;
                }

                if (Input.GetKey(config.keyStrafeLeft))
                {
                    targetSideVelocity += -side;
                    isMoving = true;
                }

                if (Input.GetKey(config.keyStrafeRight))
                {
                    targetSideVelocity += side;
                    isMoving = true;
                }

                crouchBlendFactor = Mathf.Lerp(crouchBlendFactor, isCrouch ? 1f : 0f, config.crouchLerpFactor);
                
                movingSpeed = (isRunning ? config.runSpeed : config.moveSpeed);
                movingDirection = (targetFwdVelocity + targetSideVelocity).normalized;

                if (Input.GetKey(config.keyJump))
                {
                    isJumping = true;
                }
                else
                {
                    context.animator.SetBool("isRunning", isMoving && isRunning);
                    context.animator.SetBool("isWalking", isMoving && !isRunning);
                    context.animator.SetBool("isMoving", isMoving);
                    var isBackward = isMoving && Vector3.Dot(fwd, movingDirection) < 0f;
                    context.animator.SetBool("isBackward", isBackward);
                    context.animator.SetFloat("backwardMultiplier", isBackward ? -1f : 1f);
                }
                
                if (isMoving)
                {
                    lastMoveTime = Time.time;
                }
                context.animator.SetBool("isWaitingForPlayer", !isMoving && Time.time - lastMoveTime > config.danceTimeout);

                context.animator.SetLayerWeight(1, crouchBlendFactor);
                
                var rotation = Input.GetAxis("Mouse X");
                context.cachedTransform.Rotate(Vector3.up, rotation * config.mouseRotationSpeed);
                
                UpdateFwdSideMoveFactor(config);
            }

            private void UpdateFwdSideMoveFactor(PlayerConfig config)
            {
                var currentFactor = 0.5f;
                var fwdToSideFactor = Input.GetKey(config.keyBackward) || Input.GetKey(config.keyForward)
                    ? 0.25f
                    : 0f;
                if (Input.GetKey(config.keyStrafeLeft))
                {
                    currentFactor += -0.5f + fwdToSideFactor;
                }

                if (Input.GetKey(config.keyStrafeRight))
                {
                    currentFactor += 0.5f - fwdToSideFactor;
                }

                currentFactor = Mathf.Clamp01(currentFactor);
                fwdSideMoveFactor = Mathf.Lerp(fwdSideMoveFactor, currentFactor, 0.2f);
                context.animator.SetFloat("fwdSideMoveFactor", fwdSideMoveFactor);
            }

            public override void HandleFixedUpdate()
            {
                var targetVelocity = (movingDirection * movingSpeed + GetGravity()) * Time.fixedDeltaTime;
                context.velocity = Vector3.Lerp(context.velocity, targetVelocity, context.config.acceleration);

                context.controller.Move(context.velocity);
            }

            public override AbstractState GetNextState()
            {
                if (isJumping)
                {
                    ResetMovingState();
                    return statesRegistry[typeof(JumpingState)];
                }
                if (!context.controller.isGrounded)
                {
                    ResetMovingState();
                    return statesRegistry[typeof(FallingState)];
                }

                return null;
            }

            private void ResetMovingState()
            {
                context.animator.SetBool("isRunning", false);
                context.animator.SetBool("isWalking", false);
                context.animator.SetBool("isMoving", false);
                context.animator.SetBool("isWaitingForPlayer", false);
                context.animator.SetLayerWeight(1, 0f);
            }
        }

        public class JumpingState : AbstractState
        {
            private float startTime;
            
            public JumpingState(StateContext context) : base(context)
            {
            }

            public override void EnterState()
            {
                context.animator.SetBool("isJumping", true);
                context.velocity += (context.config.jumpVelocityScaler * context.velocity 
                                     - context.config.jumpAcceleration * GetGravity())
                                    * Time.fixedDeltaTime;
                startTime = Time.time;
            }

            public override void HandleUpdate()
            {
            }

            public override void HandleFixedUpdate()
            {
                context.velocity += GetGravity() * Time.fixedDeltaTime;
                context.controller.Move(context.velocity);
            }

            public override AbstractState GetNextState()
            {
                if (Time.time - startTime < 0.3f)
                {
                    return null;
                }
                
                if (context.controller.isGrounded)
                {
                    context.animator.SetBool("isJumping", false);
                    return statesRegistry[typeof(MovingState)];
                }
                if (context.velocity.y < 0f)
                {
                    context.animator.SetBool("isJumping", false);
                    return statesRegistry[typeof(FallingState)];
                }

                return null;
            }
        }

        public class FallingState : AbstractState
        {
            public FallingState(StateContext context) : base(context)
            {
            }

            public override void EnterState()
            {
                context.animator.SetBool("isFalling", true);
                context.fallStartHeight = context.cachedTransform.position.y;
            }

            public override void HandleUpdate()
            {
            }

            public override void HandleFixedUpdate()
            {
                context.velocity += GetGravity() * Time.fixedDeltaTime;
                context.controller.Move(context.velocity);
            }

            public override AbstractState GetNextState()
            {
                if (!context.controller.isGrounded)
                {
                    return null;
                }
                context.animator.SetBool("isFalling", false);
                return statesRegistry[typeof(RestoringState)];
            }
        }

        public class RestoringState : AbstractState
        {
            private bool isHardLanding;
            
            public RestoringState(StateContext context) : base(context)
            {
            }

            public override void EnterState()
            {
                isHardLanding = context.fallStartHeight - context.cachedTransform.position.y > 3f;

                var groundVelocity = context.velocity;
                groundVelocity.y = 0f;

                if (isHardLanding)
                {
                    EventBus.Publish(new PlayerHardFallEvent());
                    
                    if (groundVelocity.magnitude > context.config.rollAfterFallVelocity)
                    {
                        context.animator.SetTrigger("fallToRoll");
                    }
                    else
                    {
                        context.animator.SetTrigger("fallHardLanding");
                    }
                    context.isRestoreCompleted = false;
                }
                else
                {
                    if (groundVelocity.magnitude > context.config.rollAfterFallVelocity)
                    {
                        context.animator.SetTrigger("fallToRoll");
                        context.isRestoreCompleted = false;
                    }
                    else
                    {
                        context.animator.SetTrigger("normalLanding");
                        context.isRestoreCompleted = true;
                    }
                }
            }

            public override void HandleUpdate()
            {
            }

            public override void HandleFixedUpdate()
            {
                context.velocity = Vector3.Lerp(context.velocity, Vector3.zero, context.config.restoreSlowdown);
                context.controller.Move(context.velocity);
            }

            public override AbstractState GetNextState()
            {
                if (!context.controller.isGrounded)
                {
                    return statesRegistry[typeof(FallingState)];
                }

                //context.isRestoreCompleted set by animation event
                if (context.isRestoreCompleted)
                {
                    return statesRegistry[typeof(MovingState)];
                }

                return null;
            }
        }
    }

    [Serializable]
    public class PlayerConfig
    {
        public KeyCode keyForward = KeyCode.W;
        public KeyCode keyBackward = KeyCode.S;
        public KeyCode keyStrafeLeft = KeyCode.A;
        public KeyCode keyStrafeRight = KeyCode.D;
        public KeyCode keyJump = KeyCode.Space;
        public KeyCode keyCrouch = KeyCode.C;
        public KeyCode keyRun = KeyCode.LeftShift;
        public float moveSpeed = 10f;
        public float runSpeed = 20f;
        public float mouseRotationSpeed = 10f;
        [Range(0, 1)]
        public float acceleration = 0.75f;
        public float rollAfterFallVelocity = 0.25f;
        public float restoreSlowdown = 0.75f;
        public float jumpAcceleration = 5f;
        public float jumpVelocityScaler = 2f;
        public float gravityScaler = 0.5f;
        public float crouchLerpFactor = 0.2f;
        public float danceTimeout = 1f;
    }

    public class StateContext
    {
        public CharacterController controller;
        public Transform cachedTransform;
        public PlayerConfig config;
        public Animator animator;
        public Vector3 gravity;

        //Current velocity
        public Vector3 velocity;
        public float fallStartHeight;
        public bool isRestoreCompleted;
    }
}