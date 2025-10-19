using UnityEditor;
using UnityEngine;

namespace Robot
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _chipUIInventory;
        [SerializeField] private CanvasGroup _guiUI;

        private void Start()
        {
            InputManager.Source.OpenChipsInventory += OpenChipInventory;
            InputManager.Source.CloseChipsInventory += CloseChipInventory;
        }

        private void OnDisable()
        {
            InputManager.Source.OpenChipsInventory -= OpenChipInventory;
            InputManager.Source.CloseChipsInventory -= CloseChipInventory;
        }

        private void CloseChipInventory()
        {
            _chipUIInventory.SetActive(false);
            ShowGUI();
        }

        private void OpenChipInventory()
        {
            _chipUIInventory.SetActive(true);
            _chipUIInventory.GetComponentInChildren<ChipDisplay>().RefreshChipDescription();
            HideGUI();
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
