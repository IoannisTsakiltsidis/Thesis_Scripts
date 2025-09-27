// Assets/Scripts/UI/SettingsMenuUI.cs
using TMPro;
using UnityEngine;

public class SettingsMenuUI : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    const string Key = "QualityIndex";

    void OnEnable()
    {
        qualityDropdown.ClearOptions();
        var names = QualitySettings.names; // e.g., "Low/Medium/High/Ultra" or your custom
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(names));

        int current = QualitySettings.GetQualityLevel();
        qualityDropdown.SetValueWithoutNotify(current);
    }

    // Wire to dropdown OnValueChanged(int)
    public void OnQualityChanged(int index)
    {
        index = Mathf.Clamp(index, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(Key, index);
    }
}
