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
   
    private float cooldownTimer = 0f;
    private bool canShoot = true;
    private Vector3 currentFacingDirection = Vector3.right;

    private void Start()
    {
        currentFacingDirection = GetPlayerFacingDirection();
        PlayerStatsManager.Source.OnStatsChanged += ModifyBulletSpeed;
        UpdateFirePointDirection();
        bulletSpeed = (PlayerStatsManager.Source.PlayerSpeedPower + 4);
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

        HandleShooting();
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

    void UpdateFirePointDirection()
    {
        if (firePoint != null)
        {
            firePoint.right = currentFacingDirection;
            
            float xOffset = Mathf.Abs(firePoint.localPosition.x);
            firePoint.localPosition = new Vector3(currentFacingDirection.x * xOffset, firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }

    void HandleShooting()
    {
        if (canShoot && Input.GetButton("Fire1"))
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
        bulletSpeed = (PlayerStatsManager.Source.PlayerSpeedPower + 4);
    }
   
}