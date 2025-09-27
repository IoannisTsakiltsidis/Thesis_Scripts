using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitlesUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeIn = 0.08f;
    public float fadeOut = 0.15f;

    private CanvasGroup _cg;
    private Coroutine _fx;

    private void Awake()
    {
        _cg = gameObject.AddComponent<CanvasGroup>();
        _cg.alpha = 0f;
    }

    private void OnEnable()
    {
        if (VoiceLinePlayer.I != null)
            VoiceLinePlayer.I.OnSubtitle += Handle;
    }

    private void OnDisable()
    {
        if (VoiceLinePlayer.I != null)
            VoiceLinePlayer.I.OnSubtitle -= Handle;
    }

    private void Handle(string content, float duration)
    {
        if (_fx != null) StopCoroutine(_fx);
        _fx = StartCoroutine(Show(content, duration));
    }

    private IEnumerator Show(string content, float duration)
    {
        text.text = content;
        // Fade in
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            _cg.alpha = Mathf.Lerp(0f, 1f, t / fadeIn);
            yield return null;
        }
        _cg.alpha = 1f;

        // Hold
        if (duration > 0f) yield return new WaitForSeconds(duration);

        // Fade out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            _cg.alpha = Mathf.Lerp(1f, 0f, t / fadeOut);
            yield return null;
        }
        _cg.alpha = 0f;
        text.text = "";
    }
}
