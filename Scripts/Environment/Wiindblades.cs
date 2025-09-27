using UnityEngine;

/// <summary>
/// WindGeneratorRotator wiring steps:
/// 1. Assign the WT Blade Connector transform to the Hub field.
/// 2. Choose the axis that matches the hub's local rotation axis.
/// 3. Configure the rotation direction and RPM to match the turbine design.
/// 4. Place this component on the blades GameObject to spin them around the connector's chosen local axis.
/// </summary>
[DisallowMultipleComponent]
public class WindGeneratorRotator : MonoBehaviour
{
    [Tooltip("Assign the WT Blade Connector transform that acts as the hub.")]
    public Transform hub;

    public enum HubAxis
    {
        X,
        Y,
        Z
    }

    [Tooltip("Select which local axis on the hub the blades should rotate around.")]
    public HubAxis hubAxis = HubAxis.X;

    [Tooltip("Rotation speed in revolutions per minute.")]
    public float rpm = 20f;

    [Tooltip("Set to false to spin the blades counter-clockwise when viewed along the chosen axis.")]
    public bool clockwise = true;

    private void Update()
    {
        if (hub == null)
        {
            return;
        }

        float degrees = rpm * 6f * Time.deltaTime;
        if (!clockwise)
        {
            degrees = -degrees;
        }

        Vector3 localAxis = GetLocalAxis();
        Vector3 worldAxis = hub.TransformDirection(localAxis);
        Vector3 selfAxis = transform.InverseTransformDirection(worldAxis).normalized;
        transform.Rotate(selfAxis, degrees, Space.Self);
    }

    private Vector3 GetLocalAxis()
    {
        switch (hubAxis)
        {
            case HubAxis.Y:
                return Vector3.up;
            case HubAxis.Z:
                return Vector3.forward;
            default:
                return Vector3.right;
        }
    }
}