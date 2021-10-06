using System;
using UnityEngine;

public class WaterController : MonoBehaviour {

    static float level = 2.7f;

    public static float Level => level;

    protected void Update() {
        if (transform.position.y != level) {
            transform.position = new Vector3(
                transform.position.x,
                level,
                transform.position.z);
        }
    }
}
