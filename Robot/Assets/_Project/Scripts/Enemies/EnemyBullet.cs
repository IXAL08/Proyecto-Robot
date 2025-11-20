using Robot;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bala")]
    public float speed;
    public float damage;
    public float destroyTime = 3f;

    public LayerMask collisionMask = ~0; 
    private Rigidbody rb;
    private float lifeTimer;
    private bool hasCollided = false;
    private Vector3 movementDirection = Vector3.right;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
       
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
        if (rb == null)
        {
            transform.position += movementDirection * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & collisionMask) != 0)
        {
            HandleCollision(other.collider);
        }

        if (other.collider.gameObject.layer == 6 || other.collider.gameObject.layer == 3)
        {
            DestroyBullet();
        }
    }
    
    void HandleCollision(Collider other)
    {
        if(hasCollided) return;
        
        hasCollided = true;

        PlayerStatsManager.Source.TakeDamage(damage);
        

        DestroyBullet();
    }

    void DestroyBullet()
    {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
    }
    
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    
    public void SetDirection(Vector3 direction)
    {
        movementDirection = direction.normalized;
        
        
        if (movementDirection != Vector3.zero)
        {
            transform.right = movementDirection;
        }
        
      
        if (rb != null)
        {
            rb.linearVelocity = movementDirection * speed;
        }
    }

}
