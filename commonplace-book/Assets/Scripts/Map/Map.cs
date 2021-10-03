using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour {

    /// <summary>The number of pixels a tile takes up</summary>
    public const int PxPerTile = 16;
    /// <summary>The number of pixels that make up a tile</summary>
    public const float UnitsPerTile = 1;

    public const string ResourcePath = "Maps/";
    
    public Grid grid;
    public ObjectLayer objectLayer;

    public string bgmKey { get; private set; }

    private Vector2Int _size;
    public Vector2Int size {
        get {
            if (_size.x == 0) {
                _size = GetComponent<TacticsTerrainMesh>().size;
            }
            return _size;
        }
    }
    public Vector2 sizePx { get { return size * PxPerTile; } }
    public int width { get { return size.x; } }
    public int height { get { return size.y; } }

    private TacticsTerrainMesh _terrain;
    public TacticsTerrainMesh Terrain {
        get {
            if (_terrain == null) _terrain = GetComponent<TacticsTerrainMesh>();
            return _terrain;
        }
    }

    public void Start() {
        // TODO: figure out loading
        Global.Instance().Maps.ActiveMap = this;
    }

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
        if (bgmKey != null) {
            Global.Instance().Audio.PlayBGM(bgmKey);
        }
    }

    public void OnTeleportAway() {

    }
}
