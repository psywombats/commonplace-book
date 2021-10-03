using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugPanel : SingletonBehavior {

    public static DebugPanel Instance => Global.Instance().Debug;

    public void LogError(string error) {
        Debug.LogError(error);
    }
}