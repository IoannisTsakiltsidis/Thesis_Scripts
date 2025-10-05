// Assets/Scripts/UI/HideSpatialUIOnTrigger.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HideSpatialUIOnTrigger : MonoBehaviour
{
    [Header("Who can trigger")]
    [Tooltip("Only react to colliders tagged as this (usually 'Player').")]
    public string playerTag = "Player";

    [Header("What to hide")]
    [Tooltip("Root of the spatial UI or prop to hide.")]
    public GameObject targetRoot;

    [Tooltip("Destroy the target instead of SetActive(false).")]
    public bool destroyTarget = false;

    [Tooltip("Optional delay (seconds) before hiding/destroying.")]
    [Min(0f)] public float delay = 0f;

    [Tooltip("Prevent running more than once.")]
    public bool runOnlyOnce = true;

    [Header("Events (optional)")]
    public UnityEvent onHidden;

    bool _done;

    void Reset()
    {
        // Make sure this collider is a trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_done && runOnlyOnce) return;
        if (!other || (playerTag.Length > 0 && !other.CompareTag(playerTag))) return;
        if (!targetRoot) return;

        _done = true;
        if (delay > 0f) Invoke(nameof(HideNow), delay);
        else HideNow();
    }

    // You can also call this from your existing VoiceLine trigger script if you prefer:
    public void TriggerHide() => HideNow();

    void HideNow()
    {
        if (!targetRoot) return;

        // Disable any fade scripts so it doesn't keep touching material props
        foreach (var fade in targetRoot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (fade && fade.GetType().Name == "FadeOutWhenNear") // loose match to avoid direct dependency
                fade.enabled = false;
        }

        if (destroyTarget)
            Destroy(targetRoot);
        else
            targetRoot.SetActive(false);

        onHidden?.Invoke();
    }
}
