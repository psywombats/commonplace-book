using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharaEvent))]
public class AvatarEvent : MonoBehaviour, IInputListener {

    [SerializeField] private float tilesPerSecond = 6f;
    [SerializeField] private float degreesPerSecond = 60f;
    [Space]
    [SerializeField] private StudioEventEmitter stepEmitter = null;
    [SerializeField] private RippleEmitterComponent rippleEmitter = null;

    private int pauseCount;
    public bool InputPaused {
        get {
            return pauseCount > 0;
        }
    }

    private MapEvent parent;
    public MapEvent Event {
        get {
            if (parent == null) {
                parent = GetComponent<MapEvent>();
            }
            return parent;
        }
    }
    
    private const float WalkFrameRamp = 1f;
    private float walkFrames;
    private float lastWalkFrames;

    public void Start() {
        Global.Instance().Maps.Avatar = this;
        Global.Instance().Input.PushListener(this);
        pauseCount = 0;
    }

    public virtual void Update() {
        var speed = walkFrames / WalkFrameRamp;
        walkFrames -= Time.deltaTime / 2f;
        if (walkFrames > WalkFrameRamp) walkFrames = WalkFrameRamp;
        if (walkFrames < 0) walkFrames = 0;
        var height = Event.Map.GetHeightAt(new Vector2(transform.localPosition.x, transform.localPosition.z));
        Event.transform.localPosition = new Vector3(
            Event.transform.localPosition.x,
            height,
            Event.transform.localPosition.z);

        if (walkFrames >= lastWalkFrames && walkFrames > 0) {
            var colliding = Physics.OverlapSphere(transform.position, 1f);
            var walkRatio = walkFrames / WalkFrameRamp;
            foreach (var collider in colliding) {
                BushAudioComponent.GetForTarget(collider.gameObject).Emit();
                foreach (var joint in collider.transform.parent.GetComponentsInChildren<JointComponent>()) {
                    joint.ApplyForce((joint.transform.position - transform.position) * walkRatio);
                }
            }
        } else {
            speed = speed - (1f - speed) * 2f;
        }
        lastWalkFrames = walkFrames;

        
        RuntimeManager.StudioSystem.setParameterByName("PlayerSpeed", speed);
        RuntimeManager.StudioSystem.setParameterByName("Height", transform.position.y);

        rippleEmitter.SpeedUp(speed * 1.5f);
        rippleEmitter.enabled = height <= WaterController.Level;
        if (walkFrames < lastWalkFrames) {
            rippleEmitter.transform.position = transform.position;
        }

        var cross = WindController.Instance.GetCross(transform.position);
        var wind = WindController.Instance.GetForce(cross);
        var windPct = wind.magnitude / WindController.Instance.Direction.magnitude;
        RuntimeManager.StudioSystem.setParameterByName("WindSpeed", windPct);
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (InputPaused) {
            return true;
        }
        switch (eventType) {
            case InputManager.Event.Hold:
                switch (command) {
                    case InputManager.Command.Up:
                        TryStep(OrthoDir.North);
                        return false;
                    case InputManager.Command.Down:
                        TryStep(OrthoDir.South);
                        return false;
                    case InputManager.Command.Right:
                        TryStep(OrthoDir.East);
                        return false;
                    case InputManager.Command.Left:
                        TryStep(OrthoDir.West);
                        return false;
                    default:
                        return false;
                }
            case InputManager.Event.Up:
                switch (command) {
                    case InputManager.Command.Confirm:
                        Interact();
                        return false;
                    case InputManager.Command.Cancel:
                        ShowMenu();
                        return false;
                    case InputManager.Command.Debug:
                        return false;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }

    public void PauseInput() {
        pauseCount += 1;
    }

    public void UnpauseInput() {
        pauseCount -= 1;
    }

    private void Interact() {
        Vector2Int target = GetComponent<MapEvent>().position + VectorForDir(GetComponent<CharaEvent>().Facing);
        List<MapEvent> targetEvents = GetComponent<MapEvent>().Map.GetEventsAt(target);
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.switchEnabled && !tryTarget.IsPassableBy(parent)) {
                tryTarget.GetComponent<Dispatch>().Signal(MapEvent.EventInteract, this);
                return;
            }
        }

        target = GetComponent<MapEvent>().position;
        targetEvents = GetComponent<MapEvent>().Map.GetEventsAt(target);
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.switchEnabled && tryTarget.IsPassableBy(parent)) {
                tryTarget.GetComponent<Dispatch>().Signal(MapEvent.EventInteract, this);
                return;
            }
        }
    }

    private bool TryStep(OrthoDir dir) {
        if (dir == OrthoDir.North || dir == OrthoDir.South) {
            if (!stepEmitter.IsPlaying()) {
                stepEmitter.Play();
            }
            walkFrames += Time.deltaTime;
            var speed = tilesPerSecond;
            var underwater = WaterController.Level - transform.localPosition.y;
            if (underwater > 0) {
                var maxUnder = 1.5f;
                var s = 1f - (underwater / maxUnder);
                speed *= s;
                if (speed < 0) speed = 0;
                if (speed < .2f && dir == OrthoDir.South) speed = .2f;
            }
            if (dir == OrthoDir.South) {
                speed *= -.7f;
            }
            Event.transform.localPosition = new Vector3(
                Event.transform.localPosition.x + Mathf.Sin(Mathf.Deg2Rad * transform.localRotation.eulerAngles.y) * speed * Time.deltaTime,
                Event.transform.localPosition.y,
                Event.transform.localPosition.z + Mathf.Cos(Mathf.Deg2Rad * transform.localRotation.eulerAngles.y) * speed * Time.deltaTime);

            //rippleEmitter.transform.position = new Vector3(
            //    transform.position.x,
            //    transform.position.y,
            //    transform.position.z + (dir == OrthoDir.North ? .4f : -.2f));
        } else {
            var sign = dir == OrthoDir.East ? 1 : -1;
            Event.transform.Rotate(Vector3.up, degreesPerSecond * sign * Time.deltaTime);

        }
        return true;
    }

    protected virtual Vector2Int VectorForDir(OrthoDir dir) {
        return dir.XY2D();
    }

    private void ShowMenu() {
        // oh shiii
    }
}
