using System;
using System.Collections.Generic;
using Robot;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScriptablesDataBase : Singleton<IScriptableDatabaseSource>, IScriptableDatabaseSource
{
    [SerializeField] private Dictionary<string, ScriptableObject> _database = new Dictionary<string, ScriptableObject>();
    [SerializeField] private DatabaseData _databaseData; 
    bool _isDatabaseLoaded = false;
    
    private void LoadDatabase()
    {
        _isDatabaseLoaded = true;
        _database = _databaseData.Database;
    }

    public ScriptableObject GetScriptableByKey(string key)
    {
        if(!_isDatabaseLoaded) LoadDatabase();
        return _database[key];
    }

    public void AddToDatabase(ScriptableObject newScriptable)
    {
        if(!_isDatabaseLoaded) LoadDatabase();
        if (_databaseData.ContainsValue(newScriptable))
        {
            LoadDatabase();
            return;
        }
        _databaseData.AddToDatabase($"{newScriptable.name}_{Guid.NewGuid()}", newScriptable);
        LoadDatabase();
    }

    public string GetKeyFromScriptable(ScriptableObject scriptable)
    {
        if(!_isDatabaseLoaded) LoadDatabase();
        string key = "";
        foreach (var obj in _database)
        {
            if (obj.Value == scriptable)
            {
                key = obj.Key;
                break;
            }
        }
        if(string.IsNullOrEmpty(key)) print("Scriptable Not Found in Database");
        return key;
    }

    public string GetKeyAndAddScriptable(ScriptableObject scriptable)
    {
        if(!_isDatabaseLoaded) LoadDatabase();
        if (!_database.ContainsValue(scriptable))
        {
            AddToDatabase(scriptable);
        }
        return GetKeyFromScriptable(scriptable);
    }
}
