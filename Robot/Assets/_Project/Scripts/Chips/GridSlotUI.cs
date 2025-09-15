using UnityEngine;

namespace Robot
{
    public class GridSlotUI : MonoBehaviour
    {
        [SerializeField] private int _x, _y;

        public int X => _x;
        public int Y => _y;

        public void AssignCoord(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
