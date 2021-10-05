using UnityEngine;

public class GeneratedMap : Map {

    [SerializeField] protected Vector2Int size;
    [SerializeField] public GeneratedTerrainMesh terrain;

    private MeshFilter filter;
    public MeshFilter Filter => filter ?? (filter = GetComponent<MeshFilter>());

    private new MeshRenderer renderer;
    public MeshRenderer Renderer => renderer ?? (renderer = GetComponent<MeshRenderer>());

    public override Vector2Int Size => size;

    public override float GetHeightAt(Vector2Int loc) {
        return terrain.GetHeightAt(loc);
    }

    public void Rebuild() {
        Global.Instance().Maps.Generator.Populate(this, new Vector2Int(0, 0));
    }
}
