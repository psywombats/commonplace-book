using System.Collections.Generic;

/// <summary>
/// Unity can't serialize normal dictionaries for arcane reasons
/// </summary>
[System.Serializable]
public class SerialDictionary<K, V> {

    public List<K> keys;
    public List<V> values;

    public SerialDictionary(Dictionary<K, V> dictionary) {
        if (dictionary != null) {
            keys = new List<K>(dictionary.Keys);
            values = new List<V>(dictionary.Values);
        } else {
            keys = new List<K>();
            values = new List<V>();
        }
    }

    public Dictionary<K, V> ToDictionary() {
        Dictionary<K, V> result = new Dictionary<K, V>();
        for (int i = 0; keys != null && i < keys.Count; i += 1) {
            result[keys[i]] = values[i];
        }
        return result;
    }

    public bool IsEmpty() {
        return keys == null || keys.Count == 0;
    }
}
