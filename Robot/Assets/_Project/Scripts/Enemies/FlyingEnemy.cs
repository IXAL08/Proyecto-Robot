using System.Collections;
using Robot;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float flySpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float returnDelay = 2f;
    public float minDistanceFromPlayer = 2f;

    [Header("Ataque")]
    public int damage = 10;
    public float cooldown = 2f;
    public float attackDuration = 0.5f;

    [Header("Referencias")]
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public GameObject attackEffect;
    
    [Header("Feedback")]
    public Color attackColor = Color.red;
    public Color idleColor = Color.green;
    public Renderer enemyRenderer;
    
    private int currentHealth;
    private float attackTimer;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool playerDetected = false;
    private Vector3 startPos;
    private bool isChasing = false;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        
        startPos = transform.position;
        currentHealth = 50;

        if (player == null)
        {
            FindPlayer();
        }

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = idleColor;
        }

        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            canAttack = true;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
        }

        if (isReturning)
        {
            ReturnToStart();
        }
        else if (playerDetected && !isReturning)
        {
            if (distanceToPlayer <= attackRange && canAttack)
            {
                Attack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer();
                isChasing = true;
            }

        }
    }

    void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        void ChasePlayer()
        {
            Vector3 direction = (player.position - transform.position).normalized;
            
            float currentDistance = Vector3.Distance(transform.position, player.position);
            
            Vector3 movement = Vector3.zero;
        
            if (currentDistance > minDistanceFromPlayer)
            {
                movement = direction * flySpeed;
            }
            else if (currentDistance < minDistanceFromPlayer - 0.5f)
            {
                movement = -direction * flySpeed;
            }
        
            if (rb != null)
            {
                rb.linearVelocity = movement;
            }
            else
            {
                transform.position += movement * Time.deltaTime;
            }
            
            LookAtPlayer();
        }

        void ReturnToStart()
        {
            Vector3 direction = (startPos - transform.position).normalized;

            Vector3 movement = direction * flySpeed;
            if (rb != null)
            {
                rb.linearVelocity = movement;
            }
            else
            {
                transform.position += movement * Time.deltaTime;
            }

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (Vector3.Distance(transform.position, startPos) < 0.5f)
            {
                isReturning = false;
                playerDetected = false;
                isChasing = false;


                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                }
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
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }

        void Attack()
        {
            if (!canAttack || isAttacking) return;
        
            isAttacking = true;
            canAttack = false;
            attackTimer = cooldown;
            
            StartCoroutine(AttackRoutine());
        }

        IEnumerator AttackRoutine()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
        
            
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = attackColor;
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
            
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = idleColor;
            }
        
            isAttacking = false;
            
            StartCoroutine(StartReturn());
        }
        
        IEnumerator StartReturn()
        {
            
            yield return new WaitForSeconds(returnDelay);
            
            isReturning = true;
            isChasing = false;
        }

        void ApplyDamage()
        {

            Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
            
            foreach (Collider playerCollider in hitPlayers)
            {
                PlayerStatsManager.Source.TakeDamage(damage);
            }
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, 50);
            
            StartCoroutine(DamageFlash());
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        IEnumerator DamageFlash()
        {
            if (enemyRenderer != null)
            {
                Color originalColor = enemyRenderer.material.color;
                enemyRenderer.material.color = Color.white;
            
                yield return new WaitForSeconds(0.1f);
            
                if (enemyRenderer != null)
                {
                    enemyRenderer.material.color = isAttacking ? attackColor : idleColor;
                }
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }
        
        bool IsPlayerInRange(float range)
        {
            if (player == null) return false;
            return Vector3.Distance(transform.position, player.position) <= range;
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
                Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        
            // Posición inicial (solo en Play mode)
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(startPos, 0.5f);
                Gizmos.DrawLine(transform.position, startPos);
            }
        
            // Estado actual
            if (Application.isPlaying)
            {
                Gizmos.color = isReturning ? Color.cyan : (isChasing ? Color.green : Color.white);
                Gizmos.DrawWireSphere(transform.position, 0.3f);
            }
        }
}
