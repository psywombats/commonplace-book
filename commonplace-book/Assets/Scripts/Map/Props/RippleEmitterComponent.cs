using System.Collections.Generic;
using UnityEngine;

public class RippleEmitterComponent : MonoBehaviour {

    [SerializeField] private MeshRenderer rendererPrefab = null;
    [Space]
    [SerializeField] private float betweenEmissions = .7f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 scatter = default;

    // renderers by time they were instantiated
    private Dictionary<MeshRenderer, float> renderers = new Dictionary<MeshRenderer, float>();
    private List<MeshRenderer> unusedPool = new List<MeshRenderer>();

    private float timer = 0;
    private float sinceLastEmission = 0f;

    private List<MeshRenderer> toRemove = new List<MeshRenderer>();
    public void Update() {
        sinceLastEmission += Time.deltaTime;
        timer += Time.deltaTime;
        if (sinceLastEmission > betweenEmissions) {
            Emit();
            sinceLastEmission = 0f;
        }

        toRemove.Clear();
        foreach (var renderer in renderers.Keys) {
            var started = renderers[renderer];
            var elapsed = timer - started;
            var t = elapsed / duration;
            if (t > 1) {
                renderer.gameObject.SetActive(false);
                unusedPool.Add(renderer);
                toRemove.Add(renderer);
                continue;
            }

            renderer.material.SetFloat("_Elapsed", t);
        }
        foreach (var renderer in toRemove) renderers.Remove(renderer);
    }

    public void SpeedUp(float ratio) {
        var speedy = Time.deltaTime * ratio;
        sinceLastEmission += speedy;
    }

    private void Emit() {
        MeshRenderer instance;
        if (unusedPool.Count > 0) {
            instance = unusedPool[0];
            unusedPool.RemoveAt(0);
            instance.gameObject.SetActive(true);
        } else {
            instance = Instantiate(rendererPrefab);
        }

        renderers[instance] = timer;
        instance.transform.position = new Vector3(
            transform.position.x + scatter.x * Random.Range(-1f, 1f),
            transform.position.y + scatter.y * Random.Range(-1f, 1f),
            transform.position.z + scatter.z * Random.Range(-1f, 1f));
    }
}
