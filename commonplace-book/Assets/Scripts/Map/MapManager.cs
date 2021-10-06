using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class MapManager : SingletonBehavior {

    private static readonly string DefaultTransitionTag = "default";

    public Map ActiveMap { get; set; }
    public AvatarEvent Avatar { get; set; }
    public MapGenerator Generator { get; private set; } = new MapGenerator();
    public LuaCutsceneContext Context { get; private set; } = new LuaCutsceneContext();

    private MapCamera _camera;
    public new MapCamera camera {
        get {
            if (_camera == null) {
                _camera = FindObjectOfType<MapCamera>();
            }
            return _camera;
        }
        set {
            _camera = value;
        }
    }

    public IEnumerator TeleportRoutine(string mapName, string targetEventName) {
        TransitionData data = Global.Instance().Database.Transitions.GetData(DefaultTransitionTag);
        
        yield return camera.fade.FadeRoutine(data.GetFadeOut());
        StartCoroutine(Avatar.GetComponent<CharaEvent>().FadeRoutine(0.1f, true));
        RawTeleport(mapName, targetEventName);
        camera = ActiveMap.GetComponentInChildren<MapCamera>();
        yield return camera.fade.FadeRoutine(data.GetFadeIn(), true);
    }
    
    private void RawTeleport(string mapName, Vector2Int location) {
        Assert.IsNotNull(ActiveMap);
        Map newMapInstance = InstantiateMap(mapName);
        RawTeleport(newMapInstance, location);
    }

    private void RawTeleport(string mapName, string targetEventName) {
        Assert.IsNotNull(ActiveMap);
        Map newMapInstance = InstantiateMap(mapName);
        Assert.IsNotNull(newMapInstance, "No new map!!");
        MapEvent target = newMapInstance.GetEventNamed(targetEventName);
        RawTeleport(newMapInstance, target.position);
    }

    private void RawTeleport(Map map, Vector2Int location) {
        Assert.IsNotNull(ActiveMap);
        Assert.IsNotNull(Avatar);

        Avatar.transform.SetParent(map.ObjectLayer.transform, false);

        ActiveMap.OnTeleportAway();
        Destroy(ActiveMap.gameObject.transform.parent.gameObject);
        ActiveMap = map;
        ActiveMap.OnTeleportTo();
        Avatar.GetComponent<MapEvent>().SetLocation(location);
    }

    private Map InstantiateMap(string mapName) {
        GameObject newMapObject = null;
        if (ActiveMap != null) {
            string localPath = Map.ResourcePath + mapName;
            newMapObject = Resources.Load<GameObject>(localPath);
        }
        if (newMapObject == null) {
            newMapObject = Resources.Load<GameObject>(mapName);
        }
        Assert.IsNotNull(newMapObject);
        return Instantiate(newMapObject).GetComponentInChildren<Map>();
    }
}
