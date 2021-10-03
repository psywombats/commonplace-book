using UnityEngine;

public class PlayBGMComponent : MonoBehaviour {

    [SerializeField] private string bgmKey = null;

    public void Start() {
        Global.Instance().Audio.PlayBGM(bgmKey);
    }
}
