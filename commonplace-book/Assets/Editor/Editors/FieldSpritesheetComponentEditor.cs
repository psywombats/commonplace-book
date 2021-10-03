using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldSpritesheetComponent), editorForChildClasses: true)]
public class FieldSpritesheetComponentEditor : Editor {

    private string newTag = "";

    public override void OnInspectorGUI() {
        var spritesheet = (FieldSpritesheetComponent)target;
        GUILayout.Label("Current spritesheet: " + spritesheet.Name);

        newTag = GUILayout.TextField(newTag);
        if (GUILayout.Button("Set by tag")) {
            spritesheet.SetByTag(newTag);
            newTag = "";
            EditorUtility.SetDirty(spritesheet);
        }
    }
}
