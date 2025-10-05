using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Panels (optional)")]
    public GameObject settingsPanel;

    [Header("Focus (optional)")]
    public Selectable defaultFocused;      // set to START for keyboard/controller Enter

    [Header("Background (optional)")]
    public MenuBackgroundController bg;    // assign the BG_Controller with the script above

    void Awake()
    {
        if (continueButton) continueButton.interactable = SaveSystem.HasSave();

        // Wire events (or wire in Inspector if you prefer)
        if (startButton) startButton.onClick.AddListener(StartNewGame);
        if (continueButton) continueButton.onClick.AddListener(ContinueGame);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton) quitButton.onClick.AddListener(ExitGame);
    }

    void OnEnable()
    {
        if (defaultFocused) defaultFocused.Select();
    }

    public void StartNewGame()
    {
        StartCoroutine(PlayAndLoad(Scenes.CorridorPass1, SpawnIds.From_Start));
    }

    public void ContinueGame()
    {
        var data = SaveSystem.Load();
        if (data == null) return;
        StartCoroutine(PlayAndLoad(data.sceneName, data.spawnId));
    }

    private IEnumerator PlayAndLoad(string sceneName, string spawnId)
    {
        if (bg) yield return bg.FadeOut(0.4f); // optional polish
        GameFlowManager.I.LoadWithLoading(sceneName, spawnId);
    }

    public void OpenSettings() { if (settingsPanel) settingsPanel.SetActive(true); }
    public void CloseSettings() { if (settingsPanel) settingsPanel.SetActive(false); }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
