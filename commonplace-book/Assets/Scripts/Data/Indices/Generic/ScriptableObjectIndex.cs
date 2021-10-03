using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class ScriptableObjectIndex<T, U> : ScriptableObject, IIndexPopulater, IDataIndex<T> where T : MainSchema, IKeyedDataObject where U : ScriptableObjectReference {

    [SerializeField] protected List<U> references;

    private Dictionary<string, string> tagToPath;
    private Dictionary<string, string> TagToPath {
        get {
            if (tagToPath == null) {
                tagToPath = new Dictionary<string, string>();
                foreach (var reference in references) {
                    tagToPath.Add(reference.tag, reference.path);
                }
            }
            return tagToPath;
        }
    }

    private Dictionary<string, T> tagToObject = new Dictionary<string, T>();

#if UNITY_EDITOR
    private void RecursivelyPopulateFrom(string dirPath) {
        foreach (var filePath in Directory.EnumerateFiles(dirPath)) {
            var asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (asset != null) {
                asset.ResetKey();
                var baseDir = "Resources/";
                var index = filePath.LastIndexOf(baseDir);
                var subpath = filePath.Substring(index + baseDir.Length, filePath.Length - index - baseDir.Length);
                subpath = subpath.Replace("\\", "/");
                subpath = subpath.Substring(0, subpath.LastIndexOf("."));
                AddReference(asset, subpath);
            }
        }
        foreach (var dir in Directory.EnumerateDirectories(dirPath)) {
            RecursivelyPopulateFrom(dir);
        }
        EditorUtility.SetDirty(this);
    }
#endif

    protected virtual void AddReference(T asset, string subpath) {
        // there's some not great generics going on here, but it prevents this having to be copy/pasted everywhere
        references.Add((U) new ScriptableObjectReference() {
            path = subpath,
            tag = asset.Key,
        });
    }

    public void PopulateIndex() {
#if UNITY_EDITOR
        if (references == null) {
            references = new List<U>();
        } else {
            references.Clear();
        }
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var localPath = selectedPath.Substring(0, selectedPath.LastIndexOf('/'));
        RecursivelyPopulateFrom(localPath);
#endif
    }

    public T GetData(string key) {
        var obj = GetDataOrNull(key);
        if (obj == null) {
            Debug.LogError("Scriptable index " + GetType().Name + " does not contain " + key);
        }
        return obj;
    }

    public T GetDataOrNull(string key) {
        if (tagToObject.ContainsKey(key)) {
            return tagToObject[key];
        } else if (TagToPath.ContainsKey(key)) {
            var path = TagToPath[key];
            var obj = Resources.Load<T>(path);
            tagToObject[key] = obj;
            return obj;
        } else {
            return null;
        }
    }
}

[Serializable]
public class ScriptableObjectReference {
    public string tag;
    public string path;
}
