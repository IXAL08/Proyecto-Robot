using UnityEngine;

namespace Robot
{
    public class EndgameTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            UIManager.Source.OpenEndgame();
        }
    }
}
