using UnityEngine;

public class MainSchema : ScriptableObject, IKeyedDataObject {

    public string Key {
        get => name;
        set => name = value;
    }

    public void ResetKey() {
#if UNITY_EDITOR
        var path = UnityEditor.AssetDatabase.GetAssetPath(this);
        var index = path.LastIndexOf("/");
        path = path.Substring(index + 1, path.Length - index - 1);
        path = path.Substring(0, path.IndexOf("."));
        name = path;
#endif
    }
}
