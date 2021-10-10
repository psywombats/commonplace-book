using System;
using UnityEngine;

public class WindController : MonoBehaviour {

    [SerializeField] private Vector3 force;
    [SerializeField] private float longPeriod = 30f;
    [SerializeField] private float longAmplitude = .8f;
    [SerializeField] private float fastPeriod = 2.5f;
    [SerializeField] private float fastAmplitude = .2f;
    [SerializeField] private float waveSpeed = 1f;

    public static WindController Instance { get; private set; }

    public Vector3 Direction => force;

    public Vector3 GetForce(float cross) {
        var coef = longAmplitude * Mathf.Sin(2f * Mathf.PI * (Time.timeSinceLevelLoad + cross / waveSpeed) / longPeriod) +
            fastAmplitude * Mathf.Sin(2f * Mathf.PI * (Time.timeSinceLevelLoad + cross / waveSpeed) / fastPeriod);
        coef -= .3f;
        if (coef < 0f) return Vector3.zero;
        else return coef * force;
    }

    public float GetCross(Vector3 pos) {
        var projection = Vector3.Project(pos, force);
        return projection.magnitude;
    }

    protected void Awake() {
        if (Instance != null) throw new Exception();
        Instance = this;
    }
}
