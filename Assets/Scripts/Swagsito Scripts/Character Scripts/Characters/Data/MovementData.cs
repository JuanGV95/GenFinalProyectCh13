using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Heroes/Movement Data")]
public class MovementData : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)] public float walkSpeed = 2.5f;
    [Min(0f)] public float runSpeed = 5f;
    [Min(0f)] public float sprintSpeed = 8f;

    [Header("Responsiveness")]
    [Min(0f)] public float acceleration = 35f;
    [Min(0f)] public float deceleration = 45f;
    [Min(0f)] public float rotationSpeed = 1080f;

    [Header("Airborne")]
    [Min(0f)] public float jumpHeight = 1.5f;
    [Min(0.01f)] public float gravity = 25f;
    [Min(0.01f)] public float terminalVelocity = 40f;

    [Range(0f, 1f)]
    public float airControl = 0.6f;

    [Header("Landing")]
    [Min(0f)] public float landingDuration = 0.12f;

    [Header("Ground Detection")]
    [Min(0.01f)] public float groundCheckRadius = 0.25f;
    [Min(0.01f)] public float groundCheckDistance = 0.15f;

    [Range(0f, 89f)]
    public float maxSlopeAngle = 45f;

    public LayerMask groundLayer;
}