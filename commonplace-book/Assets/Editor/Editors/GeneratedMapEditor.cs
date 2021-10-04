using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpritesheetData))]
public class GeneratedMapEditor : Editor {

    private Vector2Int offset;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var map = (GeneratedMap)target;

        offset = EditorGUILayout.Vector2IntField("Offset", offset);

        if (GUILayout.Button("Generate")) {
            var generator = new MapGenerator();
            generator.Populate(map, offset);
        }
    }
}
