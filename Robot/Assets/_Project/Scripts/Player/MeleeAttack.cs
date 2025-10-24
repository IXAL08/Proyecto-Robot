using System;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")] public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 0.5f;

    [Header("Dirección Automática")] public bool useFacingDirection = true;

    [Header("Referencias")] public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Efecto de Partículas")] public ParticleSystem attackParticles;

    private float cooldownTimer = 0f;
    private bool canAttack = true;
    private bool facingRight = true;

    private void Start()
    {
        UpdateAttackDirection();
    }

    void Update()
    {
        // Actualizar cooldown
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;
            }
        }

        // Detectar dirección automáticamente
        if (useFacingDirection)
        {
            DetectFacingDirection();
        }

        // Ejecutar ataque
        if (Input.GetButton("Fire1") && canAttack)
        {
            Attack();
        }
    }

    void DetectFacingDirection()
    {
        // Detectar dirección basada en input horizontal
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0.1f)
        {
            facingRight = true;
        }
        else if (horizontalInput < -0.1f)
        {
            facingRight = false;
        }

        UpdateAttackDirection();
    }

    void Attack()
    {
        canAttack = false;
        cooldownTimer = attackCooldown;

        // Reproducir efecto de partículas
        if (attackParticles != null)
        {
            Instantiate(attackParticles, attackPoint.position, Quaternion.identity);
        }

        // Detectar enemigos
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemigoMelee enemyHealth = enemy.GetComponent<EnemigoMelee>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
    
    void UpdateAttackDirection()
    {
        if (attackPoint != null)
        {
            float xOffset = facingRight ? 0.5f : -0.5f;
            attackPoint.localPosition = new Vector3(xOffset, 0, 0);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
