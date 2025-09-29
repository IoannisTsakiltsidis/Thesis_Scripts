using UnityEngine;
public class PlayerLifetime : MonoBehaviour
{
    static PlayerLifetime _instance;
    void Awake() { if (_instance != null && _instance != this) { Destroy(gameObject); return; } _instance = this; DontDestroyOnLoad(gameObject); }
}
