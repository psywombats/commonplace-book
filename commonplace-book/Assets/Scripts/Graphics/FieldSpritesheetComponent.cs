using UnityEngine;

public class FieldSpritesheetComponent : MonoBehaviour {

    [SerializeField] public new string tag;

    public int StepCount => spritesheet == null ? 1 : spritesheet.StepCount;
    public string Name => spritesheet == null ? "" : spritesheet.name;

    private SpritesheetData spritesheet = null;

    public void Awake() {
        SetByTag(tag);
    }

    public void SetByTag(string tag) {
        this.tag = tag;
        if (tag == null || tag == "") {
            // ?
        } else {
            spritesheet = IndexDatabase.Instance().FieldSprites.GetData(tag).sprite;
        }
    }

    public Sprite GetFrame(OrthoDir dir, int step) {
        if (spritesheet == null) {
            return null;
        }
        if (spritesheet.IsSingleFrame) {
            step = 0;
        }
        if (spritesheet.IsSingleStrip) {
            dir = OrthoDir.North;
        }
        return spritesheet.GetSprite(dir, step);
    }

    public Sprite FrameForDirection(OrthoDir facing) {
        return GetFrame(facing, 0);
    }
}
