using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class SystemData {

    [JsonProperty] public int LastSaveSlot { get; set; } = -1;

    [JsonProperty] public SaveInfoData[] SaveInfo { get; private set; } = new SaveInfoData[SerializationManager.SaveSlotCount];

    [JsonProperty] public Setting<float> SettingMusicVolume { get; private set; } =             new Setting<float>(0.9f);
    [JsonProperty] public Setting<float> SettingSoundEffectVolume { get; private set; } =       new Setting<float>(0.9f);
    [JsonProperty] public Setting<float> SettingWalkSpeed { get; private set; } =               new Setting<float>(1.0f);

    [JsonProperty] public Dictionary<InputManager.Command, KeybindSetting> keybinds = new Dictionary<InputManager.Command, KeybindSetting>();
}
