namespace UnityEngine
{
    /// <summary>
    /// This is meant to be used with Interfaces, not classes.
    /// </summary>
    /// <typeparam name="I">Interface to mask.</typeparam>
    public class Singleton<I> : MonoBehaviour where I : class
    {
        public static I Source { get; private set; }

        [SerializeField] private bool _isPersistent = false;

        protected bool _hasBeenDestroyed = false;

        protected virtual void Awake()
        {
            if (_isPersistent && Source != null)
            {
                DestroyImmediate(gameObject);
                _hasBeenDestroyed = true;
                return;
            }

            Source = this as I;

            if (_isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
