using UnityEngine;

public class JointComponent : MonoBehaviour {

    [SerializeField] private Vector3 neutralDirection = new Vector3(0, 1, 0);
    [SerializeField] private float damping = 40f;
    [SerializeField] private float springiness = 10f;

    [SerializeField] private float speed = 2f;

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

    protected void Start() {
        OnReposition(true);
    }

    protected void Update() {
        if (transform.position.y < WaterController.Level) {
            enabled = false;
        }

        transform.localRotation *= Quaternion.Euler(angularVelocity * Time.deltaTime * speed);

        frameflip = (frameflip + 1) % 3;
        if (frameflip != 0) {
            return;
        }

        var magA = angularVelocity.magnitude;
        if (magA > 90) {
            angularVelocity = angularVelocity.normalized * 90;
        }

        var deltaQ = origRotation * Quaternion.Inverse(transform.localRotation);
        var deltaA = Sign(deltaQ.eulerAngles).magnitude;
        if (deltaA > 1) {
            AimForRot(origRotation, springiness * deltaA);
        }
        
        angularVelocity = Vector3.MoveTowards(angularVelocity, Vector3.zero, damping * Time.deltaTime * speed * (magA / 90f + .1f));

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

        ApplyForce(WindController.Instance.GetForce(windProj));
    }

    public void OnReposition(bool enable) {
        enabled = enable;
        if (enable) {
            origRotation = transform.localRotation;
            damping = damping * Random.Range(.8f, 1f);
            springiness = springiness * Random.Range(.8f, 1f);
            windProj = WindController.Instance.GetCross(transform.position);
        }
    }

    public void ApplyForce(Vector3 force) {
        var targetGlobal = Quaternion.FromToRotation(neutralDirection, force);
        var dot = Quaternion.Dot(targetGlobal, transform.rotation);
        var targetLocal = Quaternion.Inverse(transform.rotation) * targetGlobal;
        AimForRot(targetLocal, force.magnitude);
    }

    protected void AimForRot(Quaternion target, float mag) {
        mag *= Time.deltaTime * speed;
        var delta = Quaternion.Inverse(transform.localRotation) * target;
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