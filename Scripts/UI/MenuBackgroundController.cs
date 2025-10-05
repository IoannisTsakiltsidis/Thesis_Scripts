using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class MenuBackgroundController : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer video;       // VideoPlayer in the scene
    public CanvasGroup uiFade;      // CanvasGroup on your top-level UI root (buttons etc.)

    [Header("Behavior")]
    public string videoFileName = "menu.mp4"; // place under Assets/StreamingAssets/
    public bool prepareOnStart = true;        // pre-buffer before showing UI
    public bool playOnStart = true;           // auto-play after prepare
    public float fadeInTime = 0.5f;           // UI fade-in on start

    void Reset()
    {
        // Try to auto-find CanvasGroup on this GameObject
        if (!uiFade) uiFade = GetComponentInChildren<CanvasGroup>(true);
        if (!video) video = GetComponentInChildren<VideoPlayer>(true);
    }

    IEnumerator Start()
    {
        // Prepare video first for a stutter-free first frame
        if (video && prepareOnStart)
        {
            video.source = VideoSource.Url;
            video.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            video.isLooping = true;
            video.playOnAwake = false;
            video.waitForFirstFrame = true;
            video.skipOnDrop = true;

            video.Prepare();
            while (!video.isPrepared)
                yield return null;

            if (playOnStart) video.Play();
        }

        // Fade UI in after video is ready
        if (uiFade)
        {
            uiFade.alpha = 0f;
            float t = 0f;
            while (t < fadeInTime)
            {
                t += Time.unscaledDeltaTime;
                uiFade.alpha = Mathf.SmoothStep(0f, 1f, t / fadeInTime);
                yield return null;
            }
            uiFade.alpha = 1f;
        }
    }

    public IEnumerator FadeOut(float seconds = 0.4f)
    {
        if (!uiFade) yield break;

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            uiFade.alpha = Mathf.SmoothStep(1f, 0f, t / seconds);
            yield return null;
        }
        uiFade.alpha = 0f;
    }
}
