using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratedQuad {

    // we occupy the six triangles in the tris array starting here
    public int trisIndex;

    // we occupy the four vertices in the vert array starting here
    // can also index into the uvs array
    public int vertsIndex;
    
    public Vector3 normal;
    public Vector2Int pos;

    public Tile tile;

    private Vector2[] ourUVs;

    // creating a new quad mutates the tri/vert/uv arrays
    public GeneratedQuad(List<int> tris, List<Vector3> vertices, List<Vector2> uvs,
            Vector3 lowerLeft, Vector3 lowerRight, Vector3 upperRight, Vector3 upperLeft,
            Tile tile, Tilemap tileset, Vector3 normal, Vector2Int pos) {
        trisIndex = tris.Count;
        vertsIndex = vertices.Count;

        this.normal = normal;
        this.tile = tile;
        this.pos = pos;

        int i = vertices.Count;
        vertices.Add(lowerLeft);
        vertices.Add(lowerRight);
        vertices.Add(upperLeft);
        vertices.Add(upperRight);

        Debug.Assert(tile != null);
        Vector2[] spriteUVs = tile.sprite.uv;
        if (normal.y == 0.0f) {
            spriteUVs = MathHelper3D.AdjustZ(spriteUVs, tileset, lowerLeft.y, normal.x == 0.0f);
        }

        ourUVs = new Vector2[4] {
            spriteUVs[2],
            spriteUVs[0],
            spriteUVs[3],
            spriteUVs[1],
        };
        uvs.AddRange(ourUVs);

        tris.Add(i);
        tris.Add(i + 1);
        tris.Add(i + 2);
        tris.Add(i + 1);
        tris.Add(i + 3);
        tris.Add(i + 2);
    }

    public void UpdateTile(Tile tile, Tilemap tileset, float lowerLeftHeight) {
        Vector2[] spriteUVs = tile.sprite.uv;
        if (normal.y == 0.0f) {
            spriteUVs = MathHelper3D.AdjustZ(spriteUVs, tileset, lowerLeftHeight - 0.5f, normal.x == 0.0f);
        }

        ourUVs = new Vector2[4] {
            spriteUVs[2],
            spriteUVs[0],
            spriteUVs[3],
            spriteUVs[1],
        };
    }

    public void CopyUVs(Vector2[] uvs) {
        for (int i = 0; i < 4; i += 1) {
            uvs[vertsIndex + i] = ourUVs[i];
        }
    }
}
