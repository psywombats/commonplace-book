using System.Collections;
using UnityEngine;

public class AudioManager : SingletonBehavior {

    public const string NoBGMKey = "none";
    private const string NoChangeBGMKey = "no_change";
    private const float FadeSeconds = 0.5f;

    private float baseVolume = 1.0f;
    private float bgmVolumeMult = 1.0f;
    private Setting<float> bgmVolumeSetting;
    private Setting<float> sfxVolumeSetting;

    public string CurrentBGMKey { get; private set; }

    public void Start() {
        CurrentBGMKey = NoBGMKey;
        sfxVolumeSetting = Global.Instance().Serialization.SystemData.SettingSoundEffectVolume;
        bgmVolumeSetting = Global.Instance().Serialization.SystemData.SettingMusicVolume;
        SetVolume();
        sfxVolumeSetting.OnModify += SetVolume;
        bgmVolumeSetting.OnModify += SetVolume;
    }

    public void PlayBGM(string key) {
        if (Global.Instance().Data.GetSwitch("disable_bgm")) {
            return;
        }
        if (key != CurrentBGMKey && key != NoChangeBGMKey) {
            // TODO: audio
        }
    }

    public void PlaySFX(string sfxKey) {

    }

    private void SetVolume() {
        var sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        var bgmBus = FMODUnity.RuntimeManager.GetBus("bus:/BGM");
        sfxBus.setVolume(sfxVolumeSetting.Value);
        bgmBus.setVolume(bgmVolumeSetting.Value);
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        CurrentBGMKey = NoBGMKey;
        while (baseVolume > 0.0f) {
            baseVolume -= Time.deltaTime / durationSeconds;
            if (baseVolume < 0.0f) {
                baseVolume = 0.0f;
            }
            SetVolume();
            yield return null;
        }
        baseVolume = 1.0f;
        PlayBGM(NoBGMKey);
    }

    public IEnumerator CrossfadeRoutine(string tag) {
        if (CurrentBGMKey != null && CurrentBGMKey != NoBGMKey) {
            yield return FadeOutRoutine(FadeSeconds);
        }
        PlayBGM(tag);
    }

    private IEnumerator PlaySFXRoutine(AudioSource source, AudioClip clip, bool mutes = false) {
        while (clip != null && clip.loadState == AudioDataLoadState.Loading) {
            yield return null;
        }
        if (clip.loadState == AudioDataLoadState.Loaded) {
            source.clip = clip;
            if (mutes) {
                source.PlayDelayed(0.2f);
                var muteDuration = clip.length;
                StartCoroutine(MuteRoutine(muteDuration));
            }
        }
    }

    private IEnumerator MuteRoutine(float muteDuration) {
        muteDuration += 0.2f;
        bgmVolumeMult = 0.0f;
        SetVolume();
        yield return CoUtils.Wait(muteDuration - 0.2f);
        var elapsed = 0.0f;
        while (elapsed < 0.2f) {
            elapsed += Time.deltaTime;
            bgmVolumeMult = elapsed / 0.2f;
            SetVolume();
            yield return null;
        }
        bgmVolumeMult = 1.0f;
    }
}
