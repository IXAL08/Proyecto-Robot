using System;

namespace TerrorConsole
{
    public interface IScreenTransitionSource
    {
        event Action OnTransitionBegan;
        
        void TransitionToScene(string sceneName, TransitionType transitionType = TransitionType.Fade);
        void Transition(Action action, TransitionType transitionType = TransitionType.Fade);

    }
}
