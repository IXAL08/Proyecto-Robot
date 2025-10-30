using System;
using System.Collections.Generic;
using System.Linq;
using Robot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIManager : MonoBehaviour
{
    [SerializeField] private RecipeData _currentSelectedRecipe;
    private int _currentSelectedRecipeIndex;
    [SerializeField] private List<RecipeData> _recipesToCraft;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _itemUIPrefab;
    [SerializeField] private GameObject _itemsNeededContainer;

    [SerializeField] private GameObject UIDUMMY;

    private void Start()
    {
        LateStartSystem.ExecuteOnLateStart(LoadCurrentRecipeData);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIDUMMY.SetActive(!UIDUMMY.activeSelf);
        }
    }

    public void SelectNextRecipe()
    {
        _currentSelectedRecipeIndex++;
        if (_currentSelectedRecipeIndex >= _recipesToCraft.Count)
        {
            _currentSelectedRecipeIndex = 0;
        }
        LoadCurrentRecipeData();
    }
    
    public void SelectPreviousRecipe()
    {
        _currentSelectedRecipeIndex--;
        if (_currentSelectedRecipeIndex < 0)
        {
            _currentSelectedRecipeIndex = _recipesToCraft.Count -1;
        }
        LoadCurrentRecipeData();
    }

    private void RefreshUI()
    {
        if (_currentSelectedRecipeIndex >= _recipesToCraft.Count)
        {
            _currentSelectedRecipeIndex = _recipesToCraft.Count - 1;
        }
        LoadCurrentRecipeData();
    }

    private void LoadCurrentRecipeData()
    {
        _currentSelectedRecipe = _recipesToCraft[_currentSelectedRecipeIndex];
        if (!_currentSelectedRecipe)
        {
            print("No recipes to craft :)");
            return;
        }
        _nameText.text = _currentSelectedRecipe._itemToCraft.Name;
        _descriptionText.text = _currentSelectedRecipe._itemToCraft.Description;
        _icon.sprite = _currentSelectedRecipe._itemToCraft.Icon;

        List<InventoryItemUI> spawnedItemsUI = _itemsNeededContainer.GetComponentsInChildren<InventoryItemUI>().ToList();
        foreach (var itemUI in spawnedItemsUI)
        {
            itemUI.gameObject.SetActive(false);
        }

        while (spawnedItemsUI.Count < _currentSelectedRecipe._itemsNeeded.Count)
        {
            spawnedItemsUI.Add(Instantiate(_itemUIPrefab, _itemsNeededContainer.transform).GetComponent<InventoryItemUI>());
        }

        for (var i = 0; i < _currentSelectedRecipe._itemsNeeded.Count; i++)
        {
            var item = _currentSelectedRecipe._itemsNeeded[i];
            spawnedItemsUI[i].Setup(item.Item,item.Quantity);
            spawnedItemsUI[i].gameObject.SetActive(true);
        }
    }

    public void TryToCraftCurrentRecipe()
    {
        var inventory = Inventory.Source;
        foreach (var item in _currentSelectedRecipe._itemsNeeded)
        {
            if (!inventory.IsItemQuantityInInventory(item.Item, item.Quantity))
            {
                print($"Not enough resources {item.Item.Name} quantity {item.Quantity}");
                return;
            }
        }
        CraftCurrentRecipe();
    }

    private bool CraftCurrentRecipe()
    {
        try
        {
            foreach (var item in _currentSelectedRecipe._itemsNeeded)
            {
                Inventory.Source.RemoveItemFromInventory(item.Item, item.Quantity*-1);
            }
            Inventory.Source.AddItemToInventory(_currentSelectedRecipe._itemToCraft);
            _recipesToCraft.Remove(_currentSelectedRecipe);
            RefreshUI();
            print("Item Crafted successfully");
            return true;
        }
        catch (Exception e)
        {
            print($"Error crafting the item, Exception: {e.Message}");
            return false;
        }
    }
}
