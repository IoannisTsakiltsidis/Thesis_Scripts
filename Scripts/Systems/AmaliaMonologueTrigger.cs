using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmaliaMonologueTrigger : MonoBehaviour
{
    public Animator amaliaAnimator;
    public string faceLayerName = "Face Layer";     // EXACT layer name
    public string faceStateName = "Monologue_Good"; // EXACT state name
    public float fadeFixedTime = 0.03f;

    int faceLayerIndex = -1;
    int faceStateHash;
    bool fired;
    Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

        if (amaliaAnimator == null) { Debug.LogError("Assign Amalia Animator."); return; }

        faceLayerIndex = amaliaAnimator.GetLayerIndex(faceLayerName);
        Debug.Log($"[Monologue] Layer '{faceLayerName}' index = {faceLayerIndex}");
        if (faceLayerIndex < 0) return;

        amaliaAnimator.SetLayerWeight(faceLayerIndex, 1f);

        faceStateHash = Animator.StringToHash(faceStateName);
        bool hasState = amaliaAnimator.HasState(faceLayerIndex, faceStateHash);
        Debug.Log($"[Monologue] HasState('{faceStateName}') on layer {faceLayerIndex}: {hasState}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag("Player")) return;

        if (faceLayerIndex < 0) { Debug.LogError("Face layer not found."); return; }
        if (!amaliaAnimator.HasState(faceLayerIndex, faceStateHash))
        {
            Debug.LogError($"State '{faceStateName}' not found on layer '{faceLayerName}'.");
            return;
        }

        // Force the face layer to the monologue state at time 0
        amaliaAnimator.Play(faceStateHash, faceLayerIndex, 0f);
        // or: amaliaAnimator.CrossFadeInFixedTime(faceStateHash, fadeFixedTime, faceLayerIndex, 0f);

        Debug.Log("[Monologue] Triggered by: " + other.name);
        fired = true;
        col.enabled = false;   // never retrigger
        enabled = false;
    }
}
