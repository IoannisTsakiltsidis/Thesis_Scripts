using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public string targetSceneName;        // Scenes.STREET or Scenes.FOREST
    public string targetSpawnIdInTarget;  // SpawnIds.From_Corridor
    public GameObject promptUI;           // optional
    public float armDelay = 0.75f;

    bool inside, fired; float ready;

    void OnEnable() { fired = false; inside = false; ready = Time.unscaledTime + armDelay; if (promptUI) promptUI.SetActive(false); }
    void OnTriggerEnter(Collider other) { if (!other.CompareTag("Player")) return; inside = true; if (Time.unscaledTime >= ready && promptUI) promptUI.SetActive(true); }
    void OnTriggerExit(Collider other) { if (!other.CompareTag("Player")) return; inside = false; if (promptUI) promptUI.SetActive(false); }
    void Update() { if (inside && Input.GetKeyDown(KeyCode.E)) Go(); }

    void Go()
    {
        if (fired) return;
        fired = true; if (promptUI) promptUI.SetActive(false);
        GameFlowManager.I.LoadWithLoading(targetSceneName, targetSpawnIdInTarget);
    }
}

