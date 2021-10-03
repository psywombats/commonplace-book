using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "TransitionIndexData", menuName = "Data/VN/TransitionIndexData")]
public class TransitionIndexData : SerializableObjectIndex<TransitionData> {

}

[Serializable]
public class TransitionData : GenericDataObject {

    public string FadeOutTag;
    public string FadeInTag;

    public FadeData GetFadeOut() {
        return IndexDatabase.Instance().Fades.GetData(FadeOutTag);
    }

    public FadeData GetFadeIn() {
        return IndexDatabase.Instance().Fades.GetData(FadeInTag);
    }
}