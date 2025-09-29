using UnityEngine;

[DisallowMultipleComponent]
public class FadeOutWhenNear : MonoBehaviour
{
    [Tooltip("Camera to measure distance from. If empty, uses Camera.main.")]
    public Transform cameraTransform;

    [Tooltip("Start fading when the camera is CLOSER than this (m).")]
    [Min(0f)] public float startFade = 40f;

    [Tooltip("Fade length in meters (near -> invisible, far -> fully visible).")]
    [Min(0.1f)] public float fadeLength = 10f;

    [Tooltip("Use the combined renderer bounds for distance instead of the root pivot.")]
    public bool useBounds = true;

    private Renderer[] _rends;
    private Bounds _combined;
    private Transform _cam;
    private MaterialPropertyBlock _mpb;

    private static readonly int ID_BaseColor = Shader.PropertyToID("_BaseColor"); // HDRP/URP Lit
    private static readonly int ID_Color = Shader.PropertyToID("_Color");     // Legacy/others

    void Awake()
    {
        // Avoid conflicts with LOD crossfade
        var lod = GetComponent<LODGroup>();
        if (lod) lod.enabled = false;

        _cam = cameraTransform
             ? cameraTransform
             : (Camera.main ? Camera.main.transform : FindObjectOfType<Camera>()?.transform);

        _rends = GetComponentsInChildren<Renderer>(true);
        _mpb = new MaterialPropertyBlock();

        if (_rends != null && _rends.Length > 0)
        {
            _combined = _rends[0].bounds;
            for (int i = 1; i < _rends.Length; i++)
                if (_rends[i]) _combined.Encapsulate(_rends[i].bounds);
        }
    }

    void LateUpdate()
    {
        if (_cam == null || _rends == null) return;

        float d = useBounds
            ? DistanceToBounds(_cam.position, _combined)
            : Vector3.Distance(_cam.position, transform.position);

        // 0 near, 1 far  → fades OUT when approaching
        // Option 1: swap the bounds
        float a = Mathf.InverseLerp(startFade + fadeLength, startFade, d);



        foreach (var r in _rends)
        {
            if (!r) continue;

            r.GetPropertyBlock(_mpb);

            // Try HDRP/URP first
            if (r.sharedMaterial && r.sharedMaterial.HasProperty(ID_BaseColor))
            {
                var c = r.sharedMaterial.GetColor(ID_BaseColor);
                c.a = a;
                _mpb.SetColor(ID_BaseColor, c);
            }

            // Also set legacy _Color in case some submaterials use it
            if (r.sharedMaterial && r.sharedMaterial.HasProperty(ID_Color))
            {
                var c2 = r.sharedMaterial.GetColor(ID_Color);
                c2.a = a;
                _mpb.SetColor(ID_Color, c2);
            }

            r.SetPropertyBlock(_mpb);
        }
    }

    static float DistanceToBounds(Vector3 p, Bounds b)
    {
        if (b.size == Vector3.zero) return 0f;
        var q = b.ClosestPoint(p);
        return Vector3.Distance(p, q);
    }
}
