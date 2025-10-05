using UnityEngine;

public class EndSceneVoiceOnStart : MonoBehaviour
{
    [Header("What to play")]
    public VoiceLine finalLine;

    [Header("Timing")]
    public float delaySeconds = 0.5f;   // wait before speaking

    [Header("Music control")]
    public bool stopMusic = true;       // keep the end silent?
    public float musicFadeOut = 0.5f;

    void Start()
    {
        if (stopMusic && AudioManager.I) AudioManager.I.StopMusic(musicFadeOut);
        if (finalLine && VoiceLinePlayer.I)
            Invoke(nameof(PlayNow), delaySeconds);
    }

    void PlayNow()
    {
        VoiceLinePlayer.I.Enqueue(finalLine);
    }
}
