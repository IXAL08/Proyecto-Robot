using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class Chip : MonoBehaviour
    {
        [SerializeField] private ChipData _chipData;
        [SerializeField] private int _currentColumnPlaced, _currentRowPlaced;
        [SerializeField] private bool _hasBeenPlaced;
        [SerializeField] private List<Vector2Int> _coordinatesOccupiedOnGrid = new List<Vector2Int>();

        private List<Vector2Int> _lastShapeRotation = new List<Vector2Int>();
        private int _rotationStepsBeforeRotation;


        public int CurrentColumnPlaced => _currentColumnPlaced;
        public int CurrentRowPlaced => _currentRowPlaced;
        public ChipData ChipData => _chipData;
        public bool HasBeenPlaced => _hasBeenPlaced;
        public List<Vector2Int> CoordinatesOccupiedOnGrid => _coordinatesOccupiedOnGrid;

        public void SetPlaced(bool value)
        {
            _hasBeenPlaced = value;
        }

        public void SaveCoordinates(Vector2Int coords)
        {
            _coordinatesOccupiedOnGrid.Add(coords);
        }

        public void SetRowAndColumn(int row, int column)
        {
            _currentRowPlaced = row;
            _currentColumnPlaced = column;
        }

        public void SaveCurrentStepAndShape()
        {
            _lastShapeRotation = new List<Vector2Int>(_chipData.Shape);
            _rotationStepsBeforeRotation = _chipData.RotationSteps;
        }

        public void RestoreLastStepAndShape() 
        {
            if (_lastShapeRotation.Count > 0)
            {
                _chipData.Shape = new List<Vector2Int>(_lastShapeRotation);
                _chipData.RotationSteps = _rotationStepsBeforeRotation;
            }
        }
    }
}
