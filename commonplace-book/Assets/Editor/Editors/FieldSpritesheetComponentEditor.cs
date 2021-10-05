using UnityEditor;

[CustomEditor(typeof(FieldSpritesheetComponent), editorForChildClasses: true)]
public class FieldSpritesheetComponentEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
}
