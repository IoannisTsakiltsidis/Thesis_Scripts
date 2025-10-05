using UnityEngine;

public class PlayMusicOnStart : MonoBehaviour
{
    public AudioClip clip;
    public float fadeSeconds = 1f;

    void Start()
    {
        if (clip && AudioManager.I) AudioManager.I.PlayMusic(clip, fadeSeconds);
    }
}
