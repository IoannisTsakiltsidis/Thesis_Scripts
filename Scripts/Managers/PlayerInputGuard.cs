using UnityEngine;

public static class PlayerInputGuard
{
    static int lockCount;
    public static bool IsLocked => lockCount > 0;

    public static void SetLocked(bool locked)
    {
        if (locked) lockCount++;
        else lockCount = Mathf.Max(0, lockCount - 1);

        // Optional: integrate input system here.
        // var pi = Object.FindObjectOfType<UnityEngine.InputSystem.PlayerInput>();
        // if (pi) pi.enabled = !IsLocked;
    }
}
