using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratedQuad {

    // we occupy the four vertices in the vert array starting here
    // can also index into the uvs array
    public int vertsIndex;
    
    public Vector3 normal;
    public Vector2Int pos;

    public GeneratedTerrainMesh terrain;
    public Tile tile;

    private List<int> ourTris;
    private List<Vector2> ourUVs;
    private Vector3 lowerLeft, lowerRight, upperRight, upperLeft;
    
    public GeneratedQuad(GeneratedTerrainMesh terrain,
            Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperRight, Vector3 upperLeft,
            Tile tile, Vector3 normal, Vector2Int pos) {

        this.terrain = terrain;
        this.normal = normal;
        this.tile = tile;
        this.pos = pos;

        this.lowerLeft = lowerLeft;
        this.lowerRight = lowerRight;
        this.upperLeft = upperLeft;
        this.upperRight = upperRight;

        Debug.Assert(tile != null);
        Vector2[] spriteUVs = tile.sprite.uv;
        ourTris = new List<int>();

        if (lowerRight.y != upperLeft.y) {
            ourUVs = new List<Vector2>() {
                spriteUVs[2],
                spriteUVs[0],
                spriteUVs[1],
                spriteUVs[2],
                spriteUVs[1],
                spriteUVs[3],
            };
        } else {
            ourUVs = new List<Vector2>() {
                spriteUVs[2],
                spriteUVs[0],
                spriteUVs[3],
                spriteUVs[0],
                spriteUVs[1],
                spriteUVs[3],
            };
        }
    }

    public void CopyUVsToMesh() {
        terrain.AddUVs(ourUVs);
    }

    public void CopyVerticesToMesh(bool knit) {
        ourTris.Clear();
        if (lowerRight.y != upperLeft.y) {
            AddVertex(lowerLeft, knit);
            AddVertex(upperLeft, knit);
            AddVertex(upperRight, knit);
            AddVertex(lowerLeft, knit);
            AddVertex(upperRight, knit);
            AddVertex(lowerRight, knit);
        } else {
            AddVertex(lowerLeft, knit);
            AddVertex(upperLeft, knit);
            AddVertex(lowerRight, knit);
            AddVertex(upperLeft, knit);
            AddVertex(upperRight, knit);
            AddVertex(lowerRight, knit);
        }
        terrain.AddTris(ourTris);
    }

    private void AddVertex(Vector3 pos, bool knit) {
        if (knit) {
            ourTris.Add(terrain.GetOrAddVertex(pos));
        } else {
            ourTris.Add(terrain.AddVertex(pos));
        }
    }
}
