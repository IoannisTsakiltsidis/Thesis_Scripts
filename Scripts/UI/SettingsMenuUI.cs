// Assets/Scripts/UI/SettingsMenuUI.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Quality")]
    public TMP_Dropdown qualityDropdown;

    [Header("Audio (0..1)")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;
    public Slider voiceSlider;

    [Header("Display")]
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;

    // PlayerPrefs keys
    const string K_QUALITY = "opt_quality";
    const string K_MASTER = "opt_master";
    const string K_MUSIC = "opt_music";
    const string K_SFX = "opt_sfx";
    const string K_UI = "opt_ui";
    const string K_VOICE = "opt_voice";
    const string K_FS = "opt_fs";
    const string K_VSYNC = "opt_vsync";

    void OnEnable()
    {
        // ----- Quality -----
        if (qualityDropdown)
        {
            qualityDropdown.ClearOptions();

            // FIX: make the type explicit so AddOptions picks the correct overload
            var names = new List<string>(QualitySettings.names);
            qualityDropdown.AddOptions(names);

            int q = PlayerPrefs.GetInt(K_QUALITY, QualitySettings.GetQualityLevel());
            q = Mathf.Clamp(q, 0, QualitySettings.names.Length - 1);
            qualityDropdown.SetValueWithoutNotify(q);
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        // ----- Load saved values -----
        float master = PlayerPrefs.GetFloat(K_MASTER, 0.8f);
        float music = PlayerPrefs.GetFloat(K_MUSIC, 0.8f);
        float sfx = PlayerPrefs.GetFloat(K_SFX, 0.8f);
        float ui = PlayerPrefs.GetFloat(K_UI, 0.8f);
        float voice = PlayerPrefs.GetFloat(K_VOICE, 0.8f);
        bool fs = PlayerPrefs.GetInt(K_FS, 1) == 1;
        bool vsync = PlayerPrefs.GetInt(K_VSYNC, 1) == 1;

        SetSlider(masterSlider, master, OnMasterChanged);
        SetSlider(musicSlider, music, OnMusicChanged);
        SetSlider(sfxSlider, sfx, OnSfxChanged);
        SetSlider(uiSlider, ui, OnUiChanged);
        SetSlider(voiceSlider, voice, OnVoiceChanged);

        if (fullscreenToggle)
        {
            fullscreenToggle.SetIsOnWithoutNotify(fs);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
        if (vSyncToggle)
        {
            vSyncToggle.SetIsOnWithoutNotify(vsync);
            vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        }

        // Apply immediately
        ApplyVolumes(master, music, sfx, ui, voice);
        Screen.fullScreen = fs;
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }

    void OnDisable()
    {
        qualityDropdown?.onValueChanged.RemoveAllListeners();
        masterSlider?.onValueChanged.RemoveAllListeners();
        musicSlider?.onValueChanged.RemoveAllListeners();
        sfxSlider?.onValueChanged.RemoveAllListeners();
        uiSlider?.onValueChanged.RemoveAllListeners();
        voiceSlider?.onValueChanged.RemoveAllListeners();
        fullscreenToggle?.onValueChanged.RemoveAllListeners();
        vSyncToggle?.onValueChanged.RemoveAllListeners();
    }

    // ---------- UI helpers ----------
    void SetSlider(Slider s, float value, UnityEngine.Events.UnityAction<float> onChange)
    {
        if (!s) return;
        s.minValue = 0f; s.maxValue = 1f;
        s.SetValueWithoutNotify(value);
        s.onValueChanged.AddListener(onChange);
    }

    float Val(Slider s) => s ? s.value : 0.8f;

    // ---------- Handlers ----------
    public void OnQualityChanged(int index)
    {
        index = Mathf.Clamp(index, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(K_QUALITY, index);
    }

    public void OnMasterChanged(float v) { PlayerPrefs.SetFloat(K_MASTER, v); ApplyVolumes(v, Val(musicSlider), Val(sfxSlider), Val(uiSlider), Val(voiceSlider)); }
    public void OnMusicChanged(float v) { PlayerPrefs.SetFloat(K_MUSIC, v); ApplyVolumes(Val(masterSlider), v, Val(sfxSlider), Val(uiSlider), Val(voiceSlider)); }
    public void OnSfxChanged(float v) { PlayerPrefs.SetFloat(K_SFX, v); ApplyVolumes(Val(masterSlider), Val(musicSlider), v, Val(uiSlider), Val(voiceSlider)); }
    public void OnUiChanged(float v) { PlayerPrefs.SetFloat(K_UI, v); ApplyVolumes(Val(masterSlider), Val(musicSlider), Val(sfxSlider), v, Val(voiceSlider)); }
    public void OnVoiceChanged(float v) { PlayerPrefs.SetFloat(K_VOICE, v); ApplyVolumes(Val(masterSlider), Val(musicSlider), Val(sfxSlider), Val(uiSlider), v); }

    public void OnFullscreenChanged(bool on) { PlayerPrefs.SetInt(K_FS, on ? 1 : 0); Screen.fullScreen = on; }
    public void OnVSyncChanged(bool on) { PlayerPrefs.SetInt(K_VSYNC, on ? 1 : 0); QualitySettings.vSyncCount = on ? 1 : 0; }

    // ---------- Apply using your AudioManager ----------
    void ApplyVolumes(float master, float music, float sfx, float ui, float voice)
    {
        var A = AudioManager.I; // your singleton
        if (A == null) return;  // ensure AudioManager exists in the menu scene
        A.SetMasterVolume(master);
        A.SetMusicVolume(music);
        A.SetSFXVolume(sfx);
        A.SetUIVolume(ui);
        A.SetVoiceVolume(voice);
        PlayerPrefs.Save();
    }
}
