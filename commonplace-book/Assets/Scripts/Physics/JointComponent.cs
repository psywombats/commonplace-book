using UnityEngine;

public class JointComponent : MonoBehaviour {

    [SerializeField] private Vector3 neutralDirection = new Vector3(0, 1, 0);
    [SerializeField] private float damping = 40f;
    [SerializeField] private float springiness = 10f;

    private Quaternion origRotation;
    private Vector3 angularVelocity;

    protected void Start() {
        origRotation = transform.rotation;
    }

    protected void Update() {
        AimForRot(origRotation, springiness);

        var force = WindController.Instance.Force;
        var target = Quaternion.FromToRotation(neutralDirection, force);
        AimForRot(target, force.magnitude);

        angularVelocity = Vector3.MoveTowards(angularVelocity, Vector3.zero, damping * Time.deltaTime);
        if (angularVelocity.magnitude > 90) {
            angularVelocity = angularVelocity.normalized * 90;
        }

        transform.Rotate(angularVelocity * Time.deltaTime);
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