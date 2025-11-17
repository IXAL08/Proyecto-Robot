using UnityEngine;

public class TriggersPlatform : MonoBehaviour
{
        public Plataformas parentPlatform;
        public bool playerInside = false;
    
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
            }
        }
    
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = false;
            }
        }
    }
