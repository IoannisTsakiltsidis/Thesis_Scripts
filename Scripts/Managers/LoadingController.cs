using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public Slider progressBar;            // optional
    public TMPro.TMP_Text percentText;    // optional

    IEnumerator Start()
    {
        var req = GameFlowManager.Pending;
        float shown = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(req.targetScene, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            UpdateUI(op.progress);
            shown += Time.unscaledDeltaTime;
            yield return null;
        }

        while (shown < req.minLoadingSeconds)
        {
            UpdateUI(0.9f);
            shown += Time.unscaledDeltaTime;
            yield return null;
        }

        UpdateUI(1f);
        op.allowSceneActivation = true;
    }

    void UpdateUI(float p)
    {
        if (progressBar) progressBar.value = Mathf.Clamp01(p);
        if (percentText) percentText.text = $"{Mathf.RoundToInt(Mathf.Clamp01(p) * 100f)}%";
    }
}
