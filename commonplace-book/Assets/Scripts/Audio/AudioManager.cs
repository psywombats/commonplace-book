using System.Collections;
using UnityEngine;

public class AudioManager : SingletonBehavior {

    public const string NoBGMKey = "none";
    private const string NoChangeBGMKey = "no_change";
    private const float FadeSeconds = 0.5f;

    private AudioSource sfxSource;
    private AudioSource bgmSource, nextBgmSource;
    private LoopableAudioClipData currentBgm;
    private int bgmSets = 0;

    private float baseVolume = 1.0f;
    private float bgmVolumeMult = 1.0f;
    private Setting<float> bgmVolumeSetting;
    private Setting<float> sfxVolumeSetting;

    public string CurrentBGMKey { get; private set; }

    public void Start() {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        nextBgmSource = gameObject.AddComponent<AudioSource>();
        nextBgmSource.playOnAwake = false;

        CurrentBGMKey = NoBGMKey;

        gameObject.AddComponent<AudioListener>();
        
        sfxVolumeSetting = Global.Instance().Serialization.SystemData.SettingSoundEffectVolume;
        bgmVolumeSetting = Global.Instance().Serialization.SystemData.SettingMusicVolume;
        SetVolume();
        sfxVolumeSetting.OnModify += SetVolume;
        bgmVolumeSetting.OnModify += SetVolume;
    }

    public static void PlayFail() {
        Global.Instance().Audio.PlaySFX("fail");
    }
    
    public void PlaySFX(string key, bool mute = false) {
        if (key == null || key.Length == 0) return;
        AudioClip clip = IndexDatabase.Instance().SFX.GetData(key).clip;
        sfxSource.clip = clip;
        sfxSource.Play();
        StartCoroutine(PlaySFXRoutine(sfxSource, clip, mute));
    }

    public void PlaySFX(AudioClip sfx, bool mute = false) {
        sfxSource.clip = sfx;
        sfxSource.Play();
        StartCoroutine(PlaySFXRoutine(sfxSource, sfx, mute));
    }

    public void PlayBGM(string key) {
        if (Global.Instance().Data.GetSwitch("disable_bgm")) {
            return;
        }
        if (key != CurrentBGMKey && key != NoChangeBGMKey) {
            bgmSets += 1;
            CurrentBGMKey = key;
            bgmSource.Stop();
            nextBgmSource.Stop();
            bgmSource.timeSamples = 0;
            nextBgmSource.timeSamples = 0;
            if (key == null || key == NoBGMKey) {
                currentBgm = null;
            } else {
                SetVolume();
                var data = IndexDatabase.Instance().BGM.GetData(key);
                if (data == null) return;
                currentBgm = data.track;
                bgmSource.clip = currentBgm.clip;
                nextBgmSource.clip = currentBgm.clip;

                bgmSource.PlayScheduled(0.5f);
                StartCoroutine(SwapBgmTracks(0.5f));
            }
        }
    }

    private void ScheduleNextBGMLoop() {
        var samplesPerSecond = (float) currentBgm.clip.frequency;
        var delay = (currentBgm.loopEndSample - nextBgmSource.timeSamples) / samplesPerSecond;
        bgmSource.timeSamples = (int) currentBgm.loopBeginSample;
        bgmSource.PlayDelayed(delay);
        StartCoroutine(SwapBgmTracks(delay));
    }

    private void SetVolume() {
        bgmSource.volume = bgmVolumeSetting.Value * baseVolume * bgmVolumeMult;
        nextBgmSource.volume = bgmVolumeSetting.Value * baseVolume * bgmVolumeMult;
        sfxSource.volume = sfxVolumeSetting.Value * baseVolume;
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

    private IEnumerator SwapBgmTracks(float delay) {
        var oldBgmSets = bgmSets;
        yield return CoUtils.Wait(delay);
        if (oldBgmSets == bgmSets) {
            var temp = nextBgmSource;
            nextBgmSource = bgmSource;
            bgmSource = temp;
            ScheduleNextBGMLoop();
        }
    }
}
