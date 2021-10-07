using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Tilemap))]
public class GeneratedTerrainMesh : MonoBehaviour {

    [HideInInspector] public Vector2Int size;
    [HideInInspector] public float[] heights;
    [HideInInspector] public bool knitVertices;
    [Space]
    [SerializeField] public Tile defaultTopTile;
    [SerializeField] public Tilemap tileset;
    [Space]
    [SerializeField] public GameObject plantHolder;

    private const string PrefabPath = "Prefabs/GeneratedTerrainMesh";

    private Tilemap _tilemap;
    public Tilemap tilemap {
        get {
            if (_tilemap == null) _tilemap = GetComponent<Tilemap>();
            return _tilemap;
        }
    }

    // misc that shouldn't be here
    public List<PlantProp> Plants { get; set; } = new List<PlantProp>();

    // geometry
    private Dictionary<Vector2Int, int> cornerPosToVertexIndex;
    private Dictionary<Vector2Int, GeneratedQuad> quads;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> tris;


    public static GeneratedTerrainMesh Instantiate() {
        return Instantiate(Resources.Load<GeneratedTerrainMesh>(PrefabPath));
    }

    public void Dispose() {
        MeshFilter filter = GetComponent<MeshFilter>();
        Destroy(filter.mesh);

        Destroy(gameObject);
    }

    public void Resize(Vector2Int newSize) {
        var newHeightsSize = new Vector2Int(newSize.x + 1, newSize.y + 1);
        tilemap.ClearAllTiles();
        heights = new float[newHeightsSize.x * newHeightsSize.y];
        size = newSize;
        cornerPosToVertexIndex = new Dictionary<Vector2Int, int>();
    }

    public float GetHeightAt(Vector2 pos) {
        var x1 = Mathf.FloorToInt(pos.x);
        var x2 = Mathf.CeilToInt(pos.x);
        var y1 = Mathf.FloorToInt(pos.y);
        var y2 = Mathf.CeilToInt(pos.y);

        var h1 = HeightAtCorner(x1, y1);
        var h2 = HeightAtCorner(x1, y2);
        var ha = Mathf.Lerp(h1, h2, pos.y - y1);

        var h3 = HeightAtCorner(x2, y1);
        var h4 = HeightAtCorner(x2, y2);
        var hb = Mathf.Lerp(h3, h4, pos.y - y1);

        return Mathf.Lerp(ha, hb, pos.x - x1);
    }
    private float HeightAtCorner(int x, int y) {
        return heights[y * (size.x + 1) + x];
    }

    public void SetHeight(int cornerX, int cornerY, float height) {
        heights[cornerY * (size.x + 1) + cornerX] = height;
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

    public int GetOrAddVertex(Vector3 pos) {
        var cornerPos = new Vector2Int((int)pos.x, (int)pos.z);
        if (!cornerPosToVertexIndex.ContainsKey(cornerPos)) {
            var index = AddVertex(pos);
            if (knitVertices) {
                cornerPosToVertexIndex.Add(cornerPos, index);
            }
            return index;
        } else {
            return cornerPosToVertexIndex[cornerPos];
        }
    }
    public int AddVertex(Vector3 pos) {
        vertices.Add(pos);
        return vertices.Count - 1;
    }

    public void AddUVs(List<Vector2> uvs) {
        this.uvs.AddRange(uvs);
    }

    public void AddTris(List<int> tris) {
        this.tris.AddRange(tris);
    }

    public void Rebuild(bool regenMesh) {

#if UNITY_EDITOR
        if (!Application.isPlaying) {
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null) {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
#endif

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh();
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Resources/Maps/Meshes/" + gameObject.name + ".asset");
            }
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
            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            if (knitVertices) {
                // we need to keep the normals but unknit the vertices to use uvs
                var normals = mesh.normals;
                var normalsByPos = new Dictionary<Vector2Int, Vector3>();
                for (var i = 0; i < mesh.normals.Length; i += 1) {
                    var vertex = vertices[i];
                    normalsByPos.Add(new Vector2Int((int)vertex.x, (int)vertex.z), normals[i]);
                }

                vertices.Clear();
                tris.Clear();
                cornerPosToVertexIndex.Clear();
                foreach (var quad in quads.Values) {
                    quad.CopyVerticesToMesh(knit: false);
                }
                mesh.vertices = vertices.ToArray();
                mesh.triangles = tris.ToArray();

                var newNormals = new Vector3[mesh.vertices.Length];
                for (var i = 0; i < vertices.Count; i += 1) {
                    var vertex = vertices[i];
                    newNormals[i] = normalsByPos[new Vector2Int((int)vertex.x, (int)vertex.z)];
                }
                mesh.SetNormals(newNormals);
            }
        }

        uvs.Clear();
        foreach (var quad in quads.Values) {
            quad.CopyUVsToMesh();
        }
        mesh.uv = uvs.ToArray();
    }

    private void AddQuad(Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperRight, Vector3 upperLeft, Tile tile, Vector2Int pos, Vector3 normal) {
        var quad = new GeneratedQuad(this, lowerLeft, lowerRight, upperRight, upperLeft, tile, normal, pos);
        quads[pos] = quad;
        quad.CopyVerticesToMesh(knitVertices);
    }
}
