using Robot;
using System;
using UnityEngine;

public class RangeAttack : MonoBehaviour
{
    [Header("Configuracion de disparo")]
    public float attackCooldown = 1f;
    public float bulletSpeed = 10f;
   
    [Header("Referencias")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    [Header("Dirección de Disparo")]
    public bool usePlayerFacing = true; 
    public bool autoUpdateDirection = true;
   
    [Header("Apuntado Libre")]
    public bool useFreeAim = true;
    public Transform aimPointer;
   
    private float cooldownTimer = 0f;
    private bool canShoot = true;
    private Vector3 currentFacingDirection = Vector3.right;

    private void Start()
    {
        currentFacingDirection = GetPlayerFacingDirection();
        //PlayerStatsManager.Source.OnStatsChanged += ModifyBulletSpeed;
        InputManager.Source.Attack += HandleShooting;
        UpdateFirePointDirection();
        //bulletSpeed = (PlayerStatsManager.Source.PlayerSpeedPower + 4);
    }

    private void OnDestroy()
    {
        InputManager.Source.Attack -= HandleShooting;
    }

    private void Update()
    {
        if (autoUpdateDirection && usePlayerFacing)
        {
            UpdateFacingDirection();
        }
        
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canShoot = true;
            }
        }
        
        if (useFreeAim && aimPointer != null)
        {
            UpdateAimPointer();
        }

    }

    void UpdateFacingDirection()
    {
        Vector3 newDirection = GetPlayerFacingDirection();
        if (newDirection != Vector3.zero && newDirection != currentFacingDirection)
        {
            currentFacingDirection = newDirection;
            UpdateFirePointDirection();
        }
    }

    Vector3 GetPlayerFacingDirection()
    {
        
        if (useFreeAim)
        {
            return GetMouseDirection();
        }
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput >= 0.1f)
        {
            return Vector3.right;
        } else if (horizontalInput <= -0.1f)
        {
            return Vector3.left;
        }
        return currentFacingDirection;
    }
    
    Vector3 GetMouseDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 11;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = (worldPos - transform.position).normalized;
        return direction;
    }


    void UpdateFirePointDirection()
    {
        if (firePoint != null)
        {
            firePoint.right = currentFacingDirection;
            
            float xOffset = Mathf.Abs(firePoint.localPosition.x);
            firePoint.localPosition = new Vector3(currentFacingDirection.x * xOffset, firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }
    
    void UpdateAimPointer()
    {
        aimPointer.position = transform.position + currentFacingDirection * 2f;
    }

    void HandleShooting()
    {
        if (canShoot)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (usePlayerFacing)
        {
            currentFacingDirection = GetPlayerFacingDirection();
            UpdateFirePointDirection();
        }
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
      
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetSpeed(bulletSpeed);
            bulletScript.SetDirection(currentFacingDirection);
        }
        else
        { 
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = currentFacingDirection * bulletSpeed;
            }
        }
      
        canShoot = false;
        cooldownTimer = attackCooldown;
    }

    private void ModifyBulletSpeed(float x, float y, float speed)
    {
        //bulletSpeed = (PlayerStatsManager.Source.PlayerSpeedPower + 4);
    }
   
}