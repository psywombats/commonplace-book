using System;
using UnityEngine;

public class WindController : MonoBehaviour {

    [SerializeField] private Vector3 force;

    public static WindController Instance { get; private set; }

    public Vector3 Force => force;

    protected void Awake() {
        if (Instance != null) throw new Exception();
        Instance = this;
    }
}
