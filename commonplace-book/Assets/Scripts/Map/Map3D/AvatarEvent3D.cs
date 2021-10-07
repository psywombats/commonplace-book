using UnityEngine;

public class AvatarEvent3D : AvatarEvent {

    public override void Update() {
        base.Update();
        if (Global.Instance().Maps.camera != null) {
            Global.Instance().Maps.camera.ManualUpdate();
        }

        var a = Vector3.Angle(transform.forward, Vector3.zero - transform.position);
        Debug.Log(a);
    }

    protected override Vector2Int VectorForDir(OrthoDir dir) {
        return dir.XY3D();
    }

}
