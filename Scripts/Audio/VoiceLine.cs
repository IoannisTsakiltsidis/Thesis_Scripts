using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Voice Line")]
public class VoiceLine : ScriptableObject
{
    public string id;              // optional code/key
    public string speaker;         // e.g., "Amalia"
    public AudioClip clip;
    [TextArea] public string subtitle; // simple single-line subtitle fallback
    public SubtitleSegment[] segments; // optional timed segments
    public bool interruptible = false; // if a new line can cut it

    [System.Serializable]
    public struct SubtitleSegment
    {
        public float time;     // seconds from start
        [TextArea] public string text;
        public float duration; // if <= 0, auto until next segment
    }
}
