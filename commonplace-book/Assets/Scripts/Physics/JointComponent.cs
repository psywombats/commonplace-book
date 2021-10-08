using UnityEngine;

public class JointComponent : MonoBehaviour {

    [SerializeField] private Vector3 neutralDirection = new Vector3(0, 1, 0);
    [SerializeField] private float damping = 40f;
    [SerializeField] private float springiness = 10f;

    private int frameflip;
    private float windProj;
    private Quaternion origRotation;
    private Vector3 angularVelocity;

    private AvatarEvent avatar;
    private AvatarEvent Avatar {
        get {
            if (avatar == null) {
                avatar = Global.Instance().Maps.Avatar;
            }
            return avatar;
        }
    }

    protected void Update() {
        if (transform.position.y < WaterController.Level) {
            enabled = false;
        }

        transform.Rotate(angularVelocity * Time.deltaTime);

        frameflip = (frameflip + 1) % 3;
        if (frameflip != 0) {
            return;
        }

        if (Avatar != null) {
            var avPoss = Avatar.transform.position;
            if (Vector3.Angle(transform.forward, transform.position - avPoss) > 70) {
                return;
            }
            var sqDist = (Avatar.transform.position - transform.position).sqrMagnitude;
            if (sqDist > 12 * 12) {
                return;
            }
        }

        AimForRot(origRotation, springiness);

        var force = WindController.Instance.GetForce(windProj);
        var target = Quaternion.FromToRotation(neutralDirection, force);
        AimForRot(target, force.magnitude * 2f);

        angularVelocity = Vector3.MoveTowards(angularVelocity, Vector3.zero, damping * Time.deltaTime);
        if (angularVelocity.magnitude > 90) {
            angularVelocity = angularVelocity.normalized * 90;
        }
    }

    public void OnReposition(bool enable) {
        enabled = enable;
        if (enable) {
            origRotation = transform.rotation;
            damping = damping * Random.Range(.8f, 1f);
            springiness = springiness * Random.Range(.8f, 1f);
            windProj = WindController.Instance.GetCross(transform.position);
        }
    }

    protected void AimForRot(Quaternion target, float mag) {
        mag *= Quaternion.Dot(target, transform.rotation);
        mag *= Time.deltaTime;
        var delta = Quaternion.Inverse(transform.rotation) * target;
        var eulers = delta.eulerAngles;
        eulers = Sign(eulers);
        angularVelocity += eulers * mag;
    }

    protected Vector3 Sign(Vector3 eulers) {
        while (eulers.x > 180) {
            eulers.x -= 360;
        }
        while (eulers.y > 180) {
            eulers.y -= 360;
        }
        while (eulers.z > 180) {
            eulers.z -= 360;
        }
        return eulers;
    }
}