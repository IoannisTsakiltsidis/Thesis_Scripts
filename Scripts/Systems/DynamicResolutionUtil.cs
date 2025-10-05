using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.HighDefinition; // HDRenderPipelineAsset
#endif

/// <summary>
/// Single entry point to enable/disable Dynamic Resolution across HDRP versions.
/// If public API isn't available, it toggles the HDRP asset flag and refreshes quality.
/// </summary>
public static class DynamicResolutionUtil
{
    public static void SetActive(bool on)
    {
        // Try public static API (newer HDRP)
        var t = typeof(DynamicResolutionHandler);
        var setActiveStatic = t.GetMethod("SetActive", BindingFlags.Public | BindingFlags.Static);
        if (setActiveStatic != null) { setActiveStatic.Invoke(null, new object[] { on }); return; }

        // Try instance API (some HDRP versions)
        var instanceProp = t.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
        var setActiveInst = t.GetMethod("SetActive", BindingFlags.Public | BindingFlags.Instance);
        if (instanceProp != null && setActiveInst != null)
        {
            var inst = instanceProp.GetValue(null);
            if (inst != null) { setActiveInst.Invoke(inst, new object[] { on }); return; }
        }

        // Fallback: flip the flag on the HDRP asset & refresh current quality level
        var rp = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
        if (rp != null)
        {
            var rps = rp.currentPlatformRenderPipelineSettings;
            var dr = rps.dynamicResolutionSettings;
            dr.enabled = on;
            rps.dynamicResolutionSettings = dr;
            rp.currentPlatformRenderPipelineSettings = rps;

            QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel(), false);
            Debug.Log($"[DR] Fallback toggled Dynamic Resolution {(on ? "ON" : "OFF")} via HDRP asset.");
            return;
        }

        Debug.LogWarning("[DR] Unable to toggle Dynamic Resolution (no API and not HDRP).");
    }
}
