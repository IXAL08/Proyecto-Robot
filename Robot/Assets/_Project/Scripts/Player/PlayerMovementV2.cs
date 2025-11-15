using System;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class PlayerMovementV2 : MonoBehaviour
    {
        private Rigidbody rb;
        [SerializeField] private Animator anim;
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckerPoint;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [Header("Wall Check")]
        [SerializeField] private Transform TopWallCheckPoints;
        [SerializeField] private Transform MidWallCheckPoints;
        [SerializeField] private Transform BottomWallCheckPoints;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private float wallCheckRadius = 0.3f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float drag = 60f;
        [SerializeField] private PlayerDash _playerDash;
        private Vector3 linearDrag;
        
        [SerializeField] private float rotationSpeed = 10f;
        private Quaternion targetRotation;


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
        private bool isFacingAWall;

        private float horizontalInput;
        private CapsuleCollider _playerCollider;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            _playerCollider = rb.GetComponent<CapsuleCollider>();
            jumpsLeft = extraJumps;
            jumpCooldown = jumpDelay;
            InputManager.Source.MovePlayer += HandleMovement;
            InputManager.Source.Jump += HandleJump;
            InputManager.Source.Dash += HandleDash;
        }

        private void OnDestroy()
        {
            InputManager.Source.MovePlayer -= HandleMovement;
            InputManager.Source.Jump -= HandleJump;
            InputManager.Source.Dash -= HandleDash;
        }

        private void FixedUpdate()
        {
            if (freeze) return;

            CheckGround();
            CheckisWalls();

            if (_playerDash.GetDashing()) return;
            ApplyMovement();
            ApplyBetterFalling();
        }

        private void CheckGround()
        {
            isGrounded = Physics.CheckSphere(groundCheckerPoint.position, groundCheckRadius, groundLayerMask);
            anim.SetBool("isGrounded", isGrounded);

            if (isGrounded)
                jumpsLeft = extraJumps;

            if (jumpCooldown > 0)
                jumpCooldown -= Time.deltaTime;
        }

        private void CheckisWalls()
        {
            isFacingAWall = 
                Physics.CheckSphere(TopWallCheckPoints.position, wallCheckRadius, wallLayerMask) ||
                Physics.CheckSphere(MidWallCheckPoints.position, wallCheckRadius, wallLayerMask) ||
                Physics.CheckSphere(BottomWallCheckPoints.position, wallCheckRadius, wallLayerMask);
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
                if ((isFacingAWall && horizontalInput > 0) || (isFacingAWall && horizontalInput < 0))
                {
                    velocity.x = 0;
                }
                else
                {
                    velocity.x = horizontalInput * moveSpeed;
                }

                if (horizontalInput > 0)
                {
                    targetRotation = Quaternion.Euler(0, 0, 0); 
                }
                else if (horizontalInput < 0)
                {
                    targetRotation = Quaternion.Euler(0, -180, 0);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, drag);
            }

            rb.linearVelocity = velocity;
            anim.SetFloat("X", Mathf.Abs(velocity.x));
        }


        private void HandleJump()
        {
            if (freeze || jumpCooldown > 0) return;

            if (isGrounded || jumpsLeft > 0)
            {
                var velocity = rb.linearVelocity;
                velocity.y = jumpForce;
                rb.linearVelocity = velocity;
                anim.SetTrigger("Jump");
                _playerCollider.center = new Vector3(0,0.5f,0);
                _playerCollider.height = 1.7f;
                if (!isGrounded)
                    jumpsLeft--;

                jumpCooldown = jumpDelay;
            }
        }
        
        private void HandleDash()
        {
            _playerDash.DoDash();
        }

        private void ApplyBetterFalling()
        {
            var velocity = rb.linearVelocity;

            if (velocity.y < 0)
            {
                velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                _playerCollider.center = new Vector3(0, 0, 0);
                _playerCollider.height = 1.5f;
            }
            else if (velocity.y > 0 && !InputManager.Source.IsJumpPressed)
            {
                velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
                _playerCollider.center = new Vector3(0, 0, 0);
                _playerCollider.height = 1.5f;
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
