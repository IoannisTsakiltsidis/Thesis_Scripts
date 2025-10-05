using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VoiceLineTrigger : MonoBehaviour
{
    [Header("What to play")]
    public VoiceLine line;

    [Header("Behaviour")]
    public bool oneShot = true;        // fire once
    public float delaySeconds = 0f;    // optional delay

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Ensure triggers fire: put a kinematic RB on one side if needed
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!line || VoiceLinePlayer.I == null) return;

        if (delaySeconds > 0f) Invoke(nameof(PlayNow), delaySeconds);
        else PlayNow();

        if (oneShot) gameObject.SetActive(false);
    }

    void PlayNow() => VoiceLinePlayer.I.Enqueue(line);
}
