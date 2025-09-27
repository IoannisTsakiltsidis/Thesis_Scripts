// Assets/Scripts/UI/MainMenuUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button continueButton;

    void Awake()
    {
        if (continueButton)
            continueButton.interactable = SaveSystem.HasSave();
    }

    // Wire to Start button
    public void StartNewGame()
    {
        // Your very first scene & spawn:
        GameSession.SetPendingSpawnId(SpawnIds.From_Start);
        SceneManager.LoadScene(Scenes.CorridorPass1); // or your real start
    }

    // Wire to Continue button
    public void ContinueGame()
    {
        var data = SaveSystem.Load();
        if (data == null) return;

        GameSession.SetPendingSpawnId(data.spawnId);   // handoff to spawn placer
        SceneManager.LoadScene(data.sceneName);
    }

    // Wire to Exit button
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
