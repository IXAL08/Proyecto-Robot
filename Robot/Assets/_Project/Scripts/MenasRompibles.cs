using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenasRompibles : MonoBehaviour
{
    [Header("Vida")] 
    public int maxHits = 3;
    public bool canBeShot = true;
    public bool canBeHit = true;

    [Header("Recompensas")]
    public List<GameObject> materialPrefabs;
    public int maxMaterials = 50;
    public int minMaterials = 20;
    public float dropForce = 3f;
    public Vector3 dropArea = new Vector3(1, 0, 0);
    

    private int currentHits;
    private int currentHealth;
    private bool isBroken = false;
    private Material originalMaterial;

    private void Start()
    {
        currentHits = 0;
        currentHealth = maxHits;
        
    }

    public void TakeDamage(int damage = 1, Vector3 hitDirection = default(Vector3))
    {
        if (isBroken) return;
        
        currentHits += damage;
        currentHealth = maxHits - currentHits;
        

        if (currentHits >= maxHits)
        {
            BreakObject(hitDirection);
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

    void HitByBullet(float damage = 1, Vector3 hitDirection = default(Vector3))
    {
        if (canBeShot)
        {
            TakeDamage((int)damage, hitDirection);
        }
    }
    
    public void HitByMelee(int damage = 1, Vector3 hitDirection = default(Vector3))
    {
        if (canBeHit)
        {
            TakeDamage(damage, hitDirection);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (canBeShot)
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                Vector3 hitDirection = other.contacts[0].point - transform.position;
                HitByBullet(bullet.damage, hitDirection.normalized);
                
                return;
            }
        }

        if (canBeHit)
        {
            MeleeAttack attack = other.gameObject.GetComponent<MeleeAttack>();
            Vector3 hitDirection = other.contacts[0].point - transform.position;
            HitByMelee(attack.damage, hitDirection.normalized);
        }
    }
}
