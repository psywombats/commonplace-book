using UnityEngine;
using System.Collections.Generic;

public abstract class SerializableObjectIndex<T> : ScriptableObjectIndexParent, IDataIndex<T> where T : IKeyedDataObject {

    [SerializeField] protected List<T> dataObjects = null;

    protected Dictionary<string, T> tagToDataObject;

    public void OnEnable() {
        if (dataObjects == null) {
            return;
        }
        tagToDataObject = new Dictionary<string, T>();
        foreach (T dataObject in dataObjects) {
            if (dataObject == null || dataObject.Key == null) continue;
            tagToDataObject[dataObject.Key.ToLower()] = dataObject;
        }
    }

    public T GetData(string key) {
        if (!tagToDataObject.ContainsKey(key.ToLower())) {
            Global.Instance().Debug.LogError("Index " + GetType().Name + " does not contain key\"" + key + "\"");
            return default;
        }
        return tagToDataObject[key.ToLower()];
    }

    public T GetDataOrNull(string tag) {
        if (tagToDataObject.ContainsKey(tag.ToLower())) {
            return GetData(tag);
        } else {
            return default;
        }
    }

    public List<T> GetAll() {
        return dataObjects;
    }
}
