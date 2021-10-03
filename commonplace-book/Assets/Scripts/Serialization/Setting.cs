using Newtonsoft.Json;
using System;

[JsonObject(MemberSerialization.OptIn)]
public class Setting<T> : ISetting {

    [JsonProperty]
    private T value;
    public T Value {
        get {
            return value;
        }
        set {
            if (!this.value.Equals(value)) {
                this.value = value;
                OnModify?.Invoke();
                Global.Instance().Serialization.SaveSystemMemory();
            }
        }
    }

    public event Action OnModify;
    
    public Setting(T defaultValue) {
        value = defaultValue;
    }
}

public interface ISetting {

}
