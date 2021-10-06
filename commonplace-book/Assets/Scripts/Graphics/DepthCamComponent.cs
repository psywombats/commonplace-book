using UnityEngine;

[ExecuteInEditMode]
public class DepthCamComponent: MonoBehaviour {

    public void Start() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}