using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MapGenerator {

    // this many tiles make up a 1*1 chunk of perlin noise
    private Vector2 PerlinScale = new Vector2(10, 10);

    private Vector2 seed = new Vector2(0, 0);

    // per render session
    private GeneratedMap map;
    private float[] heights;
    
    public MapGenerator() {

    }

    public void Populate(GeneratedMap map, Vector2Int offset) {
        var size = map.Size;
        var cornerCount = new Vector2Int(size.x + 1, size.y + 1);
        var terrain = map.terrain;

        terrain.Resize(map.Size);
        for (var x = 0; x < cornerCount.x; x += 1) {
            for (var y = 0; y < cornerCount.y; y += 1) {
                var value = Mathf.PerlinNoise(
                    seed.x + offset.x + x / PerlinScale.x,
                    seed.y + offset.y + y / PerlinScale.y);
                value = (int)(value * 5) / 5f;
                terrain.SetHeight(x, y, value * 5);
            }
        }
        terrain.Rebuild(regenMesh: true);

    }
}