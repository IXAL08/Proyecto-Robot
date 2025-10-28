using UnityEngine;

namespace Robot
{
    public interface ISaveSystemSource
    {
        public int GetCurrentSaveSlot();
        public void SaveFileData(FileData fileData);
        public void SaveGame();
        public void LoadGame(int saveSlot = 0);
        public T GetFileData<T>();

    }
}
