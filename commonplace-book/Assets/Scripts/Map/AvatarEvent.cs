using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharaEvent))]
public class AvatarEvent : MonoBehaviour, IInputListener {

    private bool wantsToTrack;

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

    public void Start() {
        Global.Instance().Maps.Avatar = this;
        Global.Instance().Input.PushListener(this);
        pauseCount = 0;
    }

    public virtual void Update() {
        wantsToTrack = false;
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

    public bool WantsToTrack() {
        return wantsToTrack;
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
        return true;
    }

    protected virtual Vector2Int VectorForDir(OrthoDir dir) {
        return dir.XY2D();
    }

    private void ShowMenu() {
        // oh shiii
    }
}
