using System.Collections.Generic;
using UnityEngine;

public class GeneratedMap : Map {

    [SerializeField] protected Vector2Int size;
    [SerializeField] protected Vector2Int offset;
    [SerializeField] public GeneratedTerrainMesh terrain;
    [SerializeField] public GameObject plantHolder;

    [HideInInspector] public List<PlantProp> plants;

    private MeshFilter filter;
    public MeshFilter Filter => filter ?? (filter = GetComponent<MeshFilter>());

    private new MeshRenderer renderer;
    public MeshRenderer Renderer => renderer ?? (renderer = GetComponent<MeshRenderer>());

    public override Vector2Int Size => size;

    public override float GetHeightAt(Vector2 loc) {
        return terrain.GetHeightAt(loc);
    }

    public void Rebuild() {
        Global.Instance().Maps.Generator.Populate(this, offset);
    }
}
