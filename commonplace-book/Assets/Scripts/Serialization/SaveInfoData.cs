using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class SaveInfoData {

    [JsonProperty] public long Timestamp { get; private set; }

    public SaveInfoData() {
        // serialized
    }

    public SaveInfoData(GameData data) {
        Timestamp = data.SavedAt;
    }
}
