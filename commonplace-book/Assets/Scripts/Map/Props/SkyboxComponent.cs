using UnityEngine;

public class SkyboxComponent : MonoBehaviour {

    [SerializeField] private GameObject rotationParent;
    [SerializeField] private GameObject instance1;
    [SerializeField] private GameObject instance2;
    [SerializeField] private float width;

    protected void LateUpdate() {
        var rot = rotationParent.transform.eulerAngles.y;

        var targetTrans = -1 * rot / 360f * width;
        instance1.transform.localPosition = new Vector3(
            targetTrans,
            instance1.transform.localPosition.y,
            instance1.transform.localPosition.z);
        instance2.transform.localPosition = new Vector3(
            targetTrans + width,
            instance1.transform.localPosition.y,
            instance1.transform.localPosition.z);
    }
}
