using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robot;
using UnityEngine;

public class SaveSystemManager : Singleton<ISaveSystemSource>, ISaveSystemSource
{
    [SerializeField] int _saveSlot;
    [SerializeField] List<FileData> _saveFilesToSave = new List<FileData>();
    
    public T GetFileData<T>()
    {
        foreach (var obj in _saveFilesToSave)
        {
            if (obj is T typedObj)
            {
                return typedObj;
            }
        }

        return default;
    }
    
    protected override void Awake()
    {
        base.Awake();
        LoadGame();
    }

    public int GetCurrentSaveSlot()
    {
        return _saveSlot;
    }

    public void ResetSaveSlot(int saveSlot)
    {
        PlayerPrefs.SetString($"GameSave{_saveSlot}", "");
    }

    public void SaveFileData(FileData fileData)
    {
        if (!_saveFilesToSave.Contains(fileData))
        {
            fileData.FileSlotNumber = _saveSlot;
            _saveFilesToSave.Add(fileData);
        }

        SaveGame();
    }

    [ContextMenu("Save")]
    public void SaveGame()
    {
        string json = SaveAll();
        PlayerPrefs.SetString($"GameSave{_saveSlot}", json);
        print(json);
        Debug.Log($"✅ Files Saved");
    }

    [ContextMenu("Load")]
    public void LoadCurrentGameSave()
    {
        LoadGame(_saveSlot);
    }

    public void LoadGame(int saveSlot = 0)
    {
        var json = PlayerPrefs.GetString($"GameSave{_saveSlot}");
        _saveFilesToSave.Clear();
        if (!string.IsNullOrEmpty(json))
        {
            LoadAll(json);
            Debug.Log($"📂 Save Files Loaded");
        }
    }
    
    [Serializable]
    private class SerializableSave
    {
        public List<SerializableFileEntry> files = new();
    }

    [Serializable]
    private class SerializableFileEntry
    {
        public string type;
        public JObject data;
    }
    
    public string SaveAll()
    {
        var save = new SerializableSave();

        foreach (var file in _saveFilesToSave)
        {
            var entry = new SerializableFileEntry
            {
                type = file.GetType().AssemblyQualifiedName,
                data = JObject.FromObject(SerializeFileData(file, new HashSet<object>(), isRoot: true))
            };

            save.files.Add(entry);
        }

        string json = JsonConvert.SerializeObject(save, Formatting.Indented);
        return json;
    }

    public void LoadAll(string json)
    {
        var save = JsonConvert.DeserializeObject<SerializableSave>(json);

        foreach (var entry in save.files)
        {
            Type type = Type.GetType(entry.type);
            if (type == null)
            {
                Debug.LogWarning($"Tipo no encontrado: {entry.type}");
                continue;
            }
            
            var file = _saveFilesToSave.Find(f => f != null && f.GetType() == type);

            if (file == null)
            {
                Debug.Log($"No se encontró FileData del tipo {type.Name}. Creando nueva instancia...");

                file = ScriptableObject.CreateInstance(type) as FileData;

                if (file == null)
                {
                    Debug.LogError($"No se pudo crear instancia de {type.Name}");
                    continue;
                }

                file.name = $"{type.Name}_Generated_{_saveFilesToSave.Count}";
                file.FileSlotNumber = _saveSlot;
                _saveFilesToSave.Add(file);
            }
            
            DeserializeFileData(file, entry.data);
        }
    }
    
    private object SerializeFileData(object obj, HashSet<object> visited, bool isRoot = false)
    {
        if (obj == null)
            return null;

        Type type = obj.GetType();
        
        if (!type.IsValueType && !(obj is string))
        {
            if (visited.Contains(obj))
                return null;
            visited.Add(obj);
        }
        
        if (!isRoot && obj is ScriptableObject so)
        {
            return new JObject
            {
                ["__type"] = "ScriptableRef",
                ["key"] = ScriptablesDataBase.Source.GetKeyAndAddScriptable(so)
            };
        }
        
        if (type.IsPrimitive || obj is string || obj is decimal)
            return obj;
        
        if (obj is IEnumerable enumerable && !(obj is string))
        {
            var list = new JArray();
            foreach (var item in enumerable)
            {
                var serializedItem = SerializeFileData(item, visited);
                list.Add(serializedItem != null ? JToken.FromObject(serializedItem) : JValue.CreateNull());
            }
            return list;
        }
        
        var dict = new JObject();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.IsNotSerialized) continue;

            object value = field.GetValue(obj);
            var serializedValue = SerializeFileData(value, visited);
            dict[field.Name] = serializedValue != null ? JToken.FromObject(serializedValue) : JValue.CreateNull();
        }

        return dict;
    }
    
    private void DeserializeFileData(object target, JObject data)
    {
        if (data == null)
            return;

        Type type = target.GetType();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!data.TryGetValue(field.Name, out var token))
                continue;

            object value = DeserializeValue(field.FieldType, token);
            field.SetValue(target, value);
        }
    }

    private object DeserializeValue(Type targetType, JToken token)
    {
        if (token == null || token.Type == JTokenType.Null)
            return null;
        
        if (token.Type == JTokenType.Object && token["__type"]?.ToString() == "ScriptableRef")
        {
            string key = token["key"]?.ToString();
            return ScriptablesDataBase.Source.GetScriptableByKey(key);
        }
        
        if (typeof(IList).IsAssignableFrom(targetType))
        {
            var elementType = targetType.IsArray
                ? targetType.GetElementType()
                : targetType.GetGenericArguments()[0];

            var list = (IList)Activator.CreateInstance(targetType);
            foreach (var item in token as JArray)
            {
                list.Add(DeserializeValue(elementType, item));
            }
            return list;
        }
        
        if (targetType.IsPrimitive || targetType == typeof(string) || targetType == typeof(decimal))
            return token.ToObject(targetType);
        
        object obj = null;

        try
        {
            obj = Activator.CreateInstance(targetType);
        }
        catch (MissingMethodException)
        {
            obj = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(targetType);
        }
        if (token is JObject objData)
            DeserializeFileData(obj, objData);
        return obj;
    }
}
