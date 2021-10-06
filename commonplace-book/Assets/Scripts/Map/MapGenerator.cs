using System.Collections.Generic;
using UnityEngine;

public class MapGenerator {

    // this many tiles make up a 1*1 chunk of perlin noise
    private Vector2 PerlinScale = new Vector2(10, 10);

    private Vector2 seed = new Vector2(0, 0);
    private Vector2Int offset;

    // per render session
    private GeneratedMap map;
    private float[] heights;
    
    public MapGenerator() {

    }

    public void Populate(GeneratedMap map, Vector2Int offset) {
        this.offset = offset;
        //seed = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        var size = map.Size;
        var cornerCount = new Vector2Int(size.x + 1, size.y + 1);
        var terrain = map.terrain;

        terrain.Resize(map.Size);
        terrain.knitVertices = true;
        for (var x = 0; x < cornerCount.x; x += 1) {
            for (var y = 0; y < cornerCount.y; y += 1) {
                terrain.SetHeight(x, y, GetHeightAt(new Vector2Int(x, y)));
            }
        }
        terrain.Rebuild(regenMesh: true);

        var plantCount = size.x * size.y * .7f;
        foreach (var plant in map.plants) {
            plant.gameObject.SetActive(false);
        }
        for (var i = 0; i < plantCount; i += 1) {
            if (i >= map.plants.Count) {
                map.plants.Add(PlantProp.Instantiate());
            }
            var plant = map.plants[i];
            plant.transform.SetParent(map.plantHolder.transform);

            for (var attempt = 0; attempt < 10; attempt += 1) {
                var pos = new Vector2(Random.Range(0f, size.x), Random.Range(0f, size.y));
                var height = terrain.GetHeightAt(pos);
                var pos3d = new Vector3(pos.x, height, pos.y);
                var colliding = Physics.OverlapSphere(pos3d, plant.Collider.radius * plant.transform.localScale.x);
                if (colliding.Length != 0) {
                    continue;
                }

                if (height + Random.Range(-.2f, .2f) + plant.Height < WaterController.Level) {
                    continue;
                }

                plant.transform.localPosition = pos3d;
                plant.gameObject.SetActive(true);
                break;
            }
            
        }
    }

    private float GetHeightAt(Vector2Int pos) {
        var value = Mathf.PerlinNoise(
            (seed.x + offset.x + pos.x) / PerlinScale.x,
            (seed.y + offset.y + pos.y) / PerlinScale.y);
        return value * 5;
    }
}