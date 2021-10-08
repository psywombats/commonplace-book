using UnityEngine;

public class EnsureGenerationComponent : MonoBehaviour {

    protected void Update() {
        var map = Global.Instance().Maps.ActiveMap as GeneratedMap;
        if (map != null) {
            map.EnsureGenerationAround(transform.position);
        }
    }
}
