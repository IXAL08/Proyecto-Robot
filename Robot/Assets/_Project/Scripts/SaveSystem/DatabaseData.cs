using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewDatabaseData", menuName = "Save System/DatabaseData")]
    public class DatabaseData : ScriptableObject
    {
        public List<string> keys = new List<string>();
        public List<ScriptableObject> values = new List<ScriptableObject>();

        private Dictionary<string, ScriptableObject> database;

        public Dictionary<string, ScriptableObject> Database
        {
            get
            {
                if (database == null)
                {
                    database = new Dictionary<string, ScriptableObject>();
                    for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
                    {
                        if (!database.ContainsKey(keys[i]))
                            database.Add(keys[i], values[i]);
                    }
                }
                return database;
            }
        }
        
        public void AddToDatabase(string key, ScriptableObject value)
        {
            keys.Add(key);
            values.Add(value);
            database = null;
        }

        public void RemoveFromDatabase(string key)
        {
            if(!keys.Contains(key)) return;
            values.RemoveAt(keys.IndexOf(key));
            keys.Remove(key);
            database = null;
        }
        
        public void RemoveFromDatabase(ScriptableObject value)
        {
            if(!values.Contains(value)) return;
            keys.RemoveAt(values.IndexOf(value));
            values.Remove(value);
        }

        public ScriptableObject GetFromDatabase(string key)
        {
            return !keys.Contains(key) ? null : values[keys.IndexOf(key)];
        }

        public string GetKeyFromDatabase(ScriptableObject value)
        {
            return !values.Contains(value) ? null : keys[values.IndexOf(value)];
        }

        public bool ContainsValue(ScriptableObject value)
        {
            return values.Contains(value);
        }

    }
}
