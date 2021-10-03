public class KeybindSetting : Setting<int> {

    public InputManager.Command Command { get; set; }

    public KeybindSetting(InputManager.Command command) : base(0) {
        Command = command;
    }
}
