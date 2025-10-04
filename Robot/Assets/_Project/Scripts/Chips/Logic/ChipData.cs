using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewChips", menuName = "Inventory/Item/Chips")]
    public class ChipData : ScriptableObject
    {
        public string ID;
        public Sprite ChipSprite;
        public string ChipName;
        public string ChipEffectDescription;
        public bool IsRotable;
        public int RotationSteps = 0;

        [Tooltip("Usar solo para UI inicial y chips rectangulares")]
        public int ChipWidth = 1;
        public int ChipHeight = 1;

        [Tooltip("Si quieres una forma personalizada, rellena esta lista manualmente")]
        public List<Vector2Int> Shape = new List<Vector2Int>();

        private void OnValidate()
        {
            // Solo genera automáticamente si no hay forma personalizada
            if (Shape == null || Shape.Count == 0)
            {
                GenerateShape();
            }
        }

        private void GenerateShape()
        {
            Shape.Clear();
            for (int x = 0; x < ChipWidth; x++)
            {
                for (int y = 0; y < ChipHeight; y++)
                {
                    Shape.Add(new Vector2Int(x, y));
                }
            }
        }
    }

}
