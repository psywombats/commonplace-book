using UnityEngine;


[RequireComponent(typeof(Dispatch))]
[RequireComponent(typeof(LuaCutsceneContext))]
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public abstract class MapEvent : MonoBehaviour {
    
    private const string PropertyCondition = "show";
    private const string PropertyInteract = "onInteract";
    private const string PropertyCollide = "onCollide";

    public const string EventEnabled = "enabled";
    public const string EventCollide = "collide";
    public const string EventInteract = "interact";
    public const string EventMove = "move";

    // Editor properties
    public Vector2Int position = new Vector2Int(0, 0);
    public Vector2Int size = new Vector2Int(1, 1);
    [Space]
    [Header("Movement")]
    public float tilesPerSecond = 2.0f;
    public bool passable = true;
    [Space]
    [Header("Lua scripting")]
    public string luaCondition;
    [TextArea(3, 6)] public string luaOnInteract;
    [TextArea(3, 6)] public string luaOnCollide;

    // Properties
    public LuaMapEvent LuaObject { get; private set; }
    
    public Vector3 positionPx {
        get { return transform.localPosition; }
        set {
            transform.localPosition = value;
        }
    }

    private Map map;
    public Map Map {
        get {
            if (map != null) {
                return map;
            }
            GameObject parentObject = gameObject;
            while (parentObject.transform.parent != null) {
                parentObject = parentObject.transform.parent.gameObject;
                Map map = parentObject.GetComponent<Map>();
                if (map != null) {
                    this.map = map;
                    return map;
                }
            }
            return null;
        }
    }

    private ObjectLayer _layer;
    public ObjectLayer layer {
        get {
            if (_layer == null) {
                GameObject parent = gameObject;
                do {
                    parent = parent.transform.parent.gameObject;
                    ObjectLayer objLayer = parent.GetComponent<ObjectLayer>();
                    if (objLayer != null) {
                        _layer = objLayer;
                        break;
                    }
                } while (parent.transform.parent != null);
            }
            return _layer;
        }
    }

    private bool _switchEnabled = true;
    public bool switchEnabled {
        get {
            return _switchEnabled;
        }
        set {
            if (value != _switchEnabled) {
                GetComponent<Dispatch>().Signal(EventEnabled, value);
            }
            _switchEnabled = value;
        }
    }

    public abstract Vector3 TileToWorldCoords(Vector2Int location);

    public abstract Vector2Int OffsetForTiles(OrthoDir dir);

    // perform any pixel-perfect rounding needed for a pixel position
    public abstract Vector3 InternalPositionToDisplayPosition(Vector3 position);

    public abstract OrthoDir DirectionTo(Vector2Int position);

    public abstract float CalcTilesPerSecond();

    public void Awake() {
        LuaObject = new LuaMapEvent(this);
    }

    public void Start() {
        if (Application.isPlaying) {
            LuaObject.Set(PropertyCollide, luaOnCollide);
            LuaObject.Set(PropertyInteract, luaOnInteract);
            LuaObject.Set(PropertyCondition, luaCondition);

            GetComponent<Dispatch>().RegisterListener(EventCollide, (object payload) => {
                OnCollide((AvatarEvent)payload);
            });
            GetComponent<Dispatch>().RegisterListener(EventInteract, (object payload) => {
                OnInteract((AvatarEvent)payload);
            });

            CheckEnabled();
        }
    }

    public virtual void Update() {
        if (Application.IsPlaying(this)) {
            CheckEnabled();
        }
    }

    public void OnDrawGizmos() {
        //if (Selection.activeGameObject == gameObject) {
        //    Gizmos.color = Color.red;
        //} else {
            Gizmos.color = Color.magenta;
        //}
        DrawGizmoSelf();
    }

    public void CheckEnabled() {
        switchEnabled = LuaObject.EvaluateBool(PropertyCondition, true);
    }

    public bool IsPassableBy(MapEvent other) {
        return passable || !switchEnabled;
    }

    public OrthoDir DirectionTo(MapEvent other) {
        return DirectionTo(other.position);
    }
    
    public bool ContainsPosition(Vector2Int loc) {
        Vector2Int pos1 = position;
        Vector2Int pos2 = position + size;
        return loc.x >= pos1.x && loc.x < pos2.x && loc.y >= pos1.y && loc.y < pos2.y;
    }

    public void SetLocation(Vector2Int location) {
        map = null;
        position = location;
        SetScreenPositionToMatchTilePosition();
        SetDepth();
    }

    public void SetSize(Vector2Int size) {
        this.size = size;
        SetScreenPositionToMatchTilePosition();
        SetDepth();
    }

    // we have a solid TileX/TileY, please move the doll to the correct screen space
    public abstract void SetScreenPositionToMatchTilePosition();

    // set the one xyz coordinate not controlled by arrow keys
    public abstract void SetDepth();

    protected abstract void DrawGizmoSelf();

    // called when the avatar stumbles into us
    // before the step if impassable, after if passable
    private void OnCollide(AvatarEvent avatar) {
        LuaObject.Run(PropertyCollide);
    }

    // called when the avatar stumbles into us
    // facing us if impassable, on top of us if passable
    private void OnInteract(AvatarEvent avatar) {
        if (GetComponent<CharaEvent>() != null) {
            GetComponent<CharaEvent>().Facing = DirectionTo(avatar.GetComponent<MapEvent>());
        }
        LuaObject.Run(PropertyInteract);
    }

    private LuaScript ParseScript(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
            return new LuaScript(GetComponent<LuaContext>(), lua);
        }
    }

    private LuaCondition ParseCondition(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
           return GetComponent<LuaContext>().CreateCondition(lua);
        }
    }
}
