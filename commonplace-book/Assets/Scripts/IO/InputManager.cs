using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class InputManager : SingletonBehavior {

    public enum Command {
        Left,
        Right,
        Up,
        Down,
        Confirm,
        Menu,
        Cancel,
        Debug,
        Reset,
    };

    public enum Event {
        Down,
        Up,
        Hold,
        Repeat,
    };

    private static readonly float KeyRepeatSeconds = 0.5f;

    private Dictionary<Command, List<KeyCode>> keybinds;
    private List<IInputListener> listeners;
    private List<IInputListener> disabledListeners;
    private Dictionary<Command, float> holdStartTimes;
    private Dictionary<string, IInputListener> anonymousListeners;

    private static readonly List<Command> PlayerFacingCommands = new List<Command>() {
        Command.Left,
        Command.Right,
        Command.Up,
        Command.Down,
        Command.Confirm,
        Command.Menu,
        Command.Cancel,
        Command.Reset,
    };

    public void Awake() {
        keybinds = new Dictionary<Command, List<KeyCode>>();
        listeners = new List<IInputListener>();
        disabledListeners = new List<IInputListener>();
        holdStartTimes = new Dictionary<Command, float>();
        anonymousListeners = new Dictionary<string, IInputListener>();

        foreach (Command command in Enum.GetValues(typeof(Command))) {
            Global.Instance().SystemData.keybinds.TryGetValue(command, out KeybindSetting setting);
            if (setting == null && PlayerFacingCommands.Contains(command)) {
                setting = new KeybindSetting(command);
                Global.Instance().SystemData.keybinds.Add(command, setting);
            }

            if (setting != null) {
                setting.OnModify += RedoKeybinds;
            }
        }
        RedoKeybinds();
    }

    private List<IInputListener> listenersTemp = new List<IInputListener>();
    public void Update() {
        listenersTemp.Clear();
        listenersTemp.AddRange(listeners);
        
        foreach (Command command in Enum.GetValues(typeof(Command))) {
            var seenListeners = new HashSet<IInputListener>();
            foreach (IInputListener listener in listenersTemp) {
                if (disabledListeners.Contains(listener)) {
                    continue;
                }

                if (seenListeners.Contains(listener)) {
                    continue;
                }
                seenListeners.Add(listener);

                bool endProcessing = false; // ew.
                foreach (KeyCode code in keybinds[command]) {
                    if (Input.GetKeyDown(code)) {
                        if (command == Command.Reset) {
                            listeners.Clear();
                            disabledListeners.Clear();
                            anonymousListeners.Clear();
                            Global.Instance().GameReset();
                            return;
                        }
                        endProcessing |= listener.OnCommand(command, Event.Down);
                    }
                    if (Input.GetKeyUp(code)) {
                        endProcessing |= listener.OnCommand(command, Event.Up);
                        holdStartTimes.Remove(command);
                    }
                    if (Input.GetKey(code)) {
                        if (!holdStartTimes.ContainsKey(command)) {
                            holdStartTimes[command] = Time.time;
                        }
                        endProcessing |= listener.OnCommand(command, Event.Hold);
                        if (Time.time - holdStartTimes[command] > KeyRepeatSeconds) {
                            endProcessing |= listener.OnCommand(command, Event.Repeat);
                        }
                    }
                    if (endProcessing) break;
                }
                if (endProcessing) break;
            }
        }
    }

    public void PushListener(string id, Func<Command, Event, bool> responder) {
        IInputListener listener = new AnonymousListener(responder);
        anonymousListeners[id] = listener;
        PushListener(listener);
    }
    public void PushListener(IInputListener listener) {
        listeners.Insert(0, listener);
    }

    public void RemoveListener(string id) {
        listeners.Remove(anonymousListeners[id]);
    }
    public void RemoveListener(IInputListener listener) {
        listeners.Remove(listener);
    }

    public void DisableListener(IInputListener listener) {
        disabledListeners.Add(listener);
    }

    public void EnableListener(IInputListener listener) {
        if (disabledListeners.Contains(listener)) {
            disabledListeners.Remove(listener);
        }
    }

    public bool IsFastKeyDown() {
        foreach (KeyCode code in keybinds[Command.Confirm]) {
            if (Input.GetKey(code)) {
                return true;
            }
        }
        return false;
    }

    public void SetDefaultKeybindsForCommand(Command command) {
        switch (command) {
            case Command.Left:
                keybinds[Command.Left] = new List<KeyCode>(new[] { KeyCode.LeftArrow, KeyCode.A, KeyCode.Keypad4 });
                break;
            case Command.Right:
                keybinds[Command.Right] = new List<KeyCode>(new[] { KeyCode.RightArrow, KeyCode.D, KeyCode.Keypad6 });
                break;
            case Command.Up:
                keybinds[Command.Up] = new List<KeyCode>(new[] { KeyCode.UpArrow, KeyCode.W, KeyCode.Keypad8 });
                break;
            case Command.Down:
                keybinds[Command.Down] = new List<KeyCode>(new[] { KeyCode.DownArrow, KeyCode.S, KeyCode.Keypad2 });
                break;
            case Command.Confirm:
                keybinds[Command.Confirm] = new List<KeyCode>(new[] { KeyCode.Space, KeyCode.Z, KeyCode.Return });
                break;
            case Command.Debug:
                keybinds[Command.Debug] = new List<KeyCode>(new[] { KeyCode.F9 });
                break;
            case Command.Cancel:
                keybinds[Command.Cancel] = new List<KeyCode>(new[] { KeyCode.B, KeyCode.X, KeyCode.Backspace });
                break;
            case Command.Menu:
                keybinds[Command.Menu] = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C });
                break;
            case Command.Reset:
                keybinds[Command.Reset] = new List<KeyCode>(new[] { KeyCode.F2 });
                break;

        }
    }

    private void RedoKeybinds() {
        foreach (Command command in Enum.GetValues(typeof(Command))) {
            Global.Instance().SystemData.keybinds.TryGetValue(command, out KeybindSetting setting);
            if (setting != null && setting.Value > 0) {
                keybinds[command] = new List<KeyCode>() { (KeyCode)setting.Value };
            } else {
                SetDefaultKeybindsForCommand(command);
            }
        }
    }

    public IEnumerator ConfirmRoutine() {
        var id = "confirm";
        var done = false;
        PushListener(id, (command, type) => {
            if (type == Event.Down) {
                RemoveListener(id);
                done = true;
            }
            return true;
        });
        while (!done) yield return null;
    }
}
