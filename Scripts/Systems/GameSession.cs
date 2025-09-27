// Assets/Scripts/System/GameSession.cs
using UnityEngine;

public static class GameSession
{
    private const string Key = "PendingSpawnId";
    public static void SetPendingSpawnId(string spawnId) => PlayerPrefs.SetString(Key, spawnId);
    public static string ConsumePendingSpawnId()
    {
        var v = PlayerPrefs.GetString(Key, SpawnIds.From_Start);
        return v;
    }
}
