// Assets/Scripts/System/SceneAutosaveOnLoad.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutosaveOnLoad : MonoBehaviour
{
    [Tooltip("SpawnId that placed the player in THIS scene (e.g., SpawnIds.From_Corridor).")]
    public string spawnUsed = "From_Start";

    void Start()
    {
        var scene = SceneManager.GetActiveScene().name;
        SaveSystem.Save(new SaveData { sceneName = scene, spawnId = spawnUsed });
        Debug.Log($"[Autosave] {scene} @ {spawnUsed}");
    }
}
