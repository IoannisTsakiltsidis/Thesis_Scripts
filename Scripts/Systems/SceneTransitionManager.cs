using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct SceneLoadStep
{
    public string scene;
    public string spawnId;
    public SceneLoadStep(string scene, string spawnId = null)
    {
        this.scene = scene; this.spawnId = spawnId;
    }
}

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager I { get; private set; }

    [Header("Fade UI")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 0.45f;

    [Header("Loading")]
    [SerializeField] private bool useLoadingScreen = false;
    [SerializeField] private bool lockInputDuringTransitions = true;

    bool isTransitioning;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        if (fadeCanvas) { fadeCanvas.alpha = 0f; fadeCanvas.blocksRaycasts = false; }
    }

    public void Transition(string targetSceneName, string targetSpawnId = null)
    {
        if (isTransitioning) return;
        StartCoroutine(Co_Transition(new[] { new SceneLoadStep(targetSceneName, targetSpawnId) }));
    }

    public void TransitionChain(params SceneLoadStep[] steps)
    {
        if (isTransitioning) return;
        StartCoroutine(Co_Transition(steps));
    }

    IEnumerator Co_Transition(SceneLoadStep[] steps)
    {
        isTransitioning = true;
        if (lockInputDuringTransitions) PlayerInputGuard.SetLocked(true);

        // Fade out
        yield return Co_Fade(1f);

        // Load steps additively
        var loaded = new List<Scene>();
        foreach (var step in steps)
        {
            var op = SceneManager.LoadSceneAsync(step.scene, LoadSceneMode.Additive);
            if (useLoadingScreen) op.allowSceneActivation = false;

            if (useLoadingScreen)
            {
                while (op.progress < 0.9f) yield return null;
                op.allowSceneActivation = true;
            }
            while (!op.isDone) yield return null;

            loaded.Add(SceneManager.GetSceneByName(step.scene));
        }

        // Set last scene active
        var active = loaded[loaded.Count - 1];
        SceneManager.SetActiveScene(active);

        // Pick last non-null spawnId
        string finalSpawn = null;
        for (int i = steps.Length - 1; i >= 0 && finalSpawn == null; i--)
            if (!string.IsNullOrEmpty(steps[i].spawnId)) finalSpawn = steps[i].spawnId;

        if (!string.IsNullOrEmpty(finalSpawn)) PositionPlayerAtSpawn(finalSpawn);
        else PositionPlayerAtFallback();

        // Unload everything except the manager's scene and the loaded list
        yield return Co_UnloadAllExcept(loaded);

        // Fade in
        yield return Co_Fade(0f);

        if (lockInputDuringTransitions) PlayerInputGuard.SetLocked(false);
        isTransitioning = false;
    }

    void PositionPlayerAtSpawn(string spawnId)
    {
        var anchor = FindObjectOfType<PlayerSpawnAnchor>();
        if (!anchor) return;

        foreach (var sp in FindObjectsOfType<SpawnPoint>())
        {
            if (sp.Id == spawnId)
            {
                anchor.TeleportTo(sp.transform.position, sp.transform.rotation);
                return;
            }
        }
        PositionPlayerAtFallback();
    }

    void PositionPlayerAtFallback()
    {
        var anchor = FindObjectOfType<PlayerSpawnAnchor>();
        if (!anchor) return;
        var first = FindObjectOfType<SpawnPoint>();
        if (first) anchor.TeleportTo(first.transform.position, first.transform.rotation);
    }

    IEnumerator Co_UnloadAllExcept(List<Scene> keepScenes)
    {
        var keepNames = new HashSet<string>();
        foreach (var s in keepScenes) keepNames.Add(s.name);

        // Collect first to avoid index issues while unloading
        var toUnload = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s == gameObject.scene) continue;         // keep Persistent
            if (keepNames.Contains(s.name)) continue;    // keep chain
            if (s.isLoaded) toUnload.Add(s);
        }

        foreach (var s in toUnload)
        {
            var op = SceneManager.UnloadSceneAsync(s);
            while (!op.isDone) yield return null;
        }
    }

    IEnumerator Co_Fade(float target)
    {
        if (!fadeCanvas) yield break;
        fadeCanvas.blocksRaycasts = true;
        float start = fadeCanvas.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = target;
        fadeCanvas.blocksRaycasts = (target >= 0.99f);
    }
}
