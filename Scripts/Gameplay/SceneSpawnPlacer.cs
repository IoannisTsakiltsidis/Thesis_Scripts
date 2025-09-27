using UnityEngine;

public class SceneSpawnPlacer : MonoBehaviour
{
    [Tooltip("Leave empty to use the request's spawnId.")]
    public string spawnIdOverride;

    void Start()
    {
        var anchor = FindObjectOfType<PlayerSpawnAnchor>();
        if (!anchor) return;

        string id = string.IsNullOrEmpty(spawnIdOverride)
            ? GameFlowManager.Pending.targetSpawnId
            : spawnIdOverride;

        if (string.IsNullOrEmpty(id))
        {
            var any = FindObjectOfType<SpawnPoint>();
            if (any) anchor.TeleportTo(any.transform.position, any.transform.rotation);
            return;
        }

        SpawnPoint target = null;
        foreach (var sp in FindObjectsOfType<SpawnPoint>())
            if (sp.Id == id) { target = sp; break; }

        if (target) anchor.TeleportTo(target.transform.position, target.transform.rotation);
    }
}
