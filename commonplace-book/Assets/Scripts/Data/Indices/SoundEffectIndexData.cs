using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SoundEffectIndexData", menuName = "Data/VN/SoundEffectIndexData")]
public class SoundEffectIndexData : SerializableObjectIndex<SoundEffectData> {

}

[Serializable]
public class SoundEffectData : GenericDataObject {

    public AudioClip clip;

}
