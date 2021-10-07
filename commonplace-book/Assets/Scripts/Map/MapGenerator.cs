﻿using System.Collections.Generic;
using UnityEngine;

public class MapGenerator {

    // this many tiles make up a 1*1 chunk of perlin noise
    private Vector2 PerlinScale = new Vector2(10, 10);
    
    private Vector2Int offset;

    // per render session
    private GeneratedMap map;
    private float[] heights;
    
    public MapGenerator() {

    }

    public GeneratedTerrainMesh GenerateMesh(GeneratedMap map, Vector2Int offset, int seed = 0) {
        this.offset = offset;
        var size = map.Size;
        var cornerCount = new Vector2Int(size.x + 1, size.y + 1);

        Random.InitState(seed);

        var terrain = GeneratedTerrainMesh.Instantiate();
        terrain.transform.SetParent(map.transform);
        terrain.transform.localPosition = new Vector3(offset.x, 0, offset.y);

        terrain.Resize(map.Size);
        terrain.knitVertices = true;
        for (var x = 0; x < cornerCount.x; x += 1) {
            for (var y = 0; y < cornerCount.y; y += 1) {
                terrain.SetHeight(x, y, GetHeightAt(new Vector2Int(x, y)));
            }
        }
        terrain.Rebuild(regenMesh: true);

        var plantCount = size.x * size.y * .7f;
        for (var i = 0; i < plantCount; i += 1) {
            if (map.plantPool.Count == 0) {
                terrain.Plants.Add(PlantProp.Instantiate());
            } else {
                var p = map.plantPool[0];
                map.plantPool.Remove(p);
                terrain.Plants.Add(p);
            }
            var plant = terrain.Plants[i];
            plant.transform.SetParent(terrain.plantHolder.transform);

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

        return terrain;
    }

    private float GetHeightAt(Vector2Int pos) {
        var value = Mathf.PerlinNoise(
            (offset.x + pos.x) / PerlinScale.x,
            (offset.y + pos.y) / PerlinScale.y);
        return value * 5;
    }
}