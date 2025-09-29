using UnityEngine;
public class PlayerSpawnAnchor : MonoBehaviour
{
    public void TeleportTo(Vector3 pos, Quaternion rot)
    {
        var cc = GetComponent<CharacterController>();
        if (cc && cc.enabled) cc.enabled = false;
        transform.SetPositionAndRotation(pos, rot);
        if (cc) cc.enabled = true;
    }
}
