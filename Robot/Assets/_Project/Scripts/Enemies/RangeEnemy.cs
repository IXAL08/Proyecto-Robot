using System.Collections;
using Robot;
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
    public float attackAnimationDelay = 0.3f;
    
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
    
    [Header("Detección")]
    public LayerMask obstacleLayers; // Capas que bloquean la visión (paredes)
    
    [Header("Audio")]
    [SerializeField] private string DeathSFX = "Death";
    
    // Variables privadas
    public int currentHealth;
    private float attackTimer;
    private bool isRetreating = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isActive = false; // Si ha detectado al jugador
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
        if (player == null || isDead) return;
        
        // Verificar si puede ver al jugador para activarse
        if (!isActive)
        {
            if (CanSeePlayer())
            {
                isActive = true;
                Debug.Log("Enemigo activado - Jugador detectado");
            }
            else
            {
                return; // No hacer nada hasta que vea al jugador
            }
        }
        
        // Actualizar timers
        attackTimer -= Time.deltaTime;
        
        // Calcular distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Verificar si el jugador sigue siendo visible
        bool canSeePlayer = CanSeePlayer();
        
        // Si pierde de vista al jugador, desactivarse
        if (!canSeePlayer)
        {
            isActive = false;
            StopMoving();
            return;
        }
        
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
        LookAwayFromPlayer();
    }
    
    void LookAwayFromPlayer()
    {
        if (player != null)
        {
            Vector3 lookDirection = (transform.position - player.position).normalized;
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
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
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(animationRotation);
                transform.rotation = targetRotation;
            }
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayers))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public void TakeDamage(int damage, bool isMelee)
    {
        if (isDead) return;
        
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
        if (isDead) return;
        
        isDead = true;
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        AudioManager.Source.PlayOneShot(DeathSFX);
        
        canMove = false;
        canAttack = false;
        StopMoving();
        
        Invoke("DestroyEnemy", 2f);
        
        TryDropItem();
    }
    
    void DestroyEnemy()
    {
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

        // Iniciar corrutina para disparar después del delay de animación
        StartCoroutine(ShootAfterAnimation());
    }
    
    IEnumerator ShootAfterAnimation()
    {
        // Esperar el tiempo de la animación antes de disparar
        yield return new WaitForSeconds(attackAnimationDelay);
        
        // Disparar solo si sigue viendo al jugador
        if (bulletPrefab != null && CanSeePlayer() && !isDead)
        {
            ShootBullet();
        }
        
        // Resetear estado de ataque
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
    
    void ShootBullet()
    {
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
    }
    
    void UpdateAnimations()
    {
        if (animator != null && !isDead)
        {
            bool isIdle = rb.linearVelocity.magnitude < 0.1f && !isAttacking;
            bool isWalking = rb.linearVelocity.magnitude > 0.1f && !isAttacking;
            
            animator.SetBool("IsIdle", isIdle);
            animator.SetBool("IsMoving", isWalking);
            animator.SetBool("IsAttacking", isAttacking);
            //animator.SetBool("IsActive", isActive);
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
        
        // Línea de visión al jugador
        if (Application.isPlaying && player != null)
        {
            Gizmos.color = CanSeePlayer() ? (isActive ? Color.green : Color.yellow) : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
