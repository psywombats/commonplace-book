using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MapGenerator {

    // this many tiles make up a 1*1 chunk of perlin noise
    private Vector2Int PerlinScale = new Vector2Int(10, 10);

    private Vector2 seed = new Vector2(0, 0);

    // per render session
    private GeneratedMap map;
    private float[] heights;
    private Vector2Int size;
    private Vector2Int cornerCount;
    
    public MapGenerator() {

    }

    public void Populate(GeneratedMap map, Vector2Int offset) {
        size = map.Size;
        cornerCount = new Vector2Int(size.x + 1, size.y + 1);
        heights = new float[cornerCount.x * cornerCount.y];

        for (var x = 0; x < cornerCount.x; x += 1) {
            for (var y = 0; y < cornerCount.y; y += 1) {
                var value = Mathf.PerlinNoise(
                    seed.x + offset.x + 1f / PerlinScale.x * x,
                    seed.y + offset.y + 1f / PerlinScale.y * y);
                heights[cornerCount.x * y + x] = value;
            }
        }
    }

    private void RebuildMesh(GeneratedMap map, Vector2Int size) {
#if UNITY_EDITOR
        PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(map.gameObject);
        if (prefabStage != null) {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
#endif

        var filter = map.Filter;
        var mesh = filter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(mesh, "Assets/Resources/Maps/Meshes/" + map.gameObject.name + ".asset");
#endif
            filter.sharedMesh = mesh;
        }

        var quads = new Dictionary<Vector3, Dictionary<Vector3, TerrainQuad>>();
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();
        for (int z = 0; z < cornerCount.y; z += 1) {
            for (int x = 0; x < size.x; x += 1) {
                // top vertices
                float height = heights[cornerCount.x * z + x];
            }
        }

        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}