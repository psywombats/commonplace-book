using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * For our purposes, a CharaEvent is anything that's going to be moving around the map
 * or has a physical appearance. For parallel process or whatevers, they won't have this.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(FieldSpritesheetComponent))]
public class CharaEvent : MonoBehaviour {

    private const float DesaturationDuration = 0.5f;
    private const float StepsPerSecond = 2.0f;

    public Doll Doll;

    private Vector2 lastPosition;
    private Vector3 targetPx;
    private bool animates;
    private int oldX;

    public MapEvent Parent { get { return GetComponent<MapEvent>(); } }
    public Map Map { get { return Parent.Map; } }
    public int StepCount => sprites.StepCount;

    private bool overrideHide = false;
    public bool OverrideHide {
        get => overrideHide;
        set {
            overrideHide = value;
            UpdateEnabled(!overrideHide);
        }
    }

    private FieldSpritesheetComponent sprites;
    public FieldSpritesheetComponent Sprites {
        get {
            if (sprites == null) {
                sprites = GetComponent<FieldSpritesheetComponent>();
            }
            return sprites;
        }
    }

    [SerializeField] [HideInInspector] private OrthoDir _facing = OrthoDir.South;
    public OrthoDir Facing {
        get { return _facing; }
        set {
            _facing = value;
            UpdateAppearance();
        }
    }

    private List<SpriteRenderer> _renderers;
    protected List<SpriteRenderer> Renderers {
        get {
            if (_renderers == null) {
                _renderers = new List<SpriteRenderer> {
                    Doll.Renderer
                };
            }
            return _renderers;
        }
    }

    public void Start() {
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventEnabled, (object payload) => {
            UpdateEnabled((bool)payload);
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, (object payload) => {
            Facing = Parent.DirectionTo(Global.Instance().Maps.Avatar.GetComponent<MapEvent>());
        });
        UpdateEnabled(Parent.switchEnabled);
        animates = true;
    }

    public void Update() {
        if (animates) {
            var elapsed = Time.time;
            var newX = Mathf.FloorToInt(elapsed * StepsPerSecond) % Sprites.StepCount;
            if (oldX != newX) {
                UpdateAppearance();
                oldX = newX;
            }
        }
    }

    public void UpdateEnabled(bool enabled) {
        foreach (var renderer in Renderers) {
            renderer.enabled = enabled && !OverrideHide;
        }
        UpdateAppearance();
    }

    public void UpdateAppearance() {
        Doll.Renderer.sprite = SpriteForMain();
    }

    public void FaceToward(MapEvent other) {
        Facing = Parent.DirectionTo(other);
    }

    public void SetTransparent(bool trans) {
        var propBlock = new MaterialPropertyBlock();
        foreach (SpriteRenderer renderer in Renderers) {
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_Trans", trans ? 1 : 0);
            renderer.SetPropertyBlock(propBlock);
        }
    }

    public void SetAppearanceByTag(string fieldSpriteTag) {
        Sprites.SetByTag(fieldSpriteTag);
        UpdateAppearance();
    }

    public IEnumerator FadeRoutine(float duration, bool inverse = false) {
        float val = inverse ? 1.0f : 0.0f;
        yield return CoUtils.RunTween(Doll.Renderer.DOColor(new Color(val, val, val), duration));
    }

    public bool CanCrossTileGradient(Vector2Int from, Vector2Int to) {
        float fromHeight = Map.GetHeightAt(from);
        float toHeight = GetComponent<MapEvent>().Map.GetHeightAt(to);
        return Mathf.Abs(fromHeight - toHeight) < 1.0f && toHeight > 0.0f;
        //if (fromHeight < toHeight) {
        //    return toHeight - fromHeight <= unit.GetMaxAscent();
        //} else {
        //    return fromHeight - toHeight <= unit.GetMaxDescent();
        //}
    }

    private Sprite SpriteForMain() {
        int x = Mathf.FloorToInt(Time.time * StepsPerSecond) % Sprites.StepCount;
        if (!animates) x = 1;
        return sprites.GetFrame(Facing, x);
    }
}
