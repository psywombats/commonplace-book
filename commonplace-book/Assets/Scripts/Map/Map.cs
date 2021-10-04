using System.Collections.Generic;
using UnityEngine;


public abstract class Map : MonoBehaviour {

    /// <summary>The number of pixels a tile takes up</summary>
    public const int PxPerTile = 16;
    /// <summary>The number of pixels that make up a tile</summary>
    public const float UnitsPerTile = 1;

    public const string ResourcePath = "Maps/";
    
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectLayer objectLayer;
    [SerializeField] private string bgmKey;

    public string BgmKey => bgmKey;
    public ObjectLayer ObjectLayer => objectLayer;
    
    public abstract Vector2Int Size { get; }
    public Vector2 SizePx { get { return Size * PxPerTile; } }
    public int Width { get { return Size.x; } }
    public int Height { get { return Size.y; } }

    public void Start() {
        // TODO: figure out loading
        Global.Instance().Maps.ActiveMap = this;
    }

    public abstract float GetHeightAt(Vector2Int loc);

    public Vector3Int TileToTilemapCoords(Vector2Int loc) {
        return TileToTilemapCoords(loc.x, loc.y);
    }

    public Vector3Int TileToTilemapCoords(int x, int y) {
        return new Vector3Int(x, -1 * (y + 1), 0);
    }

    // careful, this implementation is straight from MGNE, it's efficiency is questionable, to say the least
    // it does support bigger than 1*1 events though
    public List<MapEvent> GetEventsAt(Vector2Int loc) {
        List<MapEvent> events = new List<MapEvent>();
        foreach (MapEvent mapEvent in objectLayer.GetComponentsInChildren<MapEvent>()) {
            if (mapEvent.ContainsPosition(loc)) {
                events.Add(mapEvent);
            }
        }
        return events;
    }

    // returns the first event at loc that implements T
    public T GetEventAt<T>(Vector2Int loc) {
        List<MapEvent> events = GetEventsAt(loc);
        foreach (MapEvent mapEvent in events) {
            if (mapEvent.GetComponent<T>() != null) {
                return mapEvent.GetComponent<T>();
            }
        }
        return default(T);
    }

    // returns all events that have a component of type t
    public List<T> GetEvents<T>() {
        return new List<T>(objectLayer.GetComponentsInChildren<T>());
    }

    public MapEvent GetEventNamed(string eventName) {
        foreach (ObjectLayer layer in GetComponentsInChildren<ObjectLayer>()) {
            foreach (MapEvent mapEvent in layer.GetComponentsInChildren<MapEvent>()) {
                if (mapEvent.name == eventName) {
                    return mapEvent;
                }
            }
        }
        return null;
    }

    public void OnTeleportTo() {
        if (BgmKey != null) {
            Global.Instance().Audio.PlayBGM(BgmKey);
        }
    }

    public void OnTeleportAway() {

    }
}
