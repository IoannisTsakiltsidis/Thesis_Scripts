using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class ConditionalSet { public string whenNextScene; public List<GameObject> enable = new(); public List<GameObject> disable = new(); }

public class CorridorConditionalSets : MonoBehaviour
{
    public List<ConditionalSet> sets = new();
    void OnEnable()
    {
        var next = TravelContext.NextScene;
        foreach (var s in sets)
        {
            bool match = !string.IsNullOrEmpty(next) && string.Equals(next, s.whenNextScene, StringComparison.Ordinal);
            if (match) { foreach (var go in s.enable) if (go) go.SetActive(true); foreach (var go in s.disable) if (go) go.SetActive(false); }
        }
    }
}
