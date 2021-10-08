using System.Collections.Generic;
using UnityEngine;

public class PlantProp : MonoBehaviour {

    private const string PrefabPath = "Prefabs/Props/ClumpPlant";

    [SerializeField] private GameObject leafChild = null;
    [SerializeField] private new SphereCollider collider = null;
    [Space]
    [SerializeField] private float minScale = .1f;
    [SerializeField] private float maxScale = .5f;
    [Space]
    [SerializeField] private float minHeightScale = 1f;
    [SerializeField] private float maxHeightScale = 1.5f;
    [Space]
    [SerializeField] private int minLeaves = 3;
    [SerializeField] private int maxLeaves = 7;

    public SphereCollider Collider => collider;

    public float Height => transform.localScale.y * 7.5f; // about

    private List<GameObject> spawnedLeafs = new List<GameObject>();

    public static PlantProp Instantiate() {
        var instance = Instantiate(Resources.Load<PlantProp>(PrefabPath));
        instance.ApplyVariance();
        return instance;
    }

    public void ApplyVariance() {
        var size = Random.Range(0f, 1f);
        var scale = minScale + (maxScale - minScale) * size;
        var heightScale = Random.Range(minHeightScale, maxHeightScale);
        transform.localScale = new Vector3(scale, heightScale * scale, scale);

        int leafCount = (int) (minLeaves + (maxLeaves - minLeaves) * (size / 2f + Random.Range(0f, 1f) / 2f));

        var i = 0;
        for (i = 0; i < leafCount; i += 1) {
            GameObject leaf;
            if (i < spawnedLeafs.Count) {
                leaf = spawnedLeafs[i];
            } else {
                leaf = Instantiate(leafChild);
                spawnedLeafs.Add(leaf);
            }
            leaf.SetActive(true);
            leaf.transform.SetParent(transform);
            leaf.transform.localScale = Vector3.one;
            leaf.transform.localPosition = Vector3.zero;
            leaf.transform.localEulerAngles = new Vector3(0, 360f * i / leafCount, 0);
        }
        for (; i < spawnedLeafs.Count; i += 1) {
            spawnedLeafs[i].SetActive(false);
        }

        transform.localEulerAngles = new Vector3(0, Random.Range(0, 360f), 0f);

        foreach (var joint in GetComponentsInChildren<JointComponent>(includeInactive: true)) {
            var totalScale = scale * heightScale;
            var lowscale = minScale * minHeightScale;
            var hiscale = maxScale * maxHeightScale;
            joint.OnReposition(totalScale > lowscale + .3f * (hiscale - lowscale));
        }
    }
}
