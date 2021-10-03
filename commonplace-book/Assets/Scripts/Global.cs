using System;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    private static Global instance;

    public int InstanceID { get; private set; }
    public static bool IsDestructing { get; private set; }

    [SerializeField] private List<SingletonBehavior> serializedManagers = null;

    public InputManager Input => Get<InputManager>();
    public AudioManager Audio => Get<AudioManager>();
    public MapManager Maps => Get<MapManager>();
    public DebugPanel Debug => Get<DebugPanel>();
    public SerializationManager Serialization => Get<SerializationManager>();

    public GameData Data => Serialization.Data;
    public SystemData SystemData => Serialization.SystemData;

    private IndexDatabase database;
    public IndexDatabase Database {
        get {
            if (database == null) {
                database = IndexDatabase.Instance();
            }
            return database;
        }
    }

    private Dictionary<Type, SingletonBehavior> singletons = new Dictionary<Type, SingletonBehavior>();

    public static Global Instance() {
        if (instance == null) {
            instance = FindObjectOfType<Global>();
            if (instance == null) {
                instance = Instantiate(Resources.Load<Global>("Prefabs/Global"));
                instance.ReadSerializedManagers();
            }
        }
        return instance;
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        instance.InstanceID = UnityEngine.Random.Range(0, int.MaxValue);
        instance.ReadSerializedManagers();
        DontDestroyOnLoad(gameObject);
        MoonSharp.Interpreter.UserData.RegisterAssembly();
    }

    public T Get<T>() where T : SingletonBehavior {
        var type = typeof(T);
        if (!singletons.ContainsKey(type)) {
            var instance = gameObject.AddComponent<T>();
            singletons[type] = instance;
        }
        return (T)singletons[type];
    }

    public void GameReset() {
        instance.InstanceID = UnityEngine.Random.Range(0, int.MaxValue);
        foreach (var manager in singletons.Values) {
            manager.GameReset();
        }
        IEnumeratorAwaitExtensions.Stopped = true;
        // scene switch would need to happen here
    }

    public void OnDestroy() {
        IsDestructing = true;
    }
    
    private void ReadSerializedManagers() {
        foreach (var manager in serializedManagers) {
            singletons[manager.GetType()] = manager;
        }
    }
}
