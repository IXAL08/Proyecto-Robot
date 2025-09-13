using UnityEngine;

namespace Robot
{
    public interface IChipsSource
    {
        bool CanPlaceItem(int startX, int startY, ChipData chipData);
        void PlaceItem(int startX, int startY, ChipData chipData);
    }
}
