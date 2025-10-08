using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float airControl = 0.5f;
    
    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 0.5f;
    
    [Header("Configuración de Salto")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;
    
    [Header("Detección de Paredes")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    
    private Rigidbody rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        CheckGrounded();
        CheckWalls();
        HandleJump();
        HandleDash();
        UpdateTimers();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            HandleMovement();
        }
        else
        {
            HandleDashMovement();
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround);
    }

    void CheckWalls()
    {
        RaycastHit hit;
        Vector3 rayDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0).normalized;

        if (rayDirection != Vector3.zero)
        {
            isTouchingWall = Physics.Raycast(transform.position, rayDirection, out hit, wallCheckDistance, wallLayer);
        }
        else
        {
            isTouchingWall = false;
        }
    }

    float GetWallDirection()
    {
        RaycastHit hitLeft, hitRight;
        
        bool wallLeft = Physics.Raycast(transform.position, Vector3.left, out hitLeft, wallCheckDistance, wallLayer);
        bool wallRight = Physics.Raycast(transform.position, Vector3.right, out hitRight, wallCheckDistance, wallLayer);
        
        if(wallLeft) return -1;
        if(wallRight) return 1;
        
        return 0;
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (isTouchingWall && Mathf.Sign(horizontalInput) == Mathf.Sign(GetWallDirection()))
        {
            horizontalInput = 0;
        }
        float currentSpeed = moveSpeed;
        
        if (!isGrounded)
        {
            currentSpeed *= airControl;
        }
        
            Vector3 movement = new Vector3(horizontalInput * currentSpeed, rb.linearVelocity.y, 0);
            rb.linearVelocity = movement;
            
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    

    void HandleDash()
    {
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartDash();
        }
    }

    void StartDash()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            dashDirection = transform.right;
        }
        else
        {
            dashDirection = new Vector3(horizontalInput, 0,0).normalized;
        }
        
        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        
        rb.linearVelocity = dashDirection * dashForce;
        
        //rb.useGravity = false;
    }

    void HandleDashMovement()
    {
        rb.linearVelocity = dashDirection * dashForce;
    }

    void EndDash()
    {
        isDashing = false;
        //rb.useGravity = true;

        if (rb.linearVelocity.magnitude > moveSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }

    void UpdateTimers()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }

        if (!canDash && !isDashing)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
        
        if (wallCheck != null)
        {
            Gizmos.color = isTouchingWall ? Color.green : Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckDistance);
        }
    }
}