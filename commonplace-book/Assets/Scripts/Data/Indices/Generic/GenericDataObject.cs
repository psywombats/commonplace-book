using UnityEngine;

public abstract class GenericDataObject : IKeyedDataObject {

    [SerializeField] private string tag = null;
    public string Key { get { return tag; } }

}
