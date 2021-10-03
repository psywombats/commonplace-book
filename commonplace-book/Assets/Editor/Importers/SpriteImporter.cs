using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

internal sealed class SpriteImporter : AssetPostprocessor {

    public void OnPreprocessTexture() {
        var importer = (TextureImporter)assetImporter;
        var textureSize = EditorUtils.GetPreprocessedImageSize(importer);
        var path = assetPath;
        var assetName = path.Substring(path.LastIndexOf("/") + 1, path.LastIndexOf(".") - path.LastIndexOf("/") - 1);

        if (path.Contains("Sprites") || path.Contains("UI") || path.Contains("tilesets")) {
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.textureType = TextureImporterType.Sprite;

            if (importer.spritesheet != null && importer.spritesheet.Count() > 0) {
                return;
            }

            if (path.Contains("Battler")) {
                importer.spritePixelsPerUnit = Map.PxPerTile / Map.UnitsPerTile;
                importer.isReadable = true;
            } else if (path.Contains("BattleAnim")) {
                importer.spritePixelsPerUnit = Map.PxPerTile / Map.UnitsPerTile;

                if (path.Contains("RM2K")) {
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    var edgeSize = new Vector2Int(96, 96);
                    var index = 0;
                    var spriteData = new List<SpriteMetaData>();
                    for (var y = 0; y < 4; y += 1) {
                        for (var x = 0; x < 5; x += 1) {
                            var subspriteName = assetName + "_" + index.ToString("D2");
                            var origin = new Vector2Int(x * edgeSize.x, y * edgeSize.y);
                            spriteData.Add(CreateMetadata(subspriteName, textureSize, Vector2Int.zero, edgeSize, new Vector2(0.5f, 0.5f), null, x, y).Value);
                            index += 1;
                        }
                    }
                    importer.spritesheet = spriteData.ToArray();
                }
            } else if (assetPath.Contains("Charas")) {
                importer.spritePixelsPerUnit = Map.PxPerTile / Map.UnitsPerTile;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.isReadable = true;

                if (path.Contains("RM2K")) {
                    var edgeSize = new Vector2Int(24, 32);
                    var dirs = new List<OrthoDir>() { OrthoDir.North, OrthoDir.East, OrthoDir.South, OrthoDir.West };

                    var index = 0;
                    var spriteData = new List<SpriteMetaData>();
                    for (var y = 0; y < 2; y += 1) {
                        for (var x = 0; x < 4; x += 1) {
                            var subspriteName = assetName + "_0" + index;
                            var origin = new Vector2Int(x * 72, y * 128);
                            spriteData.AddRange(CreateAllMetadata(subspriteName, textureSize, origin, edgeSize, dirs, rows: 4, cols: 3));
                            index += 1;
                        }
                    }
                    importer.spritesheet = spriteData.ToArray();
                } else if (path.Contains("Anims")) {
                    var texSize = EditorUtils.GetPreprocessedImageSize(importer);
                    var edgeSize = new Vector2Int(16, texSize.y);
                    var dirs = new List<OrthoDir>() { OrthoDir.North };
                    var cols = texSize.x / edgeSize.x;
                    var sprites = CreateAllMetadata(assetName, texSize, Vector2Int.zero, edgeSize, dirs, 1, cols);
                    importer.spritesheet = sprites.ToArray();
                } else if (path.Contains("Ocean")) {
                    var dirs = new List<OrthoDir>() { OrthoDir.East, OrthoDir.North, OrthoDir.West, OrthoDir.South };
                    var edgeSize = new Vector2Int(16, 16);
                    var texSize = EditorUtils.GetPreprocessedImageSize(importer);

                    var index = 0;
                    var spriteData = new List<SpriteMetaData>();
                    for (var y = 0; y < texSize.y / (edgeSize.y * 4); y += 1) {
                        for (var x = 0; x < texSize.x / (edgeSize.x * 2); x += 1) {
                            var subspriteName = assetName + index.ToString("D2");
                            var origin = new Vector2Int(x * 32, y * 64);
                            spriteData.AddRange(CreateAllMetadata(subspriteName, textureSize, origin, edgeSize, dirs, rows: 4, cols: 2));
                            index += 1;
                        }
                    }
                    importer.spritesheet = spriteData.ToArray();
                } else {
                    var edgeSize = new Vector2Int(16, 16);
                    var dirs = new List<OrthoDir>() { OrthoDir.East, OrthoDir.North, OrthoDir.West, OrthoDir.South };
                    var sprites = CreateAllMetadata(assetName, textureSize, Vector2Int.zero, edgeSize, dirs, rows: 4, cols: 2);
                    importer.spritesheet = sprites.ToArray();
                }
            }
        }
    }

    private List<SpriteMetaData> CreateAllMetadata(string name, Vector2Int texSize, Vector2Int origin, Vector2Int edgeSize, List<OrthoDir> dirs, int rows, int cols) {
        var spriteData = new List<SpriteMetaData>();
        for (int y = 0; y < rows; y += 1) {
            for (int x = 0; x < cols; x += 1) {
                var data = CreateMetadata(name, texSize, origin, edgeSize, new Vector2(0.5f, 0.0f), dirs, x, y);
                if (data != null) {
                    spriteData.Add(data.Value);
                }
            }
        }
        return spriteData;
    }

