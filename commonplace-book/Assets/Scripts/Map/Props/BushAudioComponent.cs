using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class BushAudioComponent : MonoBehaviour {

    private const string PrefabPath = "Prefabs/Props/BushAudio";

    private static int poolIndex = 0;
    private static List<BushAudioComponent> bushPool = new List<BushAudioComponent>();
    private static Dictionary<GameObject, BushAudioComponent> bushesByParent = new Dictionary<GameObject, BushAudioComponent>();

    private StudioEventEmitter emitter;
    private StudioEventEmitter Emitter => emitter ?? (emitter = GetComponent<StudioEventEmitter>());

    private static BushAudioComponent GetPooled() {
        if (bushPool.Count < 10) {
            return InstantiateNew();
        } else {
            var toReturn = bushPool[poolIndex];
            poolIndex += 1;
            poolIndex = poolIndex % bushPool.Count;
            return toReturn;
        }
    }

    public static BushAudioComponent GetForTarget(GameObject target) {
        if (bushesByParent.ContainsKey(target)) {
            return bushesByParent[target];
        } else {
            var audio = GetPooled();
            bushesByParent[target] = audio;
            audio.transform.SetParent(target.transform);
            audio.transform.localPosition = Vector3.zero;
            return audio;
        }
    }

    private static BushAudioComponent InstantiateNew() {
        var instance = Instantiate(Resources.Load<BushAudioComponent>(PrefabPath));
        bushPool.Add(instance);
        return instance;
    }

    public void Emit() {
        if (!Emitter.IsPlaying()) {
            Emitter.Play();
        }
    }
}
