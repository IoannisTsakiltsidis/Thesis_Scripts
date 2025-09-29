using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Mixer Routing")]
    public AudioMixer masterMixer;
    public AudioMixerGroup musicBus;
    public AudioMixerGroup sfxBus;
    public AudioMixerGroup voiceBus;
    public AudioMixerGroup uiBus;

    [Header("Music")]
    public float musicFadeSeconds = 1.0f;

    private AudioSource _musicA, _musicB;
    private bool _musicATurn = true;

    [Header("SFX One-Shots")]
    public int sfxPoolSize = 16;
    private readonly List<AudioSource> _sfxPool = new();

    [Header("Listener")]
    public Transform listenerTarget; // usually your main camera

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Music dual sources for crossfade
        _musicA = CreateChildSource("MusicA", musicBus, loop: true, spatial: false);
        _musicB = CreateChildSource("MusicB", musicBus, loop: true, spatial: false);

        // SFX pool
        for (int i = 0; i < sfxPoolSize; i++)
            _sfxPool.Add(CreateChildSource($"SFX_{i}", sfxBus, loop: false, spatial: true));
    }

    private void Update()
    {
        if (!listenerTarget && Camera.main) listenerTarget = Camera.main.transform;
        if (listenerTarget) transform.position = listenerTarget.position; // keep listener near player
    }

    private AudioSource CreateChildSource(string name, AudioMixerGroup bus, bool loop, bool spatial)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = bus;
        src.loop = loop;
        src.playOnAwake = false;
        src.spatialBlend = spatial ? 1f : 0f;
        src.rolloffMode = AudioRolloffMode.Linear;
        return src;
    }

    // ---------- Music ----------
    public void PlayMusic(AudioClip clip, float? customFade = null)
    {
        if (!clip) return;
        var fade = Mathf.Max(0f, customFade ?? musicFadeSeconds);

        var next = _musicATurn ? _musicA : _musicB;
        var prev = _musicATurn ? _musicB : _musicA;
        _musicATurn = !_musicATurn;

        next.clip = clip;
        next.volume = 0f;
        next.Play();

        StopAllCoroutines();
        if (prev.isPlaying && fade > 0f) StartCoroutine(Crossfade(prev, next, fade));
        else
        {
            if (prev.isPlaying) prev.Stop();
            next.volume = 1f;
        }
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / seconds);
            to.volume = k;
            from.volume = 1f - k;
            yield return null;
        }
        from.Stop(); from.volume = 1f;
        to.volume = 1f;
    }

    public void StopMusic(float fadeOut = 0.5f)
    {
        StopAllCoroutines();
        if (fadeOut <= 0f) { _musicA.Stop(); _musicB.Stop(); return; }
        StartCoroutine(FadeOutBoth(fadeOut));
    }

    private IEnumerator FadeOutBoth(float seconds)
    {
        float a0 = _musicA.volume, b0 = _musicB.volume, t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Clamp01(t / seconds);
            _musicA.volume = a0 * k;
            _musicB.volume = b0 * k;
            yield return null;
        }
        _musicA.Stop(); _musicB.Stop();
        _musicA.volume = _musicB.volume = 1f;
    }

    // ---------- SFX ----------
    public void PlaySFX(AudioClip clip, Vector3 pos, float vol = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSfxSource();
        src.transform.position = pos;
        src.pitch = pitch;
        src.volume = vol;
        src.spatialBlend = 1f;
        src.clip = clip;
        src.Play();
    }

    public void PlayUI(AudioClip clip, float vol = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSfxSource();
        src.outputAudioMixerGroup = uiBus;
        src.spatialBlend = 0f;
        src.pitch = pitch;
        src.volume = vol;
        src.clip = clip;
        src.Play();
        // restore routing for the pool item on completion
        StartCoroutine(RestoreBusWhenDone(src, sfxBus));
    }

    private IEnumerator RestoreBusWhenDone(AudioSource src, AudioMixerGroup bus)
    {
        while (src.isPlaying) yield return null;
        src.outputAudioMixerGroup = bus;
    }

    private AudioSource GetFreeSfxSource()
    {
        foreach (var s in _sfxPool) if (!s.isPlaying) return s;
        return _sfxPool[0]; // steal oldest if all busy (keeps it simple)
    }

    // ---------- Volume (0..1 linear) ----------
    public void SetMasterVolume(float v) => masterMixer.SetFloat("MasterVol", LinToDb(v));
    public void SetMusicVolume(float v) => masterMixer.SetFloat("MusicVol", LinToDb(v));
    public void SetSFXVolume(float v) => masterMixer.SetFloat("SFXVol", LinToDb(v));
    public void SetVoiceVolume(float v) => masterMixer.SetFloat("VoiceVol", LinToDb(v));
    public void SetUIVolume(float v) => masterMixer.SetFloat("UIVol", LinToDb(v));

    private float LinToDb(float v) => Mathf.Approximately(v, 0f) ? -80f : 20f * Mathf.Log10(Mathf.Clamp01(v));
}
