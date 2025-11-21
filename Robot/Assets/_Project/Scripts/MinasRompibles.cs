using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinasRompibles : MonoBehaviour, IAttackable
{
    [Header("Vida")] 
    public int maxHealth = 3;
    public bool canBeShot = true;
    public bool canBeHit = true;

    [Header("Recompensas")]
    public List<GameObject> materialPrefabs;
    public int maxMaterials = 50;
    public int minMaterials = 20;
    public float dropForce = 3f;
    public Vector3 dropArea = new Vector3(1, 0, 0);
    
    private int currentHealth;
    private bool isBroken = false;
    private Material originalMaterial;

    private void Start()
    {
        currentHealth = maxHealth;
        
    }

    public void TakeDamage(int damage = 1, Vector3 hitDirection = default(Vector3))
    {
        if (isBroken) return;
      
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    

    void BreakObject(Vector3 hitDirection = default(Vector3))
    {
        if(isBroken) return;
        
        isBroken = true;

        DropMaterials(hitDirection);
        
        Destroy(gameObject, 0.1f);
    }

    void DropMaterials(Vector3 hitDirection)
    {
        if(materialPrefabs == null || materialPrefabs.Count == 0) return;
        
        int materialCount = Random.Range(minMaterials, maxMaterials + 1);

        for (int i = 0; i < materialCount; i++)
        {
            GameObject materialPrefab = materialPrefabs[Random.Range(0, materialPrefabs.Count)];

            if (materialPrefab != null)
            {
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-dropArea.x, dropArea.x), Random.Range(-dropArea.y, dropArea.y), 0);
                
                GameObject material = Instantiate(materialPrefab, spawnPosition, Quaternion.identity);
                
                Rigidbody rb = material.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;

                    if (hitDirection != Vector3.zero)
                    {
                        forceDirection = (forceDirection + hitDirection.normalized * 0.5f).normalized;
                    }
                    
                    rb.AddForce(forceDirection * dropForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void TakeDamage(int damage, bool isMelee)
    {
        TakeDamage(damage, Vector3.zero);
    }

    public void Die()
    {
        BreakObject();
    }
}
