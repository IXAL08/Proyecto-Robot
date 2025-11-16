using UnityEngine;

namespace Robot
{
    public interface IUISource
    {
        void OpenPause();
        void ClosePause();
        void OpenSetting();
        void CloseSetting();
        void OpenTutorialMenu();
    }
}
