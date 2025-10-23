using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 3f;
    public float retreatDistance = 3f;
    public float stoppingDistance = 5f;
    
    [Header("Configuración de Ataque")]
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
    
    // Variables privadas
    private float attackTimer;
    private bool isRetreating = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Buscar al jugador automáticamente
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
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
    }
    
    void RetreatFromPlayer()
    {
        if (!canMove) return;
        
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 movement = direction * moveSpeed;
        
        // Mover alejándose
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
        
        // Rotar hacia el jugador (pero moverse en dirección opuesta)
        LookAtPlayer();
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
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    void Attack()
    {
        if (bulletPrefab == null) return;

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
