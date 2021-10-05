using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(GeneratedMap))]
public class GeneratedTerrainMesh : MonoBehaviour {

    [HideInInspector] public Vector2Int size;
    [HideInInspector] public float[] heights;

    [SerializeField] public Tile defaultTopTile;
    [SerializeField] public Tilemap tileset;

    private Tilemap _tilemap;
    public Tilemap tilemap {
        get {
            if (_tilemap == null) _tilemap = GetComponent<Tilemap>();
            return _tilemap;
        }
    }

    // tortured data structure
    // index by pos -> normal ->
    private Dictionary<Vector2Int, GeneratedQuad> quads;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> tris;


    public void Resize(Vector2Int newSize) {
        var newHeightsSize = new Vector2Int(newSize.x + 1, newSize.y + 1);
        tilemap.ClearAllTiles();
        heights = new float[newHeightsSize.x * newHeightsSize.y];
        size = newSize;
    }

    public float GetHeightAt(Vector2Int pos) {
        return HeightAt(pos.x, pos.y);
    }
    public float HeightAt(int x, int y) {
        if (x < 0 || x >= size.x || y < 0 || y >= size.y) {
            return 0;
        } else {
            var h1 = HeightAtCorner(x, y);
            var h2 = HeightAtCorner(x+1, y);
            var h3 = HeightAtCorner(x, y+1);
            var h4 = HeightAtCorner(x+1, y+1);
            return (h1 + h2 + h3 + h4) / 4f;
        }
    }
    private float HeightAtCorner(int x, int y) {
        return heights[y * (size.x + 1) + x];
    }

    public void SetHeight(int cornerX, int cornerY, float height) {
        heights[cornerX * (size.x + 1) + cornerY] = height;
    }

    public Tile TileAt(int x, int y) {
        Tile tile = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));
        if (tile == null) {
            return defaultTopTile;
        } else {
            return tile;
        }
    }

    public void SetTile(int x, int y, Tile tile) {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public void Rebuild(bool regenMesh) {

#if UNITY_EDITOR
        PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
        if (prefabStage != null) {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
#endif

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Resources/Maps/Meshes/" + gameObject.name + ".asset");
#endif
            filter.sharedMesh = mesh;
        }

        quads = new Dictionary<Vector2Int, GeneratedQuad>();
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        for (int z = 0; z < size.y; z += 1) {
            for (int x = 0; x < size.x; x += 1) {
                // top vertices
                AddQuad(
                    lowerLeft: new Vector3(x, HeightAtCorner(x, z), z), 
                    lowerRight: new Vector3(x + 1, HeightAtCorner(x + 1, z), z),
                    upperLeft: new Vector3(x, HeightAtCorner(x, z + 1), z + 1),
                    upperRight: new Vector3(x + 1, HeightAtCorner(x + 1, z + 1), z + 1), 
                    tile: TileAt(x, z),
                    pos: new Vector2Int(x, z), 
                    normal: new Vector3(0, 1, 0));
            }
        }

        if (regenMesh) {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }

    private void AddQuad(Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperRight, Vector3 upperLeft, Tile tile, Vector2Int pos, Vector3 normal) {
        var quad = new GeneratedQuad(tris, vertices, uvs, lowerLeft, lowerRight, upperRight, upperLeft, tile, normal, pos);
        quads[pos] = quad;
    }

    private void RepaintMesh() {
        Vector2[] uvArray = new Vector2[GetComponent<MeshFilter>().sharedMesh.uv.Length];
        foreach (var quad in quads.Values) {
            quad.CopyUVs(uvArray);
        }

        GetComponent<MeshFilter>().sharedMesh.uv = uvArray;
        uvs = new List<Vector2>(uvArray);
    }
}
