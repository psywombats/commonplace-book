using UnityEngine;

public class PrebuiltMap : Map {

    private TacticsTerrainMesh _terrain;
    public TacticsTerrainMesh Terrain {
        get {
            if (_terrain == null) _terrain = GetComponent<TacticsTerrainMesh>();
            return _terrain;
        }
    }

    public override Vector2Int Size => Terrain.size;

    public override float GetHeightAt(Vector2Int loc) {
        return Terrain.HeightAt(loc);
    }
}