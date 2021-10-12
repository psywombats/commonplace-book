using System;
using UnityEngine;

public class WaterController : MonoBehaviour {

    [SerializeField] private float offset = 0f;

    public static float Level => 2.7f;

    protected void Update() {
        if (transform.position.y != Level) {
            transform.position = new Vector3(
                transform.position.x,
                Level + offset,
                transform.position.z);
        }
    }
}
