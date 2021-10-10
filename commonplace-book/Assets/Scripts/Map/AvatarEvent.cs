﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharaEvent))]
public class AvatarEvent : MonoBehaviour, IInputListener {

    [SerializeField] private float tilesPerSecond = 8f;
    [SerializeField] private float degreesPerSecond = 70f;
    [Space]
    //[SerializeField] private SphereCollider collider = null;

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

    public void Start() {
        Global.Instance().Maps.Avatar = this;
        Global.Instance().Input.PushListener(this);
        pauseCount = 0;
    }

    public virtual void Update() {
        walkFrames -= Time.deltaTime / 2f;
        if (walkFrames < 0) walkFrames = 0;
        var height = Event.Map.GetHeightAt(new Vector2(transform.localPosition.x, transform.localPosition.z));
        Event.transform.localPosition = new Vector3(
            Event.transform.localPosition.x,
            height,
            Event.transform.localPosition.z);

        if (walkFrames > 0) {
            var colliding = Physics.OverlapSphere(transform.position, 1f);
            var walkRatio = walkFrames / WalkFrameRamp;
            foreach (var collider in colliding) {
                foreach (var joint in collider.transform.parent.GetComponentsInChildren<JointComponent>()) {
                    joint.ApplyForce((joint.transform.position - transform.position) * walkRatio);
                }
            }
        }

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Height", transform.position.y);
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
        walkFrames += Time.deltaTime;
        if (dir == OrthoDir.North || dir == OrthoDir.South) {
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
