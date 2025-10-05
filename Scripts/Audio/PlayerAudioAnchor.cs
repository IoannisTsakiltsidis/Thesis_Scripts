using UnityEngine;
public class PlayerAudioAnchor : MonoBehaviour
{
    void OnEnable() { if (AudioManager.I) AudioManager.I.listenerTarget = transform; }
}
