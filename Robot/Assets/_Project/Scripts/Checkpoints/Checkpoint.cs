using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string roomName;
    public bool isActive = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in allCheckpoints)
        {
            checkpoint.Deactivate();
        }
        
        isActive = true;
       CheckpointSystem.Instance.SetCurrentCheckpoint(this);
        
        Debug.Log("Checkpoint activado: " + roomName);
    }
    
    public void Deactivate()
    {
        isActive = false;
    }
}
