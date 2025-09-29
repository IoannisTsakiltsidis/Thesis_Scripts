using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager I { get; private set; }

    public struct LoadRequest
    {
        public string targetScene;
        public string targetSpawnId;
        public float minLoadingSeconds;
    }

    public static LoadRequest Pending;

    [Header("Defaults")]
    public float defaultMinLoadingSeconds = 1.0f;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadWithLoading(string targetScene, string targetSpawnId, float? minSeconds = null)
    {
        Pending = new LoadRequest
        {
            targetScene = targetScene,
            targetSpawnId = targetSpawnId,
            minLoadingSeconds = minSeconds ?? defaultMinLoadingSeconds
        };
        SceneManager.LoadScene(Scenes.Loading, LoadSceneMode.Single);
    }
}
