using UnityEngine;

namespace Robot
{
    public class EndgameTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
            UIManager.Source.OpenEndgame();
            }
        }
    }
}
