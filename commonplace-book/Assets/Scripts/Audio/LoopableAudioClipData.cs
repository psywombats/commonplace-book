using UnityEngine;

public class LoopableAudioClipData : ScriptableObject {

    public AudioClip clip;
    public long loopBeginSample = 0;
    public long loopEndSample;
    public bool loops = true;
}
