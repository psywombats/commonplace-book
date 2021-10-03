using UnityEngine;
using System.Collections.Generic;

public class SpritesheetData : ScriptableObject {

    public Texture2D spritesheet;
    public List<Sprite> sprites;
    public List<int> walkCycleN, walkCycleE, walkCycleW, walkCycleS;
    public bool autoAnimate = true;

    public int StepCount => walkCycleN.Count;
    public bool IsSingleFrame => spritesheet != null && spritesheet.width < Map.PxPerTile * 2;
    public bool IsSingleStrip => spritesheet != null && spritesheet.height < Map.PxPerTile * 4;

    private Dictionary<string, Sprite> spritesByName;
    private Dictionary<string, Sprite> SpritesByName
    {
        get {
            if (spritesByName == null) {
                if (sprites == null || sprites.Count == 0) {
                    return null;
                }
                spritesByName = new Dictionary<string, Sprite>();
                foreach (var sprite in sprites) {
                    spritesByName[sprite.name] = sprite;
                }
            }
            return spritesByName;
        }
    }

    public Sprite GetSprite(OrthoDir dir, int step) {
        if (SpritesByName == null) return null;
        var walkCycle = GetWalkCycle(dir);
        if (walkCycle.Count <= step) return null;
        var frameNumber = walkCycle[step];
        var frameName = NameForFrame(name, dir, frameNumber);

        if (!SpritesByName.ContainsKey(frameName)) {
            Global.Instance().Debug.LogError(this + " doesn't contain frame " + frameName);
            return null;
        }

        return SpritesByName[frameName];
    }

    public Sprite GetPreviewSprite() {
        if (SpritesByName == null) return null;
        var frameName = NameForFrame(name, OrthoDir.South, 0);
        if (SpritesByName.ContainsKey(frameName)) {
            return SpritesByName[frameName];
        } else {
            return GetSprite(OrthoDir.North, 0);
        }
    }

    public static string NameForFrame(string sheetName, OrthoDir dir, int step) {
        return sheetName + "_" + dir + "_" + step;
    }

    private List<int> GetWalkCycle(OrthoDir dir) {
        switch (dir) {
            case OrthoDir.North:    return walkCycleN;
            case OrthoDir.East:     return walkCycleE;
            case OrthoDir.West:     return walkCycleW;
            case OrthoDir.South:    return walkCycleS;
        }
        return null;
    }
}
