using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewChips", menuName = "Inventory/Item/Chips")]
    public class ChipData : ScriptableObject
    {
        [Header("Información del chip")]
        public string ID;
        public Sprite ChipSprite;
        public string ChipName;
        [TextArea]
        public string ChipEffectDescription;
        public BonusStatsChip BonusStatsChip;
        public bool IsRotable;
        public int RotationSteps = 0;

        [Header("Tamaño")]
        [Tooltip("Usar solo para UI inicial y chips rectangulares")]
        public int ChipWidth = 1;
        public int ChipHeight = 1;

        [Header("Forma personalizada")]
        [Tooltip("Si quieres una forma personalizada, rellena esta lista manualmente")]
        public List<Vector2Int> Shape = new List<Vector2Int>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += EnsureShape;
            }
        }
#endif

        private void Reset()
        {
            // Genera la forma automáticamente al crear el ScriptableObject
            EnsureShape();
        }

        /// <summary>
        /// Genera la forma predeterminada si la lista está vacía
        /// </summary>
        private void EnsureShape()
        {
            if (Shape == null || Shape.Count == 0)
            {
                GenerateShape();
            }
        }

        /// <summary>
        /// Genera la forma rectangular basada en ChipWidth y ChipHeight
        /// </summary>
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
