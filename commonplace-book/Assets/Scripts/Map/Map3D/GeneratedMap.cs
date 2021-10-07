using System.Collections.Generic;
using UnityEngine;

public class GeneratedMap : Map {

    [SerializeField] public Vector2Int chunkSize;
    [SerializeField] public GameObject plantHolder;
    [SerializeField] public int seed;

    private MapGenerator generator = new MapGenerator();
    private Dictionary<Vector2Int, GeneratedTerrainMesh> meshes = new Dictionary<Vector2Int, GeneratedTerrainMesh>();
    public List<PlantProp> plantPool { get; private set; } = new List<PlantProp>();

    // this is not right at all
    public override Vector2Int Size => chunkSize;

    public override float GetHeightAt(Vector2 pos) {
        var mesh = GetMeshForPos(pos);
        var origin = GetOriginForPos(pos);
        var submeshPoint = new Vector2(
            pos.x - origin.x,
            pos.y - origin.y);
        return mesh.GetHeightAt(submeshPoint);
    }
    
    public void Rebuild() {
        meshes.Clear();
        seed = Random.Range(0, 10000);
        GetOrGenerateMesh(Vector2Int.zero);
    }

    protected void Update() {
        var avatar = Global.Instance().Maps.Avatar;
        if (avatar != null) {
            var pos3 = avatar.Event.positionPx;
            var pos = new Vector2(pos3.x, pos3.z);
            var r = .75f;
            GetMeshForPos(pos + new Vector2(-chunkSize.x * r, -chunkSize.y * r));
            GetMeshForPos(pos + new Vector2(chunkSize.x * r, -chunkSize.y * r));
            GetMeshForPos(pos + new Vector2(-chunkSize.x * r, chunkSize.y * r));
            GetMeshForPos(pos + new Vector2(chunkSize.x * r, chunkSize.y * r));

            while (meshes.Count > 5) {
                GeneratedTerrainMesh farthest = null;
                Vector2Int farthestOrigin = Vector2Int.zero;
                var farthestDist = 0f;
                foreach (var pair in meshes) {
                    var dist = Vector2.Distance(pos, new Vector2(pair.Key.x + chunkSize.x / 2f, pair.Key.y + chunkSize.y / 2f));
                    if (farthest == null || dist > farthestDist) {
                        farthestDist = dist;
                        farthest = pair.Value;
                        farthestOrigin = pair.Key;
                    }
                }
                RemoveMesh(farthestOrigin);
            }
        }
    }

    private void RemoveMesh(Vector2Int origin) {
        var mesh = meshes[origin];

        foreach (var plant in mesh.Plants) {
            plantPool.Add(plant);
            plant.transform.SetParent(plantHolder.transform);
            plant.gameObject.SetActive(false);
        }
        mesh.Plants.Clear();

        mesh.Dispose();
        meshes.Remove(origin);
    }

    private Vector2Int GetOriginForPos(Vector2 pos) {
        var origin = new Vector2Int(
            nfmod(pos.x, chunkSize.x),
            nfmod(pos.y, chunkSize.y));
        return origin;
    }

    private GeneratedTerrainMesh GetMeshForPos(Vector2 pos) {
        var origin = GetOriginForPos(pos);
        var mesh = GetOrGenerateMesh(origin);
        return mesh;
    }

    private GeneratedTerrainMesh GetOrGenerateMesh(Vector2Int origin) {
        if (!meshes.ContainsKey(origin)) {
            var mesh = generator.GenerateMesh(this, origin, seed);
            meshes.Add(origin, mesh);
        }
        return meshes[origin];
    }

    // -6, 5 -> -10
    private static int nfmod(float a, int b) {
        var n = a < 0;
        if (n) a *= -1;
        var c = (int)(a / b) * b;
        if (!n) return c;
        else return -c - b;
    }
}
