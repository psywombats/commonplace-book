using UnityEngine;

public class MapCamera : MonoBehaviour {

    [SerializeField] public MapEvent target = null;
    [SerializeField] public FadeComponent fade = null;

    // these are read by sprites, not actually enforced by the cameras
    public bool billboardX;
    public bool billboardY;

    public virtual void ManualUpdate() {

    }

    public virtual Camera GetCameraComponent() {
        return GetComponent<Camera>();
    }
}
