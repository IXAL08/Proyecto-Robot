using Robot;
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

    private bool isActivate = false;
    private float cooldownTimer = 0f;
    private bool canAttack = true;
    private bool facingRight = true;

    private void Start()
    {
        InputManager.Source.Attack += OnButtonPressed;
        PlayerStatsManager.Source.OnMeleeChipActivation += ActivateMeleeAttack;

    }

    private void OnDisable()
    {
        InputManager.Source.Attack -= OnButtonPressed;
        PlayerStatsManager.Source.OnMeleeChipActivation -= ActivateMeleeAttack;
    }

    void Update()
    {
        if (!isActivate) return;

        // Actualizar cooldown
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;
            }
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

    private void OnButtonPressed()
    {
        if (!isActivate) return;

        if (canAttack)
        { 
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        cooldownTimer = attackCooldown;
        
        if (attackParticles != null)
        {
            Instantiate(attackParticles, attackPoint.position, Quaternion.identity);
        }

        // Detectar enemigos
        Collider[] hittedItems = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider item in hittedItems)
        {
            IAttackable attackable = item.GetComponent<IAttackable>();
            if (attackable != null)
            {
                attackable.TakeDamage(damage);
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

    private void ActivateMeleeAttack(bool value)
    {
        isActivate = value;
    }
}
