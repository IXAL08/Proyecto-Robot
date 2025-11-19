using System.Collections;
using Robot;
using UnityEngine;

public class HeavyEnemy : MonoBehaviour, IAttackable
{
    [Header("Movimiento")] 
    public float speed = 1f;
    
    [Header("Configuracion Ataque")]
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    
    [Header("Golpe Fuerte")]
    public int attack1Damage = 25;
    public float attack1Windup = 0.5f;
    
    [Header("Golpe Rapido")]
    public int attack2Damage = 15;
    public float attack2Windup = 0.2f;
    
    [Header("Referencias")]
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public GameObject attackEffect;
    
    [Header("Feedback")]
    public Color attackColor = Color.red;
    public Color normalColor = Color.green;
    public Renderer enemyRenderer;
    
    private int currentHealth;
    private float attackTimer;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool hasBeenProvoked = false;
    private bool isChasing = false;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        
        currentHealth = 120;
        
        if (player == null)
        {
            FindPlayer();
        }
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = normalColor;
        }
    }
    
    void Update()
    {
        if(!hasBeenProvoked) return;
        
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
        if (hasBeenProvoked)
        {
            if (distanceToPlayer <= attackRange && canAttack)
            {
                Attack();
            }
            else
            {
                ChasePlayer();
                isChasing = true;
            }
        }
    }
    
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        
        Vector3 movement = direction * speed;
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
        
        LookAtPlayer();
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
        attackTimer = attackCooldown;
        
        
        int attackType = Random.Range(1, 3); // 1 o 2
        
        if (attackType == 1)
        {
            StartCoroutine(Attack1Routine());
        }
        else
        {
            StartCoroutine(Attack2Routine());
        }
    }
    
    IEnumerator Attack1Routine()
    {
        Debug.Log("Ataque 1 - Golpe Fuerte!");
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.magenta;
        }
        
        
        // Windup del ataque fuerte
        yield return new WaitForSeconds(attack1Windup);
        
        // Aplicar daño si el jugador todavía está en rango
        if (IsPlayerInRange(attackRange))
        {
            ApplyDamage(attack1Damage);
        }
        
        if (attackEffect != null)
        {
            Instantiate(attackEffect, attackPoint.position, Quaternion.identity);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = normalColor;
        }
        
        isAttacking = false;
    }
    
    IEnumerator Attack2Routine()
    {
        Debug.Log("Ataque 2 - Golpe Rápido!");
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.cyan;
        }
        
        yield return new WaitForSeconds(attack2Windup);
        
        if (IsPlayerInRange(attackRange))
        {
            ApplyDamage(attack2Damage);
        }
        
        yield return new WaitForSeconds(0.1f);
        
        if (IsPlayerInRange(attackRange))
        {
            ApplyDamage(attack2Damage);
        }
        
        if (attackEffect != null)
        {
            Instantiate(attackEffect, attackPoint.position, Quaternion.identity);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = normalColor;
        }
        
        isAttacking = false;
    }
    
    void ApplyDamage(int damageAmount)
    {
        // Detectar jugador en el rango de ataque
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        
        foreach (Collider playerCollider in hitPlayers)
        {
            PlayerStatsManager.Source.TakeDamage(damageAmount);
        }
    }
    
    public void TakeDamage(int damage)
    {
        // Provocar al enemigo cuando recibe daño por primera vez
        if (!hasBeenProvoked)
        {
            hasBeenProvoked = true;
        }
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        
        // Feedback visual de daño
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
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
                enemyRenderer.material.color = isAttacking ? attackColor : normalColor;
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
    
    void OnDrawGizmosSelected()
    {
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
        
        // Estado actual
        if (Application.isPlaying)
        {
            Gizmos.color = hasBeenProvoked ? (isAttacking ? Color.magenta : Color.green) : Color.gray;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
        }
    }
}
