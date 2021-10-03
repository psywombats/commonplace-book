using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BGMIndexData", menuName = "Data/Index/BGMIndexData")]
public class BGMIndexData : SerializableObjectIndex<BGMData> {

}

[Serializable]
public class BGMData : GenericDataObject {

    public LoopableAudioClipData track;

}
