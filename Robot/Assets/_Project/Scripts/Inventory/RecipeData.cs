using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewRecipeData", menuName = "Carfting/Recipe")]
    public class RecipeData : ScriptableObject
    {
        public ItemData _itemToCraft;
        public List<InventorySlot> _itemsNeeded;
    }
}
