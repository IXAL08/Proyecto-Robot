using UnityEngine;

namespace Robot
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _chipUIInventory;
        [SerializeField] private CanvasGroup _guiUI;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenOrCloseChipMenu(_chipUIInventory, true);
                HideGUI();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenOrCloseMenu(_chipUIInventory, false);
                ShowGUI();
            }
        }

        private void OpenOrCloseMenu(GameObject menu, bool value)
        {
            menu.SetActive(value);
        }

        private void OpenOrCloseChipMenu(GameObject menu, bool value)
        {
            menu.SetActive(value);
            menu.GetComponentInChildren<ChipDisplay>().RefreshChipDescription();
        }

        private void HideGUI()
        {
            _guiUI.alpha = 0;
        }

        private void ShowGUI()
        {
            _guiUI.alpha = 1;
        }
    }
}
