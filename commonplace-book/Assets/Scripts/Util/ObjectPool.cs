using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    [SerializeField] private GameObject prefab = null;

    private List<GameObject> unusedInstances = new List<GameObject>();
    private List<GameObject> allInstances = new List<GameObject>();

    public void OnDestroy() {
        foreach (GameObject instance in unusedInstances) {
            Destroy(instance);
        }
    }

    public GameObject GetObject() {
        GameObject instance;
        if (unusedInstances.Count > 0) {
            instance = unusedInstances[0];
            unusedInstances.RemoveAt(0);
        } else {
            instance = Instantiate(prefab);
            instance.transform.SetParent(transform, false);
            allInstances.Add(instance);
        }
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void FreeInstance(GameObject instance) {
        Debug.Assert(allInstances.Contains(instance));
        instance.gameObject.SetActive(false);
        unusedInstances.Add(instance);
    }

    public void FreeAll() {
        foreach (var instance in allInstances) {
            if (!unusedInstances.Contains(instance)) {
                FreeInstance(instance);
            }
        }
    }
}
