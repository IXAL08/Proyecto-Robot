using System;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class PlayerMovementV2 : MonoBehaviour
    {
        private Rigidbody rb;
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckerPoint;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private Transform leftWallCheckPoints;
        [SerializeField] private Transform rightWallCheckPoints;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private float wallCheckRadius = 0.3f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float drag = 60f;
        private Vector3 linearDrag;

        [Header("Jumps")]
        [SerializeField] private int extraJumps = 1;
        private int jumpsLeft;
        [SerializeField] private float jumpDelay = 0.2f;
        private float jumpCooldown;
        
        [Header("Gravity Control")]
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f; 

        [Header("State")]
        [SerializeField] private bool freeze = false;
        private bool isGrounded;
        private bool isRWall;
        private bool isLWall;

        private float horizontalInput;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            jumpsLeft = extraJumps;
            jumpCooldown = jumpDelay;
            InputManager.Source.MovePlayer += HandleMovement;
            InputManager.Source.Jump += HandleJump;
        }

        private void OnDestroy()
        {
            InputManager.Source.MovePlayer -= HandleMovement;
            InputManager.Source.Jump -= HandleJump;
        }

        private void FixedUpdate()
        {
            if (freeze) return;

            CheckGround();
            CheckisWalls();
            
            ApplyMovement();
            ApplyBetterFalling();
        }

        private void CheckGround()
        {
            isGrounded = Physics.CheckSphere(groundCheckerPoint.position, groundCheckRadius, groundLayerMask);

            if (isGrounded)
                jumpsLeft = extraJumps;

            if (jumpCooldown > 0)
                jumpCooldown -= Time.deltaTime;
        }

        private void CheckisWalls()
        {
            isLWall = Physics.CheckSphere(leftWallCheckPoints.position, wallCheckRadius, wallLayerMask);
            isRWall = Physics.CheckSphere(rightWallCheckPoints.position, wallCheckRadius, wallLayerMask);
        }

        private void HandleMovement(float input)
        {
            if (freeze) return;
            horizontalInput = input;   
        }

        private void ApplyMovement()
        {
            var velocity = rb.linearVelocity;

            if (InputManager.Source.IsMoving)
            {
                if (isRWall && horizontalInput > 0 || isLWall && horizontalInput < 0)
                {
                    velocity.x = 0;
                }
                else
                {
                    velocity.x = horizontalInput * moveSpeed;
                }
            }else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, drag);
            }

            rb.linearVelocity = velocity;
        }

        private void HandleJump()
        {
            if (freeze || jumpCooldown > 0) return;

            if (isGrounded || jumpsLeft > 0)
            {
                var velocity = rb.linearVelocity;
                velocity.y = jumpForce;
                rb.linearVelocity = velocity;

                if (!isGrounded)
                    jumpsLeft--;

                jumpCooldown = jumpDelay;
            }
        }
        
        private void ApplyBetterFalling()
        {
            var velocity = rb.linearVelocity;

            if (velocity.y < 0)
            {
                velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (velocity.y > 0 && !InputManager.Source.IsJumpPressed)
            {
                velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }

            rb.linearVelocity = velocity;
        }


        private void OnDrawGizmosSelected()
        {
            if (groundCheckerPoint == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckerPoint.position, groundCheckRadius);
        }
    }
}
