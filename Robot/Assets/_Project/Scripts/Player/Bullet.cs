using Robot;
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bala")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float destroyTime = 3f;

    public LayerMask collisionMask = ~0; 
    private Rigidbody rb;
    private float lifeTimer;
    private bool hasCollided = false;
    private Vector3 movementDirection;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

       
        lifeTimer = destroyTime;
        
        if (movementDirection != Vector3.zero)
        {
            transform.right = movementDirection;
        }

        if (rb != null)
        {
            rb.linearVelocity = movementDirection * speed;
        }

    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0 && !hasCollided)
        {
            DestroyBullet();
        }

    }

    private void FixedUpdate()
    {
        transform.position += movementDirection * speed * Time.deltaTime;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & collisionMask) != 0)
        {
            HandleCollision(other.collider);
        }
    }

    void HandleCollision(Collider other)
    {
        if(hasCollided) return;
        
        hasCollided = true;

        EnemigoMelee meleeHealth = other.GetComponent<EnemigoMelee>();
        if (meleeHealth != null)
        {
            meleeHealth.TakeDamage((int)damage);
        }
        
        RangeEnemy rangeHealth = other.GetComponent<RangeEnemy>();
        if (rangeHealth != null)
        {
            rangeHealth.TakeDamage((int)damage);
        }
        
        FlyingEnemy flyingHealth = other.GetComponent<FlyingEnemy>();
        if (flyingHealth != null)
        {
            flyingHealth.TakeDamage((int)damage);
        }
        
        HeavyEnemy heavyHealth = other.GetComponent<HeavyEnemy>();
        if (heavyHealth != null)
        {
            heavyHealth.TakeDamage((int)damage);
        }
        

        DestroyBullet();
    }

    public float BulletDamage()
    {
        return damage;
    }

    void DestroyBullet()
    {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
    
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    public void SetDirection(Vector3 direction)
    {
        movementDirection = direction.normalized;
    }

}