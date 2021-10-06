using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantProp))]
public class PlantPropEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var plant = (PlantProp)target;

        if (GUILayout.Button("Vary")) {
            plant.ApplyVariance();
        }
    }
}
