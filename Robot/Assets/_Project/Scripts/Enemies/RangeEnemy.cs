using System.Collections;
using UnityEngine;

public class RangeEnemy : MonoBehaviour, IAttackable
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 3f;
    public float retreatDistance = 3f;
    public float stoppingDistance = 5f;

    [Header("Configuración de Ataque")]
    public int maxHealth = 20;
    public float attackRange = 7f;
    public float attackCooldown = 2f;
    public int damage = 10;
    public float bulletSpeed = 8f;
    
    [Header("Referencias")]
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    [Header("Estados")]
    public bool canMove = true;
    public bool canAttack = true;
    
    [Header("Sistema de Drops")]
    public GameObject[] drops;
    public float dropChance = 0.5f;
    
    [Header("Efectos Visuales")]
    public Color normalColor = Color.white;
    public Renderer EnemyRenderer;
    
    // Variables privadas
    public int currentHealth;
    private float attackTimer;
    private bool isRetreating = false;
    private bool isAttacking = false;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 animationRotation = new Vector3(0, 90, 0);
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        currentHealth = maxHealth;
        
        transform.rotation = Quaternion.Euler(animationRotation);
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Actualizar timers
        attackTimer -= Time.deltaTime;
        
        // Calcular distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Determinar comportamiento según distancia
        if (distanceToPlayer <= retreatDistance)
        {
            // Muy cerca - RETROCEDER
            RetreatFromPlayer();
            isRetreating = true;
        }
        else if (distanceToPlayer <= attackRange && distanceToPlayer > retreatDistance)
        {
            // Distancia de ataque - DISPARAR
            StopMoving();
            isRetreating = false;
            
            // Atacar si puede
            if (canAttack && attackTimer <= 0)
            {
                Attack();
            }
            
            // Rotar hacia el jugador
            LookAtPlayer();
        }

        UpdateAnimations();
    }
    
    void RetreatFromPlayer()
    {
        if (!canMove) return;
        
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 movement = direction * moveSpeed;
        
        // Mover alejándose
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, 0);
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
        LookAwayFromPlayer();
    }
    
    void LookAwayFromPlayer()
    {
        if (player != null)
        {
            Vector3 lookDirection = (transform.position - player.position).normalized;
            lookDirection.y = 0; // Mantener rotación solo en eje Y
            
            if (lookDirection != Vector3.zero)
            {
                // Aplicar la rotación de huida más la corrección para las animaciones
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(animationRotation);
                transform.rotation = targetRotation;
            }
        }
    }
    
    void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    
    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0; // Mantener rotación solo en eje Y
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(animationRotation);
                transform.rotation = targetRotation;
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(DamageFlash());
       
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    IEnumerator DamageFlash()
    {
        if (EnemyRenderer != null)
        {
            Color originalColor = EnemyRenderer.material.color;
            EnemyRenderer.material.color = Color.red;
            
            yield return new WaitForSeconds(0.1f);
            
            EnemyRenderer.material.color = originalColor;
        }
    }

    public void Die()
    {
        TryDropItem();
        Destroy(gameObject);
    }
    
    void TryDropItem()
    {
        if (drops != null && drops.Length > 0 && Random.value <= dropChance)
        {
            GameObject itemToDrop = drops[Random.Range(0, drops.Length)];

            if (itemToDrop != null)
            {
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
                Debug.Log("Enemigo soltó un item: " + itemToDrop.name);
            }
        }
    }

    void Attack()
    {
        if (bulletPrefab == null) return;

        isAttacking = true;
        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Reiniciar cooldown
        attackTimer = attackCooldown;

        // Instanciar proyectil
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Configurar proyectil
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetSpeed(bulletSpeed);
            bulletScript.SetDamage(damage);

            // Dirección hacia el jugador
            Vector3 attackDirection = (player.position - firePoint.position).normalized;
            bulletScript.SetDirection(attackDirection);
        }
        else
        {
            // Configuración alternativa con Rigidbody
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 attackDirection = (player.position - firePoint.position).normalized;
                rb.linearVelocity = attackDirection * bulletSpeed;
            }
        }
        Invoke("ResetAttack", 0.5f);
    }
    
    void ResetAttack()
    {
        isAttacking = false;
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            bool isIdle = rb.linearVelocity.magnitude < 0.1f && !isAttacking;
            
            // Caminata: cuando se está moviendo
            bool isWalking = rb.linearVelocity.magnitude > 0.1f && !isAttacking;
            
            animator.SetBool("IsIdle", isIdle);
            animator.SetBool("IsWalking", isWalking);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }
    void OnDrawGizmosSelected()
    {
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Distancia de retroceso
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
        
        // Distancia de detención
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        
        // Dirección actual
        if (Application.isPlaying)
        {
            Gizmos.color = isRetreating ? Color.magenta : Color.green;
            Vector3 direction = isRetreating ? 
                (transform.position - (player != null ? player.position : transform.position)).normalized :
                ((player != null ? player.position : transform.position + transform.forward) - transform.position).normalized;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
}
