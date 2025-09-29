using UnityEngine;

public class DoorToCorridorSimple : MonoBehaviour
{
    [Header("Target Corridor Pass")]
    [Tooltip("Set this per door: Scenes.CorridorPass2 (from STREET) or Scenes.CorridorPass3 (from FOREST).")]
    public string corridorScene;          // e.g., Scenes.CorridorPass2 / Scenes.CorridorPass3

    [Header("Spawn inside that Corridor pass")]
    [Tooltip("SpawnIds.From_Street (coming from STREET) or SpawnIds.From_Forest (coming from FOREST).")]
    public string corridorSpawnId;        // e.g., SpawnIds.From_Street / SpawnIds.From_Forest

    [Header("UI & Safety")]
    public GameObject promptUI;           // optional "Press E"
    public float armDelay = 0.75f;

    bool inside, fired; float ready;

    void OnEnable() { fired = false; inside = false; ready = Time.unscaledDeltaTime + Time.unscaledTime + armDelay; if (promptUI) promptUI.SetActive(false); }
    void OnTriggerEnter(Collider other) { if (!other.CompareTag("Player")) return; inside = true; if (Time.unscaledTime >= ready && promptUI) promptUI.SetActive(true); }
    void OnTriggerExit(Collider other) { if (!other.CompareTag("Player")) return; inside = false; if (promptUI) promptUI.SetActive(false); }

    void Update()
    {
        if (!inside) return;
        if (Input.GetKeyDown(KeyCode.E)) Go();
    }

    void Go()
    {
        if (fired) return;
        if (string.IsNullOrEmpty(corridorScene) || string.IsNullOrEmpty(corridorSpawnId))
        {
            Debug.LogWarning($"{name}: Set corridorScene and corridorSpawnId.");
            return;
        }

        fired = true;
        if (promptUI) promptUI.SetActive(false);

        // Hard switch via loading screen
        GameFlowManager.I.LoadWithLoading(corridorScene, corridorSpawnId);
    }
}
