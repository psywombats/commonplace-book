using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FadeIndexData", menuName = "Data/Index/FadeIndexData")]
public class FadeIndexData : SerializableObjectIndex<FadeData> {

}

[Serializable]
public class FadeData : GenericDataObject {

    [Range(-1, 1)] public float brightnessMod;
    public float delay;
    public bool invert;

    // copy constructor
    public FadeData(FadeData source) {
        brightnessMod = source.brightnessMod;
        delay = source.delay;
        invert = source.invert;
    }
}
