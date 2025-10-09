using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Robot
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _chipUIInventory;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenChipInventory(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenChipInventory(false);
            }
        }

        private void OpenChipInventory(bool value)
        {
            _chipUIInventory.SetActive(value);
        }

        private async UniTask InitializeUI()
        {
            await UniTask.WaitForSeconds(0.5f);
        }
    }
}
