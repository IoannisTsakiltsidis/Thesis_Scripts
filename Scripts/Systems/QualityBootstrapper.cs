// Assets/Scripts/System/QualityBootstrapper.cs
using UnityEngine;

public class QualityBootstrapper : MonoBehaviour
{
    const string Key = "QualityIndex";

    void Awake()
    {
        if (PlayerPrefs.HasKey(Key))
        {
            int idx = PlayerPrefs.GetInt(Key);
            idx = Mathf.Clamp(idx, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(idx, true);
        }
    }
}
