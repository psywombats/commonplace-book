using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeroFixedDistanceComponent : MonoBehaviour {

    [SerializeField] private float distance = 20f;
    [SerializeField] private float acceleration = 1f;

    private Rigidbody body;
    private Rigidbody Body => body ?? (body = GetComponent<Rigidbody>());

    public void Update() {
        var ava = Global.Instance().Maps.Avatar;
        if (ava != null) {
            var dist = Vector3.Distance(transform.position, ava.transform.position);
            if (dist < distance) {
                var v = transform.position - Global.Instance().Maps.Avatar.transform.position;
                v.y = 0;
                Body.AddForce(v.normalized * acceleration);
            }
        }
    }
}
