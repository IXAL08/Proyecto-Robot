using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float MovementSpeed = 5f;
    public float jumpForce = 5f;
    
    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    
    [Header("Componentes")]
    public LayerMask groundLayer = 1;
    public float groundCheckDistance  = 0.1f;
        
    private Rigidbody rb;
    private Collider playerCollider;
    
    private float horizontalInput;
    private bool jumpInput;
    private bool dashInput;
    
    private bool isGrounded;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool canDash = true;
    private Vector3 dashDirection;

     void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInput();
        CheckGrounded();
        HandleDashCooldown();
        
        // Handle dash input in Update for better responsiveness
        if (dashInput && canDash && !isDashing)
        {
            StartDash();
        }
    }
    
    void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDash();
        }
        else
        {
            HandleMovement();
            HandleJump();
        }
    }
    
    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }
    
    void CheckGrounded()
    {
        // Raycast downward to check if player is grounded
        Vector3 rayStart = transform.position;
        float rayLength = playerCollider.bounds.extents.y + groundCheckDistance;
        
        isGrounded = Physics.Raycast(rayStart, Vector3.down, rayLength, groundLayer);
        
        // Optional: Visualize the ray in the scene view
        Debug.DrawRay(rayStart, Vector3.down * rayLength, isGrounded ? Color.green : Color.red);
    }
    
    void HandleMovement()
    {
        // Only move horizontally (X-axis)
        Vector3 movement = new Vector3(horizontalInput * MovementSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = movement;
    }
    
    void HandleJump()
    {
        if (jumpInput && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
        }
    }
    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        
        // Determine dash direction based on input
        dashDirection = new Vector3(horizontalInput, 0f, 0f).normalized;
        
        // If no horizontal input, dash in facing direction (right by default)
        if (dashDirection == Vector3.zero)
        {
            dashDirection = Vector3.right;
        }
        
        // Apply initial dash force
        rb.linearVelocity = dashDirection * dashForce;
        
        // Start cooldown
        dashCooldownTimer = dashCooldown;
    }
    
    void HandleDash()
    {
        dashTimer -= Time.fixedDeltaTime;
        
        if (dashTimer <= 0f)
        {
            EndDash();
        }
    }
    
    void EndDash()
    {
        isDashing = false;
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y, 0f);
    }
    
    void HandleDashCooldown()
    {
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            
            if (dashCooldownTimer <= 0f)
            {
                canDash = true;
            }
        }
    }
    
}
