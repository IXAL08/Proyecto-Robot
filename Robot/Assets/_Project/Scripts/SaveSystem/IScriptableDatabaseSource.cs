using UnityEngine;

namespace Robot
{
    public interface IScriptableDatabaseSource
    {
        public ScriptableObject GetScriptableByKey(string key);
        public void AddToDatabase(ScriptableObject newScriptable);
        public string GetKeyFromScriptable(ScriptableObject scriptable);
        public string GetKeyAndAddScriptable(ScriptableObject scriptable);
    }
}
