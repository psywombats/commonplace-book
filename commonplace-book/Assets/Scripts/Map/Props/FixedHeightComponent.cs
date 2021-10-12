using UnityEngine;

public class FixedHeightComponent : MonoBehaviour {

    [SerializeField] private float height = 0f;

    protected void Update() {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            height,
            transform.localPosition.z);
    }
}
