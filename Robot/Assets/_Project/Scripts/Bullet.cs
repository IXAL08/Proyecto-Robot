using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float destroyTime;


    private void Start()
    {
        throw new NotImplementedException();
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
