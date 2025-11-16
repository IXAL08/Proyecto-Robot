using UnityEngine;

namespace Robot
{
    public class TutorialTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UIManager.Source.OpenTutorialMenu();
                gameObject.SetActive(false);
            }
        }
    }
}
