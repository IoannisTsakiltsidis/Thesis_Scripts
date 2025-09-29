using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class VoiceLinePlayer : MonoBehaviour
{
    public static VoiceLinePlayer I { get; private set; }

    [Header("Routing")]
    public AudioMixer mixer;
    public string snapshotNormal = "Normal";
    public string snapshotDuck = "DuckDuringVoice";
    public float snapshotBlend = 0.15f;

    private AudioSource _voice;
    private readonly Queue<VoiceLine> _queue = new();

    public delegate void SubtitleEvent(string text, float duration);
    public event SubtitleEvent OnSubtitle;   // UI can subscribe

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _voice = gameObject.AddComponent<AudioSource>();
        _voice.playOnAwake = false;
        _voice.loop = false;
        _voice.spatialBlend = 0f; // voice is non-spatial by default (clean & clear)
        if (AudioManager.I) _voice.outputAudioMixerGroup = AudioManager.I.voiceBus;
    }

    public void Enqueue(VoiceLine line)
    {
        if (!line || !line.clip) return;

        if (line.interruptible && _voice.isPlaying)
        {
            StopAllCoroutines();
            _voice.Stop();
            ClearSubtitle();
        }

        _queue.Enqueue(line);
        if (!_voice.isPlaying) StartCoroutine(RunQueue());
    }

    private IEnumerator RunQueue()
    {
        while (_queue.Count > 0)
        {
            var line = _queue.Dequeue();

            // Duck music
            if (mixer) mixer.FindSnapshot(snapshotDuck)?.TransitionTo(snapshotBlend);

            _voice.clip = line.clip;
            _voice.volume = 1f;
            _voice.Play();

            // Subtitles
            if (line.segments != null && line.segments.Length > 0)
                yield return StartCoroutine(PlayTimedSubtitles(line));
            else if (!string.IsNullOrWhiteSpace(line.subtitle))
            {
                OnSubtitle?.Invoke(line.subtitle, line.clip.length);
                yield return new WaitForSeconds(line.clip.length);
                ClearSubtitle();
            }
            else
            {
                yield return new WaitForSeconds(line.clip.length);
            }

            // Restore music snapshot
            if (mixer) mixer.FindSnapshot(snapshotNormal)?.TransitionTo(snapshotBlend);
        }
    }

    private IEnumerator PlayTimedSubtitles(VoiceLine line)
    {
        // Assumes segments sorted by time
        var segs = line.segments;
        for (int i = 0; i < segs.Length; i++)
        {
            var now = segs[i];
            float nextTime = (i < segs.Length - 1) ? segs[i + 1].time : line.clip.length;
            float waitToStart = Mathf.Max(0f, now.time - _voice.time);
            yield return new WaitForSeconds(waitToStart);

            float dur = now.duration > 0 ? now.duration : Mathf.Max(0.1f, nextTime - now.time);
            OnSubtitle?.Invoke(now.text, dur);
            yield return new WaitForSeconds(dur);
            ClearSubtitle();
        }

        float tail = Mathf.Max(0f, line.clip.length - _voice.time);
        if (tail > 0f) yield return new WaitForSeconds(tail);
    }

    private void ClearSubtitle() => OnSubtitle?.Invoke("", 0f);
}
