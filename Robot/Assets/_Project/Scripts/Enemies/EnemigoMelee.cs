using System.Collections;
using Robot;
using UnityEngine;

public class EnemigoMelee : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public float patrolDistance = 5f;
    public float waitTimeAtPatrol = 1f;
    
    [Header("Combate")]
    public int maxHealth = 60;
    public float attackDamage = 5;
    public float attackCooldown = 2f;
    public float detectionRange = 4f;
    public float attackRange = 1.5f;

    [Header("Referencias")]
    public Transform attackPoint;
    public LayerMask playerMask;
    public GameObject attackEffect;
    
    [Header("Sistema de Drops")]
    public GameObject[] drops;
    public float dropChance = 0.5f;
    
    [Header("Efectos Visuales")]
    public Color attackColor = Color.red;
    public Color normalColor = Color.white;
    public Renderer EnemyRenderer;
    
    public int currentHealth;
    private bool canAttack;
    private float cooldownTimer;
    private bool isAttacking;
    private bool isChasing;
    private Vector3 startPosition;
    private Vector3 rightPatrolPoint;
    private Vector3 leftPatrolPoint;
    private bool movingRight = true;
    private bool isWaiting = false;
    private Rigidbody rb;
    private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnemyRenderer = GetComponentInChildren<Renderer>();
        
        currentHealth = maxHealth;
        
        startPosition = transform.position;
        leftPatrolPoint = startPosition + Vector3.left * patrolDistance;
        rightPatrolPoint = startPosition + Vector3.right * patrolDistance;
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (EnemyRenderer != null)
        {
            EnemyRenderer.material.color = normalColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        if (player == null)
        {
            FindPlayer();
        }

        if (player != null && IsPlayerInRange(detectionRange))
        {
            ChaseAndAttack();
        }
        else
        {
            Patrol();
        }
    }

    void UpdateTimers()
    {
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;
            }
        }
    }

    void Patrol()
    {
        isChasing = false;

        if (isWaiting) return;
        
        Vector3 targetPoint = movingRight ? rightPatrolPoint : leftPatrolPoint;
        
        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 movement = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, 0);
        rb.linearVelocity = movement;

        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    void ChaseAndAttack()
    {
        isChasing = true;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            Attack();
        }
        else if (distanceToPlayer > attackRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 movement = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, 0);
            rb.linearVelocity = movement;
            
            if (direction.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        } 
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    
    void Attack()
    {
        if (!canAttack || isAttacking) return;
        
        isAttacking = true;
        canAttack = false;
        cooldownTimer = attackCooldown;
        
        // Iniciar corrutina de ataque
        StartCoroutine(AttackRoutine());
    }
    
    IEnumerator AttackRoutine()
    {
        // Feedback visual de preparación
        if (EnemyRenderer != null)
        {
            EnemyRenderer.material.color = attackColor;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        if (IsPlayerInRange(attackRange))
        {
            ApplyDamage();
        }
        
        if (attackEffect != null)
        {
            Instantiate(attackEffect, attackPoint.position, Quaternion.identity);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (EnemyRenderer != null)
        {
            EnemyRenderer.material.color = normalColor;
        }
        
        isAttacking = false;
    }
    
    void ApplyDamage()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange * 0.5f, playerMask);
        
        foreach (Collider playerCollider in hitPlayers)
        {
                PlayerStatsManager.Source.TakeDamage(attackDamage);
        }
    }
    
    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.linearVelocity = Vector3.zero;
        
        yield return new WaitForSeconds(waitTimeAtPatrol);
        
        movingRight = !movingRight;
        isWaiting = false;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Feedback visual de daño
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
            EnemyRenderer.material.color = Color.white;
            
            yield return new WaitForSeconds(0.1f);
            
            if (EnemyRenderer != null)
            {
                EnemyRenderer.material.color = isAttacking ? attackColor : normalColor;
            }
        }
    }
    
    void Die()
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

    bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    void OnDrawGizmosSelected()
    {
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rango de ataque
        Gizmos.color = Color.red;
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange * 0.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        
        // Puntos de patrulla (solo en Play mode)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(leftPatrolPoint, 0.3f);
            Gizmos.DrawWireSphere(rightPatrolPoint, 0.3f);
            Gizmos.DrawLine(leftPatrolPoint, rightPatrolPoint);
        }
        else
        {
            // En el editor, mostrar patrulla basada en posición actual
            Gizmos.color = Color.cyan;
            Vector3 leftPoint = transform.position + Vector3.left * patrolDistance;
            Vector3 rightPoint = transform.position + Vector3.right * patrolDistance;
            Gizmos.DrawWireSphere(leftPoint, 0.3f);
            Gizmos.DrawWireSphere(rightPoint, 0.3f);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }
    }
}
