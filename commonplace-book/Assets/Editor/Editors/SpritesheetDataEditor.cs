using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpritesheetData))]
public class SpritesheetDataEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var data = (SpritesheetData)target;
        
        if ( GUILayout.Button("Apply 0, 2 walk cycle")) {
            data.walkCycleN = new List<int>() { 0, 2 };
            data.walkCycleE = new List<int>() { 0, 2 };
            data.walkCycleW = new List<int>() { 0, 2 };
            data.walkCycleS = new List<int>() { 0, 2 };
            EditorUtility.SetDirty(data);
            serializedObject.Update();
        }
        if (GUILayout.Button("Apply 1, 2 walk cycle")) {
            data.walkCycleN = new List<int>() { 1, 2 };
            data.walkCycleE = new List<int>() { 1, 2 };
            data.walkCycleW = new List<int>() { 1, 2 };
            data.walkCycleS = new List<int>() { 1, 2 };
            EditorUtility.SetDirty(data);
            serializedObject.Update();
        }
        if (GUILayout.Button("Apply RM2K converted cycle")) {
            data.walkCycleN = new List<int>() { 0, 2 };
            data.walkCycleE = new List<int>() { 0, 1 };
            data.walkCycleW = new List<int>() { 0, 1 };
            data.walkCycleS = new List<int>() { 0, 2 };
            EditorUtility.SetDirty(data);
            serializedObject.Update();
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        var data = (SpritesheetData)target;

        if (data == null || data.sprites?.Count == 0) {
            return null;
        }

        var sprite = data.GetPreviewSprite();
        if (sprite == null) {
            return null;
        }

        var tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        var pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
        tex.SetPixels(0, 0, (int)sprite.rect.width, (int)sprite.rect.height, pixels);
        tex.Apply();
        TextureScale.Point(tex, width, height);

        return tex;
    }
}
