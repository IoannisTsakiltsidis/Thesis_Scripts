using UnityEngine;

public class EnsureAudio : MonoBehaviour
{
    public GameObject audioPrefab; // contains AudioManager + VoiceLinePlayer
    void Awake()
    {
        if (AudioManager.I == null && audioPrefab)
        {
            Instantiate(audioPrefab);
            if (Camera.main) AudioManager.I.listenerTarget = Camera.main.transform;
        }
    }
}
