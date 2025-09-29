// Assets/Scripts/System/SaveSystem.cs
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string FilePath =
        Path.Combine(Application.persistentDataPath, "savegame.json");

    public static bool HasSave() => File.Exists(FilePath);

    public static void Save(SaveData data)
    {
        try
        {
            data.savedAtUtc = System.DateTime.UtcNow.ToString("o");
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(FilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e}");
        }
    }

    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            var json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e}");
            return null;
        }
    }

    public static void Delete()
    {
        try { if (File.Exists(FilePath)) File.Delete(FilePath); }
        catch (System.Exception e) { Debug.LogError($"Delete failed: {e}"); }
    }
}
