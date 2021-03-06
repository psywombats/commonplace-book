using UnityEngine;

public class AvatarEvent3D : AvatarEvent {

    public override void Update() {
        base.Update();
        if (Global.Instance().Maps.camera != null) {
            Global.Instance().Maps.camera.ManualUpdate();
        }
    }

    protected override Vector2Int VectorForDir(OrthoDir dir) {
        return dir.XY3D();
    }

}
