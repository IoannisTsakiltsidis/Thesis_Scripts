// Assets/Scripts/System/SaveData.cs
using System;

[Serializable]
public class SaveData
{
    public string sceneName;     // e.g., Scenes.STREET
    public string spawnId;       // e.g., SpawnIds.From_Corridor
    public string savedAtUtc;    // diagnostics
}
