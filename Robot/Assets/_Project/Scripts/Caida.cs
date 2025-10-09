using UnityEngine;

public class Caida : MonoBehaviour
{
    public Transform destination; // Lugar hacia donde teletransportar
    
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            other.transform.position = destination.position;
        }
    }
}
