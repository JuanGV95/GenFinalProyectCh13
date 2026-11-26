using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "Heroes/Camera Data")]
public class CameraData : ScriptableObject
{
    [Header("Mouse Sensitivity")]
    [Min(0f)] public float mouseSensitivityX = 0.15f;
    [Min(0f)] public float mouseSensitivityY = 0.15f;

    [Header("Gamepad Sensitivity")]
    [Min(0f)] public float gamepadSensitivityX = 180f;
    [Min(0f)] public float gamepadSensitivityY = 120f;

    public bool invertY;

    [Header("Orbit")]
    [Range(-89f, 89f)] public float minPitch = -60f;
    [Range(-89f, 89f)] public float maxPitch = 70f;
    public float initialPitch = 10f;

    [Header("Position")]
    [Min(0.1f)] public float distance = 3.5f;
    public float shoulderOffset = 0.65f;
    public float verticalOffset = 0.15f;

    [Min(0f)] public float followSmoothTime = 0.04f;

    [Header("Lens")]
    [Range(30f, 100f)] public float fieldOfView = 65f;

    [Header("Collision")]
    public LayerMask obstacleLayers;

    [Min(0.01f)] public float collisionRadius = 0.25f;
    [Min(0f)] public float collisionPadding = 0.05f;
    [Min(0.01f)] public float collisionRecoverySpeed = 12f;
}