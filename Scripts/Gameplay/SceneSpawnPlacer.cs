// Assets/Scripts/Systems/SceneSpawnPlacer.cs
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneSpawnPlacer : MonoBehaviour
{
    [Tooltip("Leave empty to use the request's spawnId from your loader (GameFlowManager.Pending.targetSpawnId).")]
    public string spawnIdOverride;

    [Header("If there's no Player in the scene, instantiate this prefab.")]
    public GameObject playerPrefab;           // <-- assign your Player prefab (tagged "Player", has PlayerSpawnAnchor)
    public string playerTag = "Player";

    void Start()
    {
        // 1) Ensure a Player with PlayerSpawnAnchor exists
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player && playerPrefab)
        {
            player = Instantiate(playerPrefab);
            player.name = "Player";
            Debug.Log("[SceneSpawnPlacer] Spawned Player prefab.");
        }

        var anchor = player ? player.GetComponent<PlayerSpawnAnchor>() : null;
        if (!anchor)
        {
            Debug.LogError("[SceneSpawnPlacer] No PlayerSpawnAnchor found (and no prefab to spawn). Cannot place player.");
            return;
        }

        // 2) Decide which spawn id to use
        string id = string.IsNullOrEmpty(spawnIdOverride)
            ? GameFlowManager.Pending.targetSpawnId   // your loader sets this
            : spawnIdOverride;

        // 3) Find a SpawnPoint with matching Id (or fallback)
        var all = FindObjectsOfType<SpawnPoint>(true);
        if (all.Length == 0)
        {
            Debug.LogError("[SceneSpawnPlacer] No SpawnPoint found in this scene.");
            return;
        }

        SpawnPoint target = null;
        if (!string.IsNullOrEmpty(id))
            target = all.FirstOrDefault(s => s.Id == id);

        if (!target)
        {
            target = all.FirstOrDefault();
            Debug.LogWarning($"[SceneSpawnPlacer] SpawnPoint '{id}' not found. Using first available: '{target.Id}'.");
        }

        // 4) Teleport using your existing anchor logic
        anchor.TeleportTo(target.transform.position, target.transform.rotation);

        // 5) Save current location (matches your previous behavior)
        SaveSystem.Save(new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            spawnId = target.Id
        });

        // 6) Sanity check camera
        var cam = player.GetComponentInChildren<Camera>(true);
        if (!cam) Debug.LogWarning("[SceneSpawnPlacer] Player has no Camera. You'll see 'No cameras rendering' if none exists.");
    }
}
