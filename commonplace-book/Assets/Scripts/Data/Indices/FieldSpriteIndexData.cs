using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FieldSpriteDataIndexData", menuName = "Data/FieldSpriteData")]
public class FieldSpriteIndexData : SerializableObjectIndex<FieldSpriteData> {

}

[Serializable]
public class FieldSpriteData : GenericDataObject {

    public SpritesheetData sprite;
}
