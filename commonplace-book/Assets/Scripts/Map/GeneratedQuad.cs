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

    private int[] ourTris;
    private Vector2[] ourUVs;
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
        ourTris = new int[6];

        if (lowerRight.y != upperLeft.y) {
            ourUVs = new Vector2[] {
                spriteUVs[2],
                spriteUVs[0],
                spriteUVs[1],
                spriteUVs[2],
                spriteUVs[1],
                spriteUVs[3],
            };
        } else {
            ourUVs = new Vector2[] {
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
        if (lowerRight.y != upperLeft.y) {
            AddVertex(lowerLeft, knit, 0);
            AddVertex(upperLeft, knit, 1);
            AddVertex(upperRight, knit, 2);
            AddVertex(lowerLeft, knit, 3);
            AddVertex(upperRight, knit, 4);
            AddVertex(lowerRight, knit, 5);
        } else {
            AddVertex(lowerLeft, knit, 0);
            AddVertex(upperLeft, knit, 1);
            AddVertex(lowerRight, knit, 2);
            AddVertex(upperLeft, knit, 3);
            AddVertex(upperRight, knit, 4);
            AddVertex(lowerRight, knit, 5);
        }
        terrain.AddTris(ourTris);
    }

    private void AddVertex(Vector3 pos, bool knit, int i) {
        if (knit) {
            ourTris[i] = terrain.GetOrAddVertex(pos);
        } else {
            ourTris[i] = terrain.AddVertex(pos);
        }
    }
}
