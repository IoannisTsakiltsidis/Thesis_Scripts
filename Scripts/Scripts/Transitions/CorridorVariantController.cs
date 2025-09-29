using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class SceneDoorMap { public string sceneName; public GameObject doorRoot; }

public class CorridorVariantController : MonoBehaviour
{
    public List<SceneDoorMap> sceneDoors = new List<SceneDoorMap>();
    public bool disableUnmappedDoors = true;

    Dictionary<string, GameObject> map;

    void Awake()
    {
        map = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        foreach (var m in sceneDoors) if (m != null && !string.IsNullOrEmpty(m.sceneName) && m.doorRoot) map[m.sceneName] = m.doorRoot;
    }

    void OnEnable()
    {
        foreach (var kv in map) if (kv.Value) kv.Value.SetActive(false);

        var prev = TravelContext.PreviousScene;
        var next = TravelContext.NextScene;

        if (!string.IsNullOrEmpty(prev) && map.TryGetValue(prev, out var prevDoor) && prevDoor) prevDoor.SetActive(false);
        if (!string.IsNullOrEmpty(next) && map.TryGetValue(next, out var nextDoor) && nextDoor) nextDoor.SetActive(true);
    }
}
