// from https://gist.github.com/mandarinx/ed733369fbb2eea6c7fa9e3da65a0e17

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class MeshFilterEditor : Editor {

    private Mesh mesh;
    private bool drawNormals;

    void OnEnable() {
        MeshFilter mf = target as MeshFilter;
        if (mf != null) {
            mesh = mf.sharedMesh;
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        drawNormals = EditorGUILayout.Toggle("Draw normals", drawNormals);

    }

    void OnSceneGUI() {
        if (mesh == null || !drawNormals) {
            return;
        }

        Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
        Handles.color = Color.yellow;
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int len = mesh.vertexCount;
        
        for (int i = 0; i < len; i++) {
            Handles.DrawLine(verts[i], verts[i] + normals[i]);
        }
    }
}