    private SpriteMetaData? CreateMetadata(
            string objectName,
            Vector2Int textureSize,
            Vector2Int origin,
            Vector2Int edgeSize,
            Vector2 pivot,
            List<OrthoDir> dirs,
            int x, int y) {

        var data = new SpriteMetaData();
        data.rect = new Rect(
            origin.x + x * edgeSize.x,
            textureSize.y - (origin.y + (y + 1) * edgeSize.y),
            edgeSize.x,
            edgeSize.y);
        data.alignment = (int)SpriteAlignment.Custom;
        data.border = new Vector4(0, 0, 0, 0);
        data.pivot = pivot;

        if (dirs != null) {
            if (y >= dirs.Count) return null;
            var dir = dirs[y];
            data.name = SpritesheetData.NameForFrame(objectName, dir, x);
        } else {
            data.name = objectName;
        }

        return data;
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (var path in importedAssets) {
            if (path.EndsWith(".png") && path.Contains("Sprites")) {
                var assetName = path.Substring(path.LastIndexOf("/") + 1, path.LastIndexOf(".") - path.LastIndexOf("/") - 1);
                if (path.Contains("Charas")) {
                    if (path.Contains("RM2K")) {
                        var frames = new List<int>() { 0, 2 };
                        for (var i = 0; i < 8; i += 1) {
                            var subspriteName = assetName + "0" + i;
                            CreateScriptableObjectForSubsprite(path, subspriteName, frames);
                        }
                    } else if (path.Contains("Ocean")) {
                        var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
                        var frames = new List<int>() { 0, 1 };
                        for (var i = 0; i < sprites.Length / 8; i += 1) {
                            var subspriteName = assetName + i.ToString("D2");
                            CreateScriptableObjectForSubsprite(path, subspriteName, frames);
                        }
                    } else {
                        CreateScriptableObject(path);
                    }
                } else if (path.Contains("Battlers") && !path.Contains("_mask")) {
                    CreateMaskForBattler(path, assetName);
                }
            }
        }
    }

    private static void CreateScriptableObjectForSubsprite(string filePath, string subspriteName, List<int> frames) {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(filePath)?.OfType<Sprite>();
        var includedSprites = new List<Sprite>();
        foreach (var sprite in sprites) {
            if (sprite.name.Contains(subspriteName)) {
                includedSprites.Add(sprite);
            }
        }
        CreateScriptableObject(filePath, subspriteName, includedSprites, frames);
    }

    private static void CreateScriptableObject(string filePath) {
        var objectName = filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.LastIndexOf(".") - filePath.LastIndexOf("/") - 1);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(filePath)?.OfType<Sprite>();
        CreateScriptableObject(filePath, objectName, new List<Sprite>(sprites), null);
    }

    private static void CreateScriptableObject(string filePath, string objectName, List<Sprite> sprites, List<int> frames) {
        var objectPath = filePath.Substring(0, filePath.LastIndexOf("/") + 1);
        objectPath += objectName + ".asset";
        var data = AssetDatabase.LoadAssetAtPath<SpritesheetData>(objectPath);
        var dataCreated = false;
        if (data == null) {
            dataCreated = true;
            data = ScriptableObject.CreateInstance<SpritesheetData>();
            AssetDatabase.CreateAsset(data, objectPath);
        }

        data.name = objectName;
        data.sprites = sprites;
       
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
        data.spritesheet = texture;

        if (dataCreated || data.walkCycleN == null || data.walkCycleN.Count == 0) {
            if (frames == null) {
                frames = new List<int>();
                for (var i = 0; i < sprites.Count; i += 1) {
                    frames.Add(i);
                }
            }

            data.walkCycleN = frames;
            if (data.walkCycleN == null) {
                data.walkCycleN = new List<int>();
                for (var i = 0; i < data.sprites.Count; i += 1) {
                    data.walkCycleN.Add(i);
                }
            }
            data.walkCycleE = data.walkCycleN;
            data.walkCycleW = data.walkCycleN;
            data.walkCycleS = data.walkCycleN;
        }

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }

    private static void CreateMaskForBattler(string path, string assetName) {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        var tex = new Texture2D(
            sprite.texture.width + 2,
            sprite.texture.height + 2,
            TextureFormat.Alpha8, false);

        var pixelsIn = sprite.texture.GetPixels(0, 0, sprite.texture.width, sprite.texture.height);
        var pixelsOut = new Color[tex.width * tex.height];
        for (var x = 0; x < tex.width; x += 1) {
            for (var y = 0; y < tex.height; y += 1) {
                var fromX = x - 1;
                var fromY = y - 1;
                var detected = false;
                detected |= MaskCheck(sprite.texture, pixelsIn, fromX, fromY);
                detected |= MaskCheck(sprite.texture, pixelsIn, fromX + 1, fromY);
                detected |= MaskCheck(sprite.texture, pixelsIn, fromX - 1, fromY);
                detected |= MaskCheck(sprite.texture, pixelsIn, fromX, fromY + 1);
                detected |= MaskCheck(sprite.texture, pixelsIn, fromX, fromY - 1);
                pixelsOut[x + y * tex.width] = new Color(1, 1, 1, detected ? 1 : 0);
            }
        }
        tex.SetPixels(0, 0, tex.width, tex.height, pixelsOut);

        var bytes = tex.EncodeToPNG();
        var objectPath = path.Substring(0, path.LastIndexOf("/") + 1);
        objectPath += "Masks/" + assetName + "_mask.png";
        File.WriteAllBytes(objectPath, bytes);
        AssetDatabase.ImportAsset(objectPath);
    }
    private static bool MaskCheck(Texture sourceTex, Color[] pixels, int x, int y) {
        if (x < 0 || y < 0 || x >= sourceTex.width || y >= sourceTex.height) {
            return false;
        } else {
            return pixels[x + sourceTex.width * y].a > 0;
        }
    }
}
