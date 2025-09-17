using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class ChipPlacementData : MonoBehaviour
    {
        [SerializeField] private List<Vector2Int> _coordinatesOccupiedOnGrid = new List<Vector2Int>();
        [SerializeField] private bool _hasBeenPlaced;
        public List<Vector2Int> CoordinatesOccupiedOnGrid => _coordinatesOccupiedOnGrid;
        public bool HasBeenPlaced => _hasBeenPlaced;

        public void ItemPlaced()
        {
            _hasBeenPlaced = true;
        }

        public void ItemRemoved()
        {
            _hasBeenPlaced = false;
            ClearCoordinates();
        }
        public void SaveCoordinates(Vector2Int coords)
        {
            _coordinatesOccupiedOnGrid.Add(coords);
        }

        public void ClearCoordinates()
        {
            _coordinatesOccupiedOnGrid.Clear();
        }
    }
}